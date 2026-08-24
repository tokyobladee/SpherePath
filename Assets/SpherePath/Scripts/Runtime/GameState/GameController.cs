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
        private const float CompletionDelay = 0.75f;
        private const float CorridorVisualPadding = 0.45f;

        private static readonly int PathMinXProperty = Shader.PropertyToID("_PathMinX");
        private static readonly int PathMaxXProperty = Shader.PropertyToID("_PathMaxX");

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
        private float _impactShakeTime;
        private int _pendingObstacleDestructionCount;
        private bool _isDoorOpen;
        private float _doorOpenProgress;
        private float _completionTimer;
        private Renderer _corridorRenderer;
        private MaterialPropertyBlock _corridorPropertyBlock;
        private Vector3 _jumpStartPosition;
        private Vector3 _jumpTargetPosition;
        private float _jumpProgress;

        public event Action Completed;

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
            _corridorRenderer = _scene.Corridor.GetComponent<Renderer>();
            _corridorPropertyBlock = new MaterialPropertyBlock();

            foreach (var obstacle in _scene.Obstacles)
            {
                if (obstacle == null)
                {
                    continue;
                }

                obstacle.Destroyed += ShowObstacleDestroyed;
            }

            ResetGame();
        }

        public void Dispose()
        {
            _energy.Changed -= UpdateEnergyView;
            _scene.Ui.RestartClicked -= ResetGame;

            foreach (var obstacle in _scene.Obstacles)
            {
                if (obstacle == null)
                {
                    continue;
                }

                obstacle.Destroyed -= ShowObstacleDestroyed;
            }

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

            if (_phase == GamePhase.Won)
            {
                UpdateCompletion(deltaTime);
            }

            UpdateDoor(deltaTime);
            UpdateImpactShake(deltaTime);
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
            _impactShakeTime = 0f;
            _pendingObstacleDestructionCount = 0;
            _isDoorOpen = false;
            _doorOpenProgress = 0f;
            _completionTimer = 0f;
            _jumpProgress = 1f;
            _scene.Player.SetPosition(_scene.PlayerSpawnPosition);
            UpdatePlayerRadius();
            UpdateLevelProgress();
            SnapCameraToPlayer();
            ApplyDoorPose();
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
            var projectedRadius = GetProjectedPlayerRadius();
            _scene.Player.SetChargeFeedback(_shotCharge.NormalizedCharge, projectedRadius);
            _scene.CameraView.SetShake(_shotCharge.NormalizedCharge);
            UpdateCorridor(projectedRadius);
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
            _scene.ChargePreview.position = _scene.Player.Position + Vector3.forward * (GetProjectedPlayerRadius() + radius + 0.25f);
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

        private void ResolveProjectileHit(Obstacle obstacle, float projectileRadius, Vector3 projectilePosition)
        {
            _impactShakeTime = 0.25f;
            _transientViewFactory.ShowProjectileBurst(projectilePosition, projectileRadius);
            var result = _obstacleClearing.ClearFromImpact(_scene.Obstacles, obstacle, projectilePosition, projectileRadius);
            _transientViewFactory.ShowInfectionRadius(result.ImpactPosition, result.InfectionRadius);
            _pendingObstacleDestructionCount = result.ClearedCount;

            if (_pendingObstacleDestructionCount > 0)
            {
                return;
            }

            ResolvePathAfterShot();
        }

        private void ResolveProjectileMiss(Vector3 projectilePosition, float projectileRadius)
        {
            _transientViewFactory.ShowProjectileBurst(projectilePosition, projectileRadius);
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
                StartNextPlayerJump();
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
            var jumpDistance = GetFlatDistance(_jumpStartPosition, _jumpTargetPosition);
            _jumpProgress = jumpDistance <= Mathf.Epsilon
                ? 1f
                : Mathf.Min(1f, _jumpProgress + _configuration.PlayerMoveSpeed * deltaTime / jumpDistance);

            var easedProgress = Mathf.SmoothStep(0f, 1f, _jumpProgress);
            var position = Vector3.Lerp(_jumpStartPosition, _jumpTargetPosition, easedProgress);
            position.y += Mathf.Sin(_jumpProgress * Mathf.PI) * _configuration.PlayerJumpHeight;
            _scene.Player.SetPosition(position);

            if (_jumpProgress >= 1f)
            {
                _scene.Player.SetPosition(_jumpTargetPosition);

                if (GetFlatDistance(_jumpTargetPosition, _moveTarget) > 0.02f)
                {
                    StartNextPlayerJump();
                }
            }

            UpdateCorridor();
            UpdateLevelProgress();
            UpdateCameraFollow(deltaTime);

            var distanceToDoor = GetFlatDistance(_scene.Player.Position, _scene.DoorPosition);

            if (distanceToDoor <= _configuration.DoorOpenDistance)
            {
                SetDoorOpen(true);
            }

            if (distanceToDoor <= _configuration.LevelCompleteDistance)
            {
                Win();
                return;
            }

            if (GetFlatDistance(_scene.Player.Position, _moveTarget) <= 0.02f)
            {
                if (ShouldLoseWhenBlocked())
                {
                    Lose();
                    return;
                }

                _phase = GamePhase.Ready;
            }
        }

        private void StartNextPlayerJump()
        {
            var current = GetGroundedPlayerPosition();
            var target = new Vector3(_moveTarget.x, current.y, _moveTarget.z);
            var remaining = GetFlatDistance(current, target);

            if (remaining <= 0.02f)
            {
                _jumpStartPosition = current;
                _jumpTargetPosition = target;
                _jumpProgress = 1f;
                return;
            }

            var direction = GetFlatDirection(current, target);
            var distance = Mathf.Min(_configuration.PlayerJumpDistance, remaining);
            _jumpStartPosition = current;
            _jumpTargetPosition = current + direction * distance;
            _jumpProgress = 0f;
        }

        private void Win()
        {
            _phase = GamePhase.Won;
            _scene.Ui.SetLevelProgressValue(1f);
            SetDoorOpen(true);
            _completionTimer = CompletionDelay;
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

        private void UpdateImpactShake(float deltaTime)
        {
            if (_impactShakeTime <= 0f)
            {
                return;
            }

            _impactShakeTime = Mathf.Max(0f, _impactShakeTime - deltaTime);
            _scene.CameraView.SetShake(_impactShakeTime / 0.25f);
        }

        private void UpdateCompletion(float deltaTime)
        {
            _completionTimer -= deltaTime;

            if (_completionTimer > 0f)
            {
                return;
            }

            Completed?.Invoke();
        }

        private void UpdateCameraFollow(float deltaTime)
        {
            _scene.CameraView.Follow(GetGroundedPlayerPosition(), deltaTime);
        }

        private void SnapCameraToPlayer()
        {
            _scene.CameraView.SnapToFollowTarget(_scene.Player.Position);
        }

        private void SetDoorOpen(bool isOpen)
        {
            _isDoorOpen = isOpen;
        }

        private void UpdateDoor(float deltaTime)
        {
            var targetProgress = _isDoorOpen ? 1f : 0f;
            _doorOpenProgress = Mathf.MoveTowards(_doorOpenProgress, targetProgress, deltaTime * 4f);
            ApplyDoorPose();
        }

        private void ApplyDoorPose()
        {
            var easedProgress = Mathf.SmoothStep(0f, 1f, _doorOpenProgress);
            _scene.DoorLeftPanel.localPosition = new Vector3(Mathf.Lerp(-0.45f, -0.9f, easedProgress), 0f, 0f);
            _scene.DoorRightPanel.localPosition = new Vector3(Mathf.Lerp(0.45f, 0.9f, easedProgress), 0f, 0f);
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
            UpdateCorridor(radius);
        }

        private float GetProjectedPlayerRadius()
        {
            return _playerSize.GetRadius(_energy.GetNormalizedAfterSpend(_shotCharge.EnergyCost));
        }

        private void UpdateLevelProgress()
        {
            var spawnPosition = new Vector3(_scene.PlayerSpawnPosition.x, 0f, _scene.PlayerSpawnPosition.z);
            var doorPosition = new Vector3(_scene.DoorPosition.x, 0f, _scene.DoorPosition.z);
            var playerPosition = new Vector3(_scene.Player.Position.x, 0f, _scene.Player.Position.z);
            var route = doorPosition - spawnPosition;
            var routeLength = route.magnitude;

            if (routeLength <= Mathf.Epsilon)
            {
                _scene.Ui.SetLevelProgressValue(1f);
                return;
            }

            var traveled = Vector3.Dot(playerPosition - spawnPosition, route.normalized);
            _scene.Ui.SetLevelProgressValue(traveled / routeLength);
        }

        private Vector3 GetGroundedPlayerPosition()
        {
            return new Vector3(_scene.Player.Position.x, _scene.PlayerSpawnPosition.y, _scene.Player.Position.z);
        }

        private static float GetFlatDistance(Vector3 first, Vector3 second)
        {
            return Vector3.Distance(new Vector3(first.x, 0f, first.z), new Vector3(second.x, 0f, second.z));
        }

        private static Vector3 GetFlatDirection(Vector3 start, Vector3 target)
        {
            var direction = new Vector3(target.x - start.x, 0f, target.z - start.z);
            return direction.sqrMagnitude <= Mathf.Epsilon ? Vector3.zero : direction.normalized;
        }

        private void ShowObstacleDestroyed(Obstacle obstacle)
        {
            _transientViewFactory.ShowObstacleBurst(obstacle.Position, obstacle.Radius);

            if (_pendingObstacleDestructionCount <= 0)
            {
                return;
            }

            _pendingObstacleDestructionCount--;

            if (_pendingObstacleDestructionCount > 0 || _phase != GamePhase.ProjectileFlying)
            {
                return;
            }

            ResolvePathAfterShot();
        }

        private void UpdateCorridor()
        {
            UpdateCorridor(_scene.Player.Radius);
        }

        private void UpdateCorridor(float playerRadius)
        {
            var start = _scene.Player.Position;
            var target = _scene.DoorPosition;
            var startFlat = new Vector3(start.x, 0f, start.z);
            var targetFlat = new Vector3(target.x, 0f, target.z);
            var midpoint = (startFlat + targetFlat) * 0.5f;
            var length = Vector3.Distance(startFlat, targetFlat);
            var pathWidth = playerRadius * 2f;
            var visualWidth = pathWidth + CorridorVisualPadding * 2f;
            var pathMinX = CorridorVisualPadding / visualWidth;
            _scene.Corridor.position = new Vector3(midpoint.x, 0.02f, midpoint.z);
            _scene.Corridor.localScale = new Vector3(visualWidth, 0.04f, length);

            if (_corridorRenderer == null)
            {
                return;
            }

            _corridorRenderer.GetPropertyBlock(_corridorPropertyBlock);
            _corridorPropertyBlock.SetFloat(PathMinXProperty, pathMinX);
            _corridorPropertyBlock.SetFloat(PathMaxXProperty, 1f - pathMinX);
            _corridorRenderer.SetPropertyBlock(_corridorPropertyBlock);
        }
    }
}
