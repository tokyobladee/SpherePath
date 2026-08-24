using System.Collections.Generic;
using SpherePath.Cameras;
using SpherePath.Configuration;
using SpherePath.Obstacles;
using SpherePath.Player;
using SpherePath.Shooting;
using SpherePath.UI;
using SpherePath.VFX;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace SpherePath.Level
{
    public sealed class LevelViewFactory
    {
        private readonly GameplayConfiguration _configuration;
        private readonly ObstacleFieldLayout _obstacleLayout;
        private readonly List<Obstacle> _obstacles = new List<Obstacle>();
        private readonly List<GameObject> _transientObjects = new List<GameObject>();

        private Material _playerMaterial;
        private Material _projectileMaterial;
        private Material _groundMaterial;
        private Material _obstacleMaterial;
        private Material _doorMaterial;
        private Material _corridorMaterial;
        private Material _infectionPreviewMaterial;
        private Material _trailMaterial;

        public LevelViewFactory(GameplayConfiguration configuration, ObstacleFieldLayout obstacleLayout)
        {
            _configuration = configuration;
            _obstacleLayout = obstacleLayout;
        }

        public LevelViewReferences Build()
        {
            CreateMaterials();
            var cameraView = ConfigureCamera();
            ConfigureLight();
            CreateGround();
            var corridor = CreateCorridor();
            var door = CreateDoor(out var leftDoorPanel, out var rightDoorPanel);
            var player = CreatePlayer();
            var chargePreview = CreateChargePreview();
            CreateObstacles();
            var ui = CreateUi();

            return new LevelViewReferences(
                _obstacles,
                player,
                door,
                leftDoorPanel,
                rightDoorPanel,
                corridor,
                chargePreview,
                cameraView,
                ui);
        }

        public Projectile CreateProjectile(Vector3 position, float radius)
        {
            var projectileObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            projectileObject.name = "Projectile";
            projectileObject.transform.position = position;
            projectileObject.GetComponent<Renderer>().sharedMaterial = _projectileMaterial;
            Object.Destroy(projectileObject.GetComponent<Collider>());
            CreateProjectileTrail(projectileObject, radius);
            var projectile = projectileObject.AddComponent<Projectile>();
            projectile.Launch(_obstacles, Vector3.forward, radius, _configuration.ProjectileSpeed, _configuration.ProjectileLifeTime);
            _transientObjects.Add(projectileObject);
            return projectile;
        }

        public void ClearTransients()
        {
            for (var i = _transientObjects.Count - 1; i >= 0; i--)
            {
                var transientObject = _transientObjects[i];
                _transientObjects.RemoveAt(i);

                if (transientObject == null)
                {
                    continue;
                }

                var projectile = transientObject.GetComponent<Projectile>();
                if (projectile != null)
                {
                    projectile.Cancel();
                    continue;
                }

                Object.Destroy(transientObject);
            }
        }

        public void ShowInfectionRadius(Vector3 center, float radius)
        {
            if (radius <= 0f)
            {
                return;
            }

            var previewObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            previewObject.name = "Infection Radius Preview";
            previewObject.transform.position = new Vector3(center.x, 0.04f, center.z);
            previewObject.transform.localScale = new Vector3(radius * 2f, 0.02f, radius * 2f);
            Object.Destroy(previewObject.GetComponent<Collider>());
            previewObject.GetComponent<Renderer>().sharedMaterial = _infectionPreviewMaterial;
            previewObject.AddComponent<TimedSelfDestroy>().SetLifeTime(0.35f);
            _transientObjects.Add(previewObject);
            CreateImpactBurst(center, radius);
        }

        private void CreateMaterials()
        {
            _playerMaterial = CreateMaterial(new Color(1f, 0.58f, 0.12f, 1f));
            _projectileMaterial = CreateMaterial(new Color(1f, 0.78f, 0.25f, 1f));
            _groundMaterial = CreateMaterial(new Color(0.54f, 0.64f, 0.47f, 1f));
            _obstacleMaterial = CreateMaterial(new Color(0.24f, 0.58f, 0.28f, 1f));
            _doorMaterial = CreateMaterial(new Color(1f, 0.56f, 0.16f, 1f));
            _corridorMaterial = CreateMaterial(new Color(1f, 0.18f, 0.4f, 0.35f));
            _infectionPreviewMaterial = CreateMaterial(new Color(1f, 0.35f, 0.12f, 0.28f));
            _trailMaterial = CreateMaterial(new Color(1f, 0.7f, 0.18f, 0.65f));
        }

        private Material CreateMaterial(Color color)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            var material = new Material(shader != null ? shader : Shader.Find("Standard"));
            material.color = color;
            return material;
        }

        private FollowCameraView ConfigureCamera()
        {
            var mainCamera = Camera.main;
            if (mainCamera == null)
            {
                var cameraObject = new GameObject("Main Camera");
                mainCamera = cameraObject.AddComponent<Camera>();
                cameraObject.AddComponent<AudioListener>();
                cameraObject.tag = "MainCamera";
            }

            mainCamera.transform.SetPositionAndRotation(new Vector3(0f, 18.5f, -17f), Quaternion.Euler(58f, 0f, 0f));
            mainCamera.fieldOfView = 37f;
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = new Color(0.18f, 0.2f, 0.22f, 1f);
            var cameraView = mainCamera.GetComponent<FollowCameraView>();
            if (cameraView == null)
            {
                cameraView = mainCamera.gameObject.AddComponent<FollowCameraView>();
            }

            cameraView.CaptureBasePose();
            cameraView.SetFollowSettings(new Vector3(0f, 18.5f, -8.5f), _configuration.CameraFollowSpeed);
            return cameraView;
        }

        private void ConfigureLight()
        {
            var light = Object.FindAnyObjectByType<Light>();
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
            ground.name = "Ground";
            ground.transform.position = new Vector3(0f, -0.05f, 0f);
            ground.transform.localScale = new Vector3(14f, 0.1f, 46f);
            ground.GetComponent<Renderer>().sharedMaterial = _groundMaterial;
        }

        private Transform CreateCorridor()
        {
            var corridorObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            corridorObject.name = "Path Width Indicator";
            Object.Destroy(corridorObject.GetComponent<Collider>());
            corridorObject.GetComponent<Renderer>().sharedMaterial = _corridorMaterial;
            return corridorObject.transform;
        }

        private Transform CreateDoor(out Transform leftDoorPanel, out Transform rightDoorPanel)
        {
            var door = new GameObject("Goal Door").transform;
            door.position = _configuration.DoorPosition;
            leftDoorPanel = CreateDoorPanel(door, "Left Door Panel", new Vector3(-0.45f, 0f, 0f));
            rightDoorPanel = CreateDoorPanel(door, "Right Door Panel", new Vector3(0.45f, 0f, 0f));
            return door;
        }

        private Transform CreateDoorPanel(Transform door, string objectName, Vector3 localPosition)
        {
            var panel = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            panel.name = objectName;
            panel.SetParent(door);
            panel.localPosition = localPosition;
            panel.localScale = new Vector3(0.8f, 3f, 0.25f);
            panel.GetComponent<Renderer>().sharedMaterial = _doorMaterial;
            return panel;
        }

        private PlayerView CreatePlayer()
        {
            var playerObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            playerObject.name = "Player Sphere";
            playerObject.GetComponent<Renderer>().sharedMaterial = _playerMaterial;
            return playerObject.AddComponent<PlayerView>();
        }

        private Transform CreateChargePreview()
        {
            var previewObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            previewObject.name = "Projectile Preview";
            Object.Destroy(previewObject.GetComponent<Collider>());
            previewObject.GetComponent<Renderer>().sharedMaterial = _projectileMaterial;
            return previewObject.transform;
        }

        private void CreateObstacles()
        {
            foreach (var position in _obstacleLayout.Positions)
            {
                var obstacleObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                obstacleObject.name = "Obstacle";
                obstacleObject.transform.position = position;
                obstacleObject.transform.localScale = new Vector3(1f, 1.2f, 1f);
                obstacleObject.GetComponent<Renderer>().sharedMaterial = _obstacleMaterial;
                _obstacles.Add(obstacleObject.AddComponent<Obstacle>());
            }
        }

        private GameUiView CreateUi()
        {
            EnsureEventSystem();
            var canvasObject = new GameObject("Game UI");
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(360f, 640f);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObject.AddComponent<GraphicRaycaster>();

            var safeAreaObject = new GameObject("Safe Area");
            safeAreaObject.transform.SetParent(canvasObject.transform, false);
            var safeArea = safeAreaObject.AddComponent<RectTransform>();
            safeArea.anchorMin = Vector2.zero;
            safeArea.anchorMax = Vector2.one;
            safeArea.offsetMin = Vector2.zero;
            safeArea.offsetMax = Vector2.zero;

            var sliderObject = new GameObject("Energy Slider");
            sliderObject.transform.SetParent(safeAreaObject.transform, false);
            var energySlider = sliderObject.AddComponent<Slider>();
            var sliderRect = sliderObject.GetComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0.18f, 0.035f);
            sliderRect.anchorMax = new Vector2(0.82f, 0.07f);
            sliderRect.offsetMin = Vector2.zero;
            sliderRect.offsetMax = Vector2.zero;
            energySlider.minValue = 0f;
            energySlider.maxValue = 1f;
            var background = CreateUiImage("Background", sliderObject.transform, new Color(0f, 0f, 0f, 0.45f));
            var fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(sliderObject.transform, false);
            var fillAreaRect = fillArea.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.offsetMin = Vector2.zero;
            fillAreaRect.offsetMax = Vector2.zero;
            var fill = CreateUiImage("Fill", fillArea.transform, new Color(1f, 0.65f, 0.12f, 1f));
            energySlider.targetGraphic = background;
            energySlider.fillRect = fill.rectTransform;

            var statusObject = new GameObject("Status Text");
            statusObject.transform.SetParent(safeAreaObject.transform, false);
            var statusText = statusObject.AddComponent<Text>();
            statusText.alignment = TextAnchor.MiddleCenter;
            statusText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            statusText.fontSize = 36;
            statusText.color = Color.white;
            var textRect = statusObject.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.08f, 0.82f);
            textRect.anchorMax = new Vector2(0.92f, 0.94f);
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            var resultPanel = CreateResultPanel(safeAreaObject.transform, out var resultHintText, out var restartButton);
            return new GameUiView(energySlider, statusText, resultPanel, resultHintText, restartButton, safeArea);
        }

        private void EnsureEventSystem()
        {
            if (Object.FindAnyObjectByType<EventSystem>() != null)
            {
                return;
            }

            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<InputSystemUIInputModule>();
        }

        private GameObject CreateResultPanel(Transform parent, out Text hintText, out Button restartButton)
        {
            var panel = new GameObject("Result Panel");
            panel.transform.SetParent(parent, false);
            var image = panel.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0.42f);
            var rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            hintText = CreateText("Result Hint", panel.transform, 24, new Color(1f, 1f, 1f, 0.9f));
            var hintRect = hintText.GetComponent<RectTransform>();
            hintRect.anchorMin = new Vector2(0.08f, 0.42f);
            hintRect.anchorMax = new Vector2(0.92f, 0.5f);
            hintRect.offsetMin = Vector2.zero;
            hintRect.offsetMax = Vector2.zero;

            var buttonObject = new GameObject("Restart Button");
            buttonObject.transform.SetParent(panel.transform, false);
            var buttonImage = buttonObject.AddComponent<Image>();
            buttonImage.color = new Color(1f, 0.62f, 0.12f, 1f);
            restartButton = buttonObject.AddComponent<Button>();
            restartButton.targetGraphic = buttonImage;
            var buttonRect = buttonObject.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.28f, 0.28f);
            buttonRect.anchorMax = new Vector2(0.72f, 0.36f);
            buttonRect.offsetMin = Vector2.zero;
            buttonRect.offsetMax = Vector2.zero;

            var buttonText = CreateText("Restart Text", buttonObject.transform, 24, Color.white);
            buttonText.text = "RESTART";
            buttonText.fontStyle = FontStyle.Bold;

            panel.SetActive(false);
            return panel;
        }

        private Text CreateText(string objectName, Transform parent, int fontSize, Color color)
        {
            var textObject = new GameObject(objectName);
            textObject.transform.SetParent(parent, false);
            var text = textObject.AddComponent<Text>();
            text.alignment = TextAnchor.MiddleCenter;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.color = color;
            var rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return text;
        }

        private void CreateProjectileTrail(GameObject projectileObject, float radius)
        {
            var trail = projectileObject.AddComponent<TrailRenderer>();
            trail.sharedMaterial = _trailMaterial;
            trail.time = 0.18f;
            trail.startWidth = radius * 1.2f;
            trail.endWidth = 0f;
            trail.minVertexDistance = 0.02f;
            trail.numCornerVertices = 4;
        }

        private void CreateImpactBurst(Vector3 center, float radius)
        {
            var burstObject = new GameObject("Impact Burst");
            _transientObjects.Add(burstObject);
            burstObject.transform.position = new Vector3(center.x, 0.4f, center.z);
            var particles = burstObject.AddComponent<ParticleSystem>();
            var main = particles.main;
            main.startLifetime = 0.35f;
            main.startSpeed = Mathf.Max(2f, radius * 2f);
            main.startSize = Mathf.Max(0.12f, radius * 0.18f);
            main.maxParticles = 64;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            var emission = particles.emission;
            emission.enabled = false;
            var shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = Mathf.Max(0.1f, radius * 0.2f);
            var renderer = particles.GetComponent<ParticleSystemRenderer>();
            renderer.sharedMaterial = _trailMaterial;
            particles.Emit(Mathf.Clamp(Mathf.RoundToInt(radius * 22f), 12, 48));
            burstObject.AddComponent<TimedSelfDestroy>().SetLifeTime(0.8f);
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
    }
}
