using System.Collections.Generic;
using SpherePath.Cameras;
using SpherePath.Input;
using SpherePath.Obstacles;
using SpherePath.Pathing;
using SpherePath.Player;
using SpherePath.Shooting;
using SpherePath.VFX;
using UnityEngine;
using UnityEngine.UI;

namespace SpherePath.GameState
{
    public sealed class PrototypeGameController : MonoBehaviour
    {
        [SerializeField] private float maximumEnergy = 10f;
        [SerializeField] private float minimumEnergy = 1.2f;
        [SerializeField] private float minimumPlayerRadius = 0.35f;
        [SerializeField] private float maximumPlayerRadius = 1.1f;
        [SerializeField] private float maxChargeTime = 1.6f;
        [SerializeField] private float minimumProjectileRadius = 0.25f;
        [SerializeField] private float maximumProjectileRadius = 1.25f;
        [SerializeField] private float minimumShotCost = 0.65f;
        [SerializeField] private float maximumShotCost = 3.1f;
        [SerializeField] private float projectileSpeed = 18f;
        [SerializeField] private float projectileLifeTime = 4f;
        [SerializeField] private float infectionRadiusMultiplier = 2.25f;
        [SerializeField] private float playerMoveSpeed = 7f;
        [SerializeField] private float cameraFollowSpeed = 6f;
        [SerializeField] private float doorOpenDistance = 5f;

        private readonly List<PrototypeObstacle> _obstacles = new List<PrototypeObstacle>();
        private readonly List<GameObject> _runtimeObjects = new List<GameObject>();

        private PlayerEnergy _energy;
        private PlayerSizeService _playerSize;
        private PlayerViabilityService _playerViability;
        private ShotChargeService _shotCharge;
        private PointerChargeInput _input;
        private ObstacleClearingService _obstacleClearing;
        private ObstacleFieldLayout _obstacleLayout;
        private PathClearanceService _pathClearance;
        private PrototypePlayerView _player;
        private Transform _door;
        private Transform _doorLeftPanel;
        private Transform _doorRightPanel;
        private Transform _corridor;
        private Transform _chargePreview;
        private PrototypeCameraView _cameraView;
        private Slider _energySlider;
        private Text _statusText;
        private GamePhase _phase;
        private Vector3 _playerStartPosition;
        private Vector3 _doorPosition;
        private Vector3 _moveTarget;
        private Material _playerMaterial;
        private Material _projectileMaterial;
        private Material _groundMaterial;
        private Material _obstacleMaterial;
        private Material _doorMaterial;
        private Material _corridorMaterial;
        private Material _infectionPreviewMaterial;

        private void Awake()
        {
            _playerStartPosition = new Vector3(0f, maximumPlayerRadius, -12f);
            _doorPosition = new Vector3(0f, 1.5f, 16f);
            _energy = new PlayerEnergy(maximumEnergy, minimumEnergy);
            _playerSize = new PlayerSizeService(minimumPlayerRadius, maximumPlayerRadius);
            _playerViability = new PlayerViabilityService(_energy);
            var chargeMeter = new ChargeMeter(maxChargeTime, minimumProjectileRadius, maximumProjectileRadius, minimumShotCost, maximumShotCost);
            _shotCharge = new ShotChargeService(_energy, chargeMeter);
            _input = new PointerChargeInput();
            _obstacleClearing = new ObstacleClearingService(infectionRadiusMultiplier);
            _obstacleLayout = new ObstacleFieldLayout();
            CreateMaterials();
            BuildScene();
            _pathClearance = new PathClearanceService(_obstacles, 0.2f);
            _energy.Changed += UpdateEnergyView;
            ResetGame();
        }

        private void OnDestroy()
        {
            if (_energy != null)
            {
                _energy.Changed -= UpdateEnergyView;
            }
        }

        private void Update()
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

            if (_phase == GamePhase.Charging)
            {
                UpdateCharging();
            }

            if (_phase == GamePhase.Moving)
            {
                MovePlayer();
            }
        }

        private void CreateMaterials()
        {
            _playerMaterial = CreateMaterial(new Color(1f, 0.58f, 0.12f, 1f));
            _projectileMaterial = CreateMaterial(new Color(1f, 0.78f, 0.25f, 1f));
            _groundMaterial = CreateMaterial(new Color(0.48f, 0.6f, 0.42f, 1f));
            _obstacleMaterial = CreateMaterial(new Color(0.28f, 0.63f, 0.24f, 1f));
            _doorMaterial = CreateMaterial(new Color(1f, 0.56f, 0.16f, 1f));
            _corridorMaterial = CreateMaterial(new Color(1f, 0.18f, 0.4f, 0.35f));
            _infectionPreviewMaterial = CreateMaterial(new Color(1f, 0.35f, 0.12f, 0.28f));
        }

        private Material CreateMaterial(Color color)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            var material = new Material(shader != null ? shader : Shader.Find("Standard"));
            material.color = color;
            return material;
        }

        private void BuildScene()
        {
            ConfigureCamera();
            ConfigureLight();
            CreateGround();
            CreateCorridor();
            CreateDoor();
            CreatePlayer();
            CreateChargePreview();
            CreateObstacles();
            CreateUi();
        }

        private void ConfigureCamera()
        {
            var mainCamera = Camera.main;
            if (mainCamera == null)
            {
                var cameraObject = new GameObject("Main Camera");
                mainCamera = cameraObject.AddComponent<Camera>();
                cameraObject.AddComponent<AudioListener>();
                cameraObject.tag = "MainCamera";
            }

            mainCamera.transform.SetPositionAndRotation(new Vector3(0f, 17f, -20f), Quaternion.Euler(56f, 0f, 0f));
            mainCamera.fieldOfView = 42f;
            mainCamera.clearFlags = CameraClearFlags.Skybox;
            _cameraView = mainCamera.GetComponent<PrototypeCameraView>();
            if (_cameraView == null)
            {
                _cameraView = mainCamera.gameObject.AddComponent<PrototypeCameraView>();
            }

            _cameraView.CaptureBasePose();
            _cameraView.SetFollowSettings(new Vector3(0f, 17f, -8f), cameraFollowSpeed);
        }

        private void ConfigureLight()
        {
            var light = FindFirstObjectByType<Light>();
            if (light == null)
            {
                var lightObject = new GameObject("Directional Light");
                light = lightObject.AddComponent<Light>();
                light.type = LightType.Directional;
            }

            light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            light.intensity = 2.4f;
        }

        private void CreateGround()
        {
            var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "Prototype Ground";
            ground.transform.position = new Vector3(0f, -0.05f, 2f);
            ground.transform.localScale = new Vector3(11f, 0.1f, 34f);
            ground.GetComponent<Renderer>().sharedMaterial = _groundMaterial;
            _runtimeObjects.Add(ground);
        }

        private void CreateCorridor()
        {
            var corridorObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            corridorObject.name = "Path Width Indicator";
            Destroy(corridorObject.GetComponent<Collider>());
            _corridor = corridorObject.transform;
            _corridor.GetComponent<Renderer>().sharedMaterial = _corridorMaterial;
            _runtimeObjects.Add(corridorObject);
        }

        private void CreateDoor()
        {
            _door = new GameObject("Goal Door").transform;
            _door.position = _doorPosition;
            _runtimeObjects.Add(_door.gameObject);

            _doorLeftPanel = CreateDoorPanel("Left Door Panel", new Vector3(-0.45f, 0f, 0f));
            _doorRightPanel = CreateDoorPanel("Right Door Panel", new Vector3(0.45f, 0f, 0f));
        }

        private Transform CreateDoorPanel(string objectName, Vector3 localPosition)
        {
            var panel = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            panel.name = objectName;
            panel.SetParent(_door);
            panel.localPosition = localPosition;
            panel.localScale = new Vector3(0.8f, 3f, 0.25f);
            panel.GetComponent<Renderer>().sharedMaterial = _doorMaterial;
            _runtimeObjects.Add(panel.gameObject);
            return panel;
        }

        private void CreatePlayer()
        {
            var playerObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            playerObject.name = "Player Sphere";
            playerObject.GetComponent<Renderer>().sharedMaterial = _playerMaterial;
            _player = playerObject.AddComponent<PrototypePlayerView>();
            _runtimeObjects.Add(playerObject);
        }

        private void CreateChargePreview()
        {
            var previewObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            previewObject.name = "Projectile Preview";
            Destroy(previewObject.GetComponent<Collider>());
            previewObject.GetComponent<Renderer>().sharedMaterial = _projectileMaterial;
            _chargePreview = previewObject.transform;
            _runtimeObjects.Add(previewObject);
        }

        private void CreateObstacles()
        {
            foreach (var position in _obstacleLayout.Positions)
            {
                CreateObstacle(position);
            }
        }

        private void CreateObstacle(Vector3 position)
        {
            var obstacleObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            obstacleObject.name = "Prototype Obstacle";
            obstacleObject.transform.position = position;
            obstacleObject.transform.localScale = new Vector3(1f, 1.2f, 1f);
            obstacleObject.GetComponent<Renderer>().sharedMaterial = _obstacleMaterial;
            var obstacle = obstacleObject.AddComponent<PrototypeObstacle>();
            _obstacles.Add(obstacle);
            _runtimeObjects.Add(obstacleObject);
        }

        private void CreateUi()
        {
            var canvasObject = new GameObject("Prototype UI");
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
            _runtimeObjects.Add(canvasObject);

            var sliderObject = new GameObject("Energy Slider");
            sliderObject.transform.SetParent(canvasObject.transform, false);
            _energySlider = sliderObject.AddComponent<Slider>();
            var sliderRect = sliderObject.GetComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0.15f, 0.04f);
            sliderRect.anchorMax = new Vector2(0.85f, 0.08f);
            sliderRect.offsetMin = Vector2.zero;
            sliderRect.offsetMax = Vector2.zero;
            _energySlider.minValue = 0f;
            _energySlider.maxValue = 1f;
            var background = CreateUiImage("Background", sliderObject.transform, new Color(0f, 0f, 0f, 0.45f));
            var fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(sliderObject.transform, false);
            var fillAreaRect = fillArea.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.offsetMin = Vector2.zero;
            fillAreaRect.offsetMax = Vector2.zero;
            var fill = CreateUiImage("Fill", fillArea.transform, new Color(1f, 0.65f, 0.12f, 1f));
            _energySlider.targetGraphic = background;
            _energySlider.fillRect = fill.rectTransform;

            var statusObject = new GameObject("Status Text");
            statusObject.transform.SetParent(canvasObject.transform, false);
            _statusText = statusObject.AddComponent<Text>();
            _statusText.alignment = TextAnchor.MiddleCenter;
            _statusText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            _statusText.fontSize = 36;
            _statusText.color = Color.white;
            var textRect = statusObject.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.1f, 0.84f);
            textRect.anchorMax = new Vector2(0.9f, 0.96f);
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
        }

        private Image CreateUiImage(string objectName, Transform parent, Color color)
        {
            var imageObject = new GameObject(objectName);
            imageObject.transform.SetParent(parent, false);
            var image = imageObject.AddComponent<Image>();
            image.color = color;
            var rect = image.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return image;
        }

        private void ResetGame()
        {
            foreach (var obstacle in _obstacles)
            {
                obstacle.Restore();
            }

            _energy.Reset();
            _shotCharge.Begin();
            _phase = GamePhase.Ready;
            _player.SetPosition(_playerStartPosition);
            UpdatePlayerRadius();
            SnapCameraToPlayer();
            SetDoorOpen(false);
            SetStatusText(string.Empty);
            _chargePreview.gameObject.SetActive(false);
            ResetCameraShake();
            UpdateCorridor();
        }

        private void StartCharging()
        {
            _phase = GamePhase.Charging;
            _shotCharge.Begin();
            _chargePreview.gameObject.SetActive(true);
        }

        private void UpdateCharging()
        {
            _shotCharge.Tick(Time.deltaTime);
            _player.SetChargeFeedback(_shotCharge.NormalizedCharge);
            if (_cameraView != null)
            {
                _cameraView.SetShake(_shotCharge.NormalizedCharge);
            }

            UpdateChargePreview();

            if (_input.ReleasedThisFrame)
            {
                FireChargedShot();
            }
        }

        private void UpdateChargePreview()
        {
            var radius = _shotCharge.ProjectileRadius;
            _chargePreview.position = _player.Position + Vector3.forward * (_player.Radius + radius + 0.25f);
            _chargePreview.localScale = Vector3.one * (radius * 2f);
        }

        private void FireChargedShot()
        {
            var shot = _shotCharge.CreateShot();
            _chargePreview.gameObject.SetActive(false);
            ResetCameraShake();

            if (!_shotCharge.TrySpend(shot))
            {
                Lose();
                return;
            }

            UpdatePlayerRadius();
            SpawnProjectile(shot.ProjectileRadius);
            _phase = GamePhase.ProjectileFlying;
        }

        private void SpawnProjectile(float radius)
        {
            var projectileObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            projectileObject.name = "Projectile";
            projectileObject.transform.position = _player.Position + Vector3.forward * (_player.Radius + radius + 0.25f);
            projectileObject.GetComponent<Renderer>().sharedMaterial = _projectileMaterial;
            Destroy(projectileObject.GetComponent<Collider>());
            var projectile = projectileObject.AddComponent<PrototypeProjectile>();
            projectile.HitObstacle += ResolveProjectileHit;
            projectile.Expired += ResolveProjectileMiss;
            projectile.Launch(_obstacles, Vector3.forward, radius, projectileSpeed, projectileLifeTime);
        }

        private void ResolveProjectileHit(PrototypeObstacle obstacle, float projectileRadius)
        {
            var result = _obstacleClearing.ClearFromImpact(_obstacles, obstacle, projectileRadius);
            ShowInfectionRadius(obstacle.Position, result.InfectionRadius);
            ResolvePathAfterShot();
        }

        private void ShowInfectionRadius(Vector3 center, float radius)
        {
            if (radius <= 0f)
            {
                return;
            }

            var previewObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            previewObject.name = "Infection Radius Preview";
            previewObject.transform.position = new Vector3(center.x, 0.04f, center.z);
            previewObject.transform.localScale = new Vector3(radius * 2f, 0.02f, radius * 2f);
            Destroy(previewObject.GetComponent<Collider>());
            previewObject.GetComponent<Renderer>().sharedMaterial = _infectionPreviewMaterial;
            previewObject.AddComponent<TimedSelfDestroy>().SetLifeTime(0.35f);
        }

        private void ResolveProjectileMiss()
        {
            ResolvePathAfterShot();
        }

        private void ResolvePathAfterShot()
        {
            _moveTarget = _pathClearance.GetReachablePosition(_player.Position, _doorPosition, _player.Radius);
            var movedDistance = Vector3.Distance(_player.Position, _moveTarget);

            if (movedDistance > 0.05f)
            {
                _phase = GamePhase.Moving;
                return;
            }

            if (ShouldLoseWhenBlocked())
            {
                Lose();
                return;
            }

            _phase = GamePhase.Ready;
        }

        private void MovePlayer()
        {
            _player.SetPosition(Vector3.MoveTowards(_player.Position, _moveTarget, playerMoveSpeed * Time.deltaTime));
            UpdateCorridor();
            UpdateCameraFollow();

            if (Vector3.Distance(_player.Position, _doorPosition) <= doorOpenDistance)
            {
                SetDoorOpen(true);
            }

            if (Vector3.Distance(_player.Position, _moveTarget) <= 0.02f)
            {
                if (Vector3.Distance(_player.Position, _doorPosition) <= 0.4f)
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
            SetStatusText("WIN");
        }

        private void Lose()
        {
            _phase = GamePhase.Lost;
            ResetCameraShake();
            SetStatusText("LOSE");
        }

        private bool ShouldLoseWhenBlocked()
        {
            return _playerViability.HasCriticalEnergy || !_shotCharge.CanAffordMinimumShot;
        }

        private void ResetCameraShake()
        {
            if (_cameraView != null)
            {
                _cameraView.ResetShake();
            }
        }

        private void UpdateCameraFollow()
        {
            if (_cameraView != null)
            {
                _cameraView.Follow(_player.Position, Time.deltaTime);
            }
        }

        private void SnapCameraToPlayer()
        {
            if (_cameraView != null)
            {
                _cameraView.SnapToFollowTarget(_player.Position);
            }
        }

        private void SetStatusText(string value)
        {
            if (_statusText != null)
            {
                _statusText.text = value;
            }
        }

        private void SetDoorOpen(bool isOpen)
        {
            if (_doorLeftPanel == null || _doorRightPanel == null)
            {
                return;
            }

            _doorLeftPanel.localPosition = isOpen ? new Vector3(-0.9f, 0f, 0f) : new Vector3(-0.45f, 0f, 0f);
            _doorRightPanel.localPosition = isOpen ? new Vector3(0.9f, 0f, 0f) : new Vector3(0.45f, 0f, 0f);
        }

        private void UpdateEnergyView(float normalizedEnergy)
        {
            if (_energySlider != null)
            {
                _energySlider.value = normalizedEnergy;
            }
        }

        private void UpdatePlayerRadius()
        {
            var radius = _playerSize.GetRadius(_energy.Normalized);
            _player.SetRadius(radius);
            UpdateCorridor();
        }

        private void UpdateCorridor()
        {
            if (_corridor == null || _player == null)
            {
                return;
            }

            var start = _player.Position;
            var target = _doorPosition;
            var midpoint = (start + target) * 0.5f;
            var length = Vector3.Distance(start, target);
            _corridor.position = new Vector3(midpoint.x, 0.02f, midpoint.z);
            _corridor.localScale = new Vector3(_player.Radius * 2f, 0.04f, length);
        }
    }
}
