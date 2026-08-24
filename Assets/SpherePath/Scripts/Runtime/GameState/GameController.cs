using System;
using SpherePath.Configuration;
using SpherePath.Input;
using SpherePath.Obstacles;
using SpherePath.Pathing;
using SpherePath.Player;
using SpherePath.Level;
using SpherePath.Shooting;
using UnityEngine;

namespace SpherePath.GameState
{
    public sealed class GameController : IDisposable
    {
        private readonly GameplayConfiguration _configuration;
        private readonly LevelViewReferences _scene;
        private readonly TransientViewFactory _transientViewFactory;
        private readonly PlayerEnergy _energy;
        private readonly PlayerSizeService _playerSize;
        private readonly PlayerViabilityService _playerViability;
        private readonly ShotChargeService _shotCharge;
        private readonly PointerChargeInput _input;
        private readonly ObstacleClearingService _obstacleClearing;
        private readonly PathClearanceService _pathClearance;

        private GamePhase _phase;
        private Vector3 _moveTarget;
        private bool _shouldLoseAfterProjectileResolution;

        public GameController(
            GameplayConfiguration configuration,
            LevelViewReferences scene,
            TransientViewFactory transientViewFactory,
            PlayerEnergy energy,
            PlayerSizeService playerSize,
            PlayerViabilityService playerViability,
            ShotChargeService shotCharge,
            PointerChargeInput input,
            ObstacleClearingService obstacleClearing,
            PathClearanceService pathClearance)
        {
            _configuration = configuration;
            _scene = scene;
            _transientViewFactory = transientViewFactory;
            _energy = energy;
            _playerSize = playerSize;
            _playerViability = playerViability;
            _shotCharge = shotCharge;
            _input = input;
            _obstacleClearing = obstacleClearing;
            _pathClearance = pathClearance;
        }

        public void Initialize()
        {
            _energy.Changed += UpdateEnergyView;
            _scene.Ui.RestartClicked += ResetGame;
            ResetGame();
        }

        public void Dispose()
        {
            _energy.Changed -= UpdateEnergyView;
            _scene.Ui.RestartClicked -= ResetGame;
            _transientViewFactory.ClearTransients();
        }

        public void Tick(float deltaTime)
        {
            _input.Tick();

            if (_input.RestartRequested || ((_phase == GamePhase.Won || _phase == GamePhase.Lost) && _input.StartedThisFrame))
            {
                ResetGame();
                return;
            }

            if (_phase == GamePhase.Ready && _input.StartedThisFrame)
            {
                StartCharging();
            }
            else if (_phase == GamePhase.Ready)
            {
                _scene.Player.SetIdleFeedback();
            }

            if (_phase == GamePhase.Charging)
            {
                UpdateCharging(deltaTime);
            }

            if (_phase == GamePhase.Moving)
            {
                MovePlayer(deltaTime);
            }
        }

        private void ResetGame()
        {
            _transientViewFactory.ClearTransients();

            foreach (var obstacle in _scene.Obstacles)
            {
                obstacle.Restore();
            }

            _energy.Reset();
            _shotCharge.Begin();
            _phase = GamePhase.Ready;
            _shouldLoseAfterProjectileResolution = false;
            _scene.Player.SetPosition(_scene.PlayerSpawnPosition);
            UpdatePlayerRadius();
            SnapCameraToPlayer();
            SetDoorOpen(false);
            _scene.Ui.ShowPlaying();
            _scene.ChargePreview.gameObject.SetActive(false);
            ResetCameraShake();
            UpdateCorridor();
        }

        private void StartCharging()
        {
            _phase = GamePhase.Charging;
            _shotCharge.Begin();
            _scene.ChargePreview.gameObject.SetActive(true);
        }

        private void UpdateCharging(float deltaTime)
        {
            var reachedEnergyLimit = _shotCharge.TickUntilEnergyLimit(deltaTime);
            _scene.Player.SetChargeFeedback(_shotCharge.NormalizedCharge);
            _scene.CameraView.SetShake(_shotCharge.NormalizedCharge);
            UpdateChargePreview();
            UpdateProjectedEnergyView();

            if (reachedEnergyLimit)
            {
                _shouldLoseAfterProjectileResolution = true;
                FireChargedShot();
                return;
            }

            if (_input.ReleasedThisFrame)
            {
                FireChargedShot();
            }
        }

        private void UpdateChargePreview()
        {
            var radius = _shotCharge.ProjectileRadius;
            _scene.ChargePreview.position = _scene.Player.Position + Vector3.forward * (_scene.Player.Radius + radius + 0.25f);
            _scene.ChargePreview.localScale = Vector3.one * (radius * 2f);
        }

        private void FireChargedShot()
        {
            var shot = _shotCharge.CreateShot();
            _scene.ChargePreview.gameObject.SetActive(false);
            ResetCameraShake();

            if (!_shotCharge.TrySpend(shot))
            {
                _energy.Deplete();
                UpdatePlayerRadius();
                Lose();
                return;
            }

            UpdatePlayerRadius();
            _shouldLoseAfterProjectileResolution = _shouldLoseAfterProjectileResolution || _energy.IsDepleted;
            SpawnProjectile(shot.ProjectileRadius);
            _phase = GamePhase.ProjectileFlying;
        }

        private void SpawnProjectile(float radius)
        {
            var position = _scene.Player.Position + Vector3.forward * (_scene.Player.Radius + radius + 0.25f);
            var projectile = _transientViewFactory.CreateProjectile(position, radius);
            projectile.HitObstacle += ResolveProjectileHit;
            projectile.Expired += ResolveProjectileMiss;
        }

        private void ResolveProjectileHit(Obstacle obstacle, float projectileRadius)
        {
            var result = _obstacleClearing.ClearFromImpact(_scene.Obstacles, obstacle, projectileRadius);
            _transientViewFactory.ShowInfectionRadius(obstacle.Position, result.InfectionRadius);
            ResolvePathAfterShot();
        }

        private void ResolveProjectileMiss()
        {
            ResolvePathAfterShot();
        }

        private void ResolvePathAfterShot()
        {
            _moveTarget = _pathClearance.GetReachablePosition(_scene.Player.Position, _scene.DoorPosition, _scene.Player.Radius);
            var movedDistance = Vector3.Distance(_scene.Player.Position, _moveTarget);
            var shouldLoseAfterProjectileResolution = _shouldLoseAfterProjectileResolution;
            _shouldLoseAfterProjectileResolution = false;

            if (movedDistance > 0.05f)
            {
                _phase = GamePhase.Moving;
                return;
            }

            if (shouldLoseAfterProjectileResolution || ShouldLoseWhenBlocked())
            {
                Lose();
                return;
            }

            _phase = GamePhase.Ready;
        }

        private void MovePlayer(float deltaTime)
        {
            _scene.Player.SetPosition(Vector3.MoveTowards(_scene.Player.Position, _moveTarget, _configuration.PlayerMoveSpeed * deltaTime));
            UpdateCorridor();
            UpdateCameraFollow(deltaTime);

            if (Vector3.Distance(_scene.Player.Position, _scene.DoorPosition) <= _configuration.DoorOpenDistance)
            {
                SetDoorOpen(true);
            }

            if (Vector3.Distance(_scene.Player.Position, _moveTarget) <= 0.02f)
            {
                if (Vector3.Distance(_scene.Player.Position, _scene.DoorPosition) <= 0.4f)
                {
                    Win();
                    return;
                }

                if (ShouldLoseWhenBlocked())
                {
                    Lose();
                    return;
                }

                _phase = GamePhase.Ready;
            }
        }

        private void Win()
        {
            _phase = GamePhase.Won;
            SetDoorOpen(true);
            _scene.Ui.ShowResult("WIN", "Tap restart to play again");
        }

        private void Lose()
        {
            _phase = GamePhase.Lost;
            ResetCameraShake();
            _scene.Ui.ShowResult("LOSE", "Try a smaller shot next time");
        }

        private bool ShouldLoseWhenBlocked()
        {
            return _playerViability.IsDepleted;
        }

        private void ResetCameraShake()
        {
            _scene.CameraView.ResetShake();
        }

        private void UpdateCameraFollow(float deltaTime)
        {
            _scene.CameraView.Follow(_scene.Player.Position, deltaTime);
        }

        private void SnapCameraToPlayer()
        {
            _scene.CameraView.SnapToFollowTarget(_scene.Player.Position);
        }

        private void SetDoorOpen(bool isOpen)
        {
            _scene.DoorLeftPanel.localPosition = isOpen ? new Vector3(-0.9f, 0f, 0f) : new Vector3(-0.45f, 0f, 0f);
            _scene.DoorRightPanel.localPosition = isOpen ? new Vector3(0.9f, 0f, 0f) : new Vector3(0.45f, 0f, 0f);
        }

        private void UpdateEnergyView(float normalizedEnergy)
        {
            _scene.Ui.SetEnergy(normalizedEnergy);
        }

        private void UpdateProjectedEnergyView()
        {
            _scene.Ui.SetEnergy(_energy.GetNormalizedAfterSpend(_shotCharge.EnergyCost));
        }

        private void UpdatePlayerRadius()
        {
            var radius = _playerSize.GetRadius(_energy.Normalized);
            _scene.Player.SetRadius(radius);
            UpdateCorridor();
        }

        private void UpdateCorridor()
        {
            var start = _scene.Player.Position;
            var target = _scene.DoorPosition;
            var midpoint = (start + target) * 0.5f;
            var length = Vector3.Distance(start, target);
            _scene.Corridor.position = new Vector3(midpoint.x, 0.02f, midpoint.z);
            _scene.Corridor.localScale = new Vector3(_scene.Player.Radius * 2f, 0.04f, length);
        }
    }
}
