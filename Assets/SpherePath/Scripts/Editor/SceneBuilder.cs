using System.Collections.Generic;
using SpherePath.Bootstrap;
using SpherePath.Cameras;
using SpherePath.Configuration;
using SpherePath.Level;
using SpherePath.Obstacles;
using SpherePath.Player;
using SpherePath.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SpherePath.Editor
{
    public static class SceneBuilder
    {
        private const string ScenePath = "Assets/SpherePath/Scenes/Main.unity";
        private const string ConfigurationPath = "Assets/SpherePath/ScriptableObjects/DefaultGameplayConfiguration.asset";
        private const string LevelCatalogPath = "Assets/SpherePath/ScriptableObjects/DefaultLevelCatalog.asset";
        private const string LevelPrefabFolder = "Assets/SpherePath/Prefabs/Levels";
        private const string MaterialFolder = "Assets/SpherePath/Materials";

        private static readonly LevelDefinition[] LevelDefinitions =
        {
            new LevelDefinition(
                "Level01",
                new[]
                {
                    new Vector3(-2.2f, 0.6f, -4.8f),
                    new Vector3(-0.8f, 0.6f, -4.4f),
                    new Vector3(0.8f, 0.6f, -4.4f),
                    new Vector3(2.2f, 0.6f, -4.8f),
                    new Vector3(-1.7f, 0.6f, -2.4f),
                    new Vector3(0f, 0.6f, -2.1f),
                    new Vector3(1.7f, 0.6f, -2.4f),
                    new Vector3(-2.4f, 0.6f, 0.1f),
                    new Vector3(-0.7f, 0.6f, 0.4f),
                    new Vector3(1f, 0.6f, 0.2f),
                    new Vector3(2.5f, 0.6f, 0.9f),
                    new Vector3(-1.9f, 0.6f, 2.9f),
                    new Vector3(-0.2f, 0.6f, 3.2f),
                    new Vector3(1.6f, 0.6f, 3f),
                    new Vector3(-2.5f, 0.6f, 5.7f),
                    new Vector3(-0.8f, 0.6f, 5.5f),
                    new Vector3(0.9f, 0.6f, 5.8f),
                    new Vector3(2.3f, 0.6f, 6.3f),
                    new Vector3(-1.6f, 0.6f, 8.4f),
                    new Vector3(0.1f, 0.6f, 8.8f),
                    new Vector3(1.8f, 0.6f, 8.5f),
                    new Vector3(-0.8f, 0.6f, 11.1f),
                    new Vector3(0.9f, 0.6f, 11.3f)
                }),
            new LevelDefinition(
                "Level02",
                new[]
                {
                    new Vector3(-2.4f, 0.6f, -5.2f),
                    new Vector3(-0.4f, 0.6f, -4.9f),
                    new Vector3(1.5f, 0.6f, -4.5f),
                    new Vector3(2.6f, 0.6f, -3.1f),
                    new Vector3(-1.4f, 0.6f, -2.6f),
                    new Vector3(0.7f, 0.6f, -2.2f),
                    new Vector3(-2.6f, 0.6f, -0.3f),
                    new Vector3(-0.9f, 0.6f, 0.2f),
                    new Vector3(1.2f, 0.6f, 0f),
                    new Vector3(2.4f, 0.6f, 1.4f),
                    new Vector3(-1.9f, 0.6f, 2.2f),
                    new Vector3(0f, 0.6f, 2.8f),
                    new Vector3(1.7f, 0.6f, 3.5f),
                    new Vector3(-2.5f, 0.6f, 5.1f),
                    new Vector3(-0.6f, 0.6f, 5.7f),
                    new Vector3(1.4f, 0.6f, 5.3f),
                    new Vector3(2.5f, 0.6f, 7f),
                    new Vector3(-1.2f, 0.6f, 8.2f),
                    new Vector3(0.8f, 0.6f, 8.8f),
                    new Vector3(-2.2f, 0.6f, 10.2f),
                    new Vector3(-0.1f, 0.6f, 10.9f),
                    new Vector3(1.9f, 0.6f, 11.4f)
                }),
            new LevelDefinition(
                "Level03",
                new[]
                {
                    new Vector3(-2.5f, 0.6f, -5.4f),
                    new Vector3(-1.1f, 0.6f, -4.7f),
                    new Vector3(0.5f, 0.6f, -5.1f),
                    new Vector3(2.2f, 0.6f, -4.3f),
                    new Vector3(-2.1f, 0.6f, -2.8f),
                    new Vector3(-0.2f, 0.6f, -2.4f),
                    new Vector3(1.8f, 0.6f, -2.7f),
                    new Vector3(-2.8f, 0.6f, -0.8f),
                    new Vector3(-1.2f, 0.6f, 0.4f),
                    new Vector3(0.7f, 0.6f, 0.1f),
                    new Vector3(2.3f, 0.6f, 0.7f),
                    new Vector3(-2.2f, 0.6f, 2.4f),
                    new Vector3(-0.4f, 0.6f, 3.1f),
                    new Vector3(1.3f, 0.6f, 2.7f),
                    new Vector3(2.7f, 0.6f, 3.9f),
                    new Vector3(-2.6f, 0.6f, 5.4f),
                    new Vector3(-0.8f, 0.6f, 5.9f),
                    new Vector3(0.9f, 0.6f, 6.4f),
                    new Vector3(2.4f, 0.6f, 6.8f),
                    new Vector3(-1.7f, 0.6f, 8.6f),
                    new Vector3(0.2f, 0.6f, 9.1f),
                    new Vector3(1.8f, 0.6f, 9.7f),
                    new Vector3(-0.9f, 0.6f, 11.2f),
                    new Vector3(1f, 0.6f, 11.5f)
                })
        };

        [MenuItem("SpherePath/Setup Game Scene")]
        public static void SetupGameScene()
        {
            EnsureFolders();
            var configuration = LoadOrCreateConfiguration();
            var levelReferences = CreateLevelPrefabs();
            var levelCatalog = CreateLevelCatalog(levelReferences);
            var scene = CreateScene();
            var entryPoint = CreateEntryPoint(configuration, levelCatalog);
            CreateEventSystem();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            Selection.activeObject = entryPoint;
        }

        private static void EnsureFolders()
        {
            EnsureFolder("Assets/SpherePath/Scenes");
            EnsureFolder("Assets/SpherePath/Prefabs");
            EnsureFolder("Assets/SpherePath/Prefabs/Levels");
            EnsureFolder("Assets/SpherePath/Materials");
            EnsureFolder("Assets/SpherePath/ScriptableObjects");
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            var parent = System.IO.Path.GetDirectoryName(path).Replace('\\', '/');
            var name = System.IO.Path.GetFileName(path);
            AssetDatabase.CreateFolder(parent, name);
        }

        private static GameplayConfiguration LoadOrCreateConfiguration()
        {
            var configuration = AssetDatabase.LoadAssetAtPath<GameplayConfiguration>(ConfigurationPath);
            if (configuration != null)
            {
                return configuration;
            }

            configuration = ScriptableObject.CreateInstance<GameplayConfiguration>();
            AssetDatabase.CreateAsset(configuration, ConfigurationPath);
            AssetDatabase.SaveAssets();
            return configuration;
        }

        private static List<LevelViewReferences> CreateLevelPrefabs()
        {
            var playerMaterial = LoadOrCreateMaterial("Player", new Color(1f, 0.58f, 0.12f, 1f));
            var projectileMaterial = LoadOrCreateMaterial("Projectile", new Color(1f, 0.78f, 0.25f, 1f));
            var groundMaterial = LoadOrCreateMaterial("Ground", new Color(0.54f, 0.64f, 0.47f, 1f));
            var obstacleMaterial = LoadOrCreateMaterial("Obstacle", new Color(0.24f, 0.58f, 0.28f, 1f));
            var doorMaterial = LoadOrCreateMaterial("Door", new Color(1f, 0.56f, 0.16f, 1f));
            var corridorMaterial = LoadOrCreateMaterial("Corridor", new Color(1f, 0.18f, 0.4f, 0.35f));
            var infectionPreviewMaterial = LoadOrCreateMaterial("InfectionPreview", new Color(1f, 0.35f, 0.12f, 0.28f));
            var trailMaterial = LoadOrCreateMaterial("Trail", new Color(1f, 0.7f, 0.18f, 0.65f));
            var levelReferences = new List<LevelViewReferences>();

            foreach (var definition in LevelDefinitions)
            {
                levelReferences.Add(CreateLevelPrefab(
                    definition,
                    playerMaterial,
                    projectileMaterial,
                    groundMaterial,
                    obstacleMaterial,
                    doorMaterial,
                    corridorMaterial,
                    infectionPreviewMaterial,
                    trailMaterial));
            }

            return levelReferences;
        }

        private static LevelViewReferences CreateLevelPrefab(
            LevelDefinition definition,
            Material playerMaterial,
            Material projectileMaterial,
            Material groundMaterial,
            Material obstacleMaterial,
            Material doorMaterial,
            Material corridorMaterial,
            Material infectionPreviewMaterial,
            Material trailMaterial)
        {
            var levelObject = new GameObject(definition.Name);
            var references = levelObject.AddComponent<LevelViewReferences>();
            var cameraView = CreateCamera(levelObject.transform);
            CreateLight(levelObject.transform);
            CreateGround(levelObject.transform, groundMaterial);
            var playerSpawn = CreatePoint(levelObject.transform, "Player Spawn", new Vector3(0f, 1.1f, -12f));
            var door = CreateDoor(levelObject.transform, doorMaterial, out var leftDoorPanel, out var rightDoorPanel);
            var player = CreatePlayer(levelObject.transform, playerMaterial);
            var corridor = CreateCorridor(levelObject.transform, corridorMaterial);
            var chargePreview = CreateChargePreview(levelObject.transform, projectileMaterial);
            var obstacles = CreateObstacles(levelObject.transform, obstacleMaterial, definition.ObstaclePositions);
            var ui = CreateUi(levelObject.transform);

            AssignLevelReferences(
                references,
                obstacles,
                player,
                playerSpawn,
                door,
                leftDoorPanel,
                rightDoorPanel,
                corridor,
                chargePreview,
                cameraView,
                ui,
                projectileMaterial,
                infectionPreviewMaterial,
                trailMaterial);

            var prefab = PrefabUtility.SaveAsPrefabAsset(levelObject, $"{LevelPrefabFolder}/{definition.Name}.prefab");
            Object.DestroyImmediate(levelObject);
            AssetDatabase.SaveAssets();
            return prefab.GetComponent<LevelViewReferences>();
        }

        private static LevelCatalog CreateLevelCatalog(List<LevelViewReferences> levelReferences)
        {
            var catalog = AssetDatabase.LoadAssetAtPath<LevelCatalog>(LevelCatalogPath);
            if (catalog == null)
            {
                catalog = ScriptableObject.CreateInstance<LevelCatalog>();
                AssetDatabase.CreateAsset(catalog, LevelCatalogPath);
            }

            var serializedCatalog = new SerializedObject(catalog);
            var levelsProperty = serializedCatalog.FindProperty("levels");
            levelsProperty.arraySize = levelReferences.Count;
            for (var i = 0; i < levelReferences.Count; i++)
            {
                levelsProperty.GetArrayElementAtIndex(i).objectReferenceValue = levelReferences[i];
            }

            serializedCatalog.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(catalog);
            AssetDatabase.SaveAssets();
            return catalog;
        }

        private static Material LoadOrCreateMaterial(string materialName, Color color)
        {
            var path = $"{MaterialFolder}/{materialName}.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material != null)
            {
                return material;
            }

            var shader = Shader.Find("Universal Render Pipeline/Lit");
            material = new Material(shader != null ? shader : Shader.Find("Standard"));
            material.color = color;
            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        private static Scene CreateScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSceneManager.SaveScene(scene, ScenePath);
            return scene;
        }

        private static EntryPoint CreateEntryPoint(GameplayConfiguration configuration, LevelCatalog levelCatalog)
        {
            var entryPointObject = new GameObject("EntryPoint");
            var entryPoint = entryPointObject.AddComponent<EntryPoint>();
            var serializedEntryPoint = new SerializedObject(entryPoint);
            serializedEntryPoint.FindProperty("configuration").objectReferenceValue = configuration;
            serializedEntryPoint.FindProperty("levelCatalog").objectReferenceValue = levelCatalog;
            serializedEntryPoint.FindProperty("initialLevelIndex").intValue = 0;
            serializedEntryPoint.ApplyModifiedPropertiesWithoutUndo();
            return entryPoint;
        }

        private static FollowCameraView CreateCamera(Transform parent)
        {
            var cameraObject = new GameObject("Main Camera");
            cameraObject.transform.SetParent(parent);
            var camera = cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
            cameraObject.tag = "MainCamera";
            camera.transform.SetPositionAndRotation(new Vector3(0f, 18.5f, -17f), Quaternion.Euler(58f, 0f, 0f));
            camera.fieldOfView = 37f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.18f, 0.2f, 0.22f, 1f);
            var cameraView = cameraObject.AddComponent<FollowCameraView>();
            cameraView.CaptureBasePose();
            cameraView.SetFollowSettings(new Vector3(0f, 18.5f, -8.5f), 6f);
            return cameraView;
        }

        private static void CreateLight(Transform parent)
        {
            var lightObject = new GameObject("Directional Light");
            lightObject.transform.SetParent(parent);
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            light.intensity = 2.4f;
        }

        private static void CreateGround(Transform parent, Material material)
        {
            var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "Ground";
            ground.transform.SetParent(parent);
            ground.transform.position = new Vector3(0f, -0.05f, 0f);
            ground.transform.localScale = new Vector3(14f, 0.1f, 46f);
            ground.GetComponent<Renderer>().sharedMaterial = material;
        }

        private static Transform CreatePoint(Transform parent, string objectName, Vector3 position)
        {
            var point = new GameObject(objectName).transform;
            point.SetParent(parent);
            point.position = position;
            return point;
        }

        private static Transform CreateDoor(Transform parent, Material material, out Transform leftDoorPanel, out Transform rightDoorPanel)
        {
            var door = new GameObject("Goal Door").transform;
            door.SetParent(parent);
            door.position = new Vector3(0f, 1.5f, 16f);
            leftDoorPanel = CreateDoorPanel(door, "Left Door Panel", new Vector3(-0.45f, 0f, 0f), material);
            rightDoorPanel = CreateDoorPanel(door, "Right Door Panel", new Vector3(0.45f, 0f, 0f), material);
            return door;
        }

        private static Transform CreateDoorPanel(Transform parent, string objectName, Vector3 localPosition, Material material)
        {
            var panel = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            panel.name = objectName;
            panel.SetParent(parent);
            panel.localPosition = localPosition;
            panel.localScale = new Vector3(0.8f, 3f, 0.25f);
            panel.GetComponent<Renderer>().sharedMaterial = material;
            return panel;
        }

        private static PlayerView CreatePlayer(Transform parent, Material material)
        {
            var playerObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            playerObject.name = "Player Sphere";
            playerObject.transform.SetParent(parent);
            playerObject.GetComponent<Renderer>().sharedMaterial = material;
            return playerObject.AddComponent<PlayerView>();
        }

        private static Transform CreateCorridor(Transform parent, Material material)
        {
            var corridorObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            corridorObject.name = "Path Width Indicator";
            corridorObject.transform.SetParent(parent);
            Object.DestroyImmediate(corridorObject.GetComponent<Collider>());
            corridorObject.GetComponent<Renderer>().sharedMaterial = material;
            return corridorObject.transform;
        }

        private static Transform CreateChargePreview(Transform parent, Material material)
        {
            var previewObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            previewObject.name = "Projectile Preview";
            previewObject.transform.SetParent(parent);
            Object.DestroyImmediate(previewObject.GetComponent<Collider>());
            previewObject.GetComponent<Renderer>().sharedMaterial = material;
            return previewObject.transform;
        }

        private static List<Obstacle> CreateObstacles(Transform parent, Material material, IReadOnlyList<Vector3> positions)
        {
            var obstacles = new List<Obstacle>();
            foreach (var position in positions)
            {
                var obstacleObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                obstacleObject.name = "Obstacle";
                obstacleObject.transform.SetParent(parent);
                obstacleObject.transform.position = position;
                obstacleObject.transform.localScale = new Vector3(1f, 1.2f, 1f);
                obstacleObject.GetComponent<Renderer>().sharedMaterial = material;
                obstacles.Add(obstacleObject.AddComponent<Obstacle>());
            }

            return obstacles;
        }

        private static GameUiView CreateUi(Transform parent)
        {
            var canvasObject = new GameObject("Game UI");
            canvasObject.transform.SetParent(parent);
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

            var energySlider = CreateEnergySlider(safeAreaObject.transform);
            var statusText = CreateStatusText(safeAreaObject.transform);
            var resultPanel = CreateResultPanel(safeAreaObject.transform, out var resultHintText, out var restartButton);
            var ui = canvasObject.AddComponent<GameUiView>();
            AssignUiReferences(ui, energySlider, statusText, resultPanel, resultHintText, restartButton, safeArea);
            return ui;
        }

        private static Slider CreateEnergySlider(Transform parent)
        {
            var sliderObject = new GameObject("Energy Slider");
            sliderObject.transform.SetParent(parent, false);
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
            return energySlider;
        }

        private static Text CreateStatusText(Transform parent)
        {
            var statusObject = new GameObject("Status Text");
            statusObject.transform.SetParent(parent, false);
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
            return statusText;
        }

        private static GameObject CreateResultPanel(Transform parent, out Text hintText, out Button restartButton)
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

        private static Text CreateText(string objectName, Transform parent, int fontSize, Color color)
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

        private static Image CreateUiImage(string objectName, Transform parent, Color color)
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

        private static void CreateEventSystem()
        {
            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<InputSystemUIInputModule>();
        }

        private static void AssignLevelReferences(
            LevelViewReferences references,
            List<Obstacle> obstacles,
            PlayerView player,
            Transform playerSpawn,
            Transform door,
            Transform leftDoorPanel,
            Transform rightDoorPanel,
            Transform corridor,
            Transform chargePreview,
            FollowCameraView cameraView,
            GameUiView ui,
            Material projectileMaterial,
            Material infectionPreviewMaterial,
            Material trailMaterial)
        {
            var serializedReferences = new SerializedObject(references);
            var obstaclesProperty = serializedReferences.FindProperty("obstacles");
            obstaclesProperty.arraySize = obstacles.Count;
            for (var i = 0; i < obstacles.Count; i++)
            {
                obstaclesProperty.GetArrayElementAtIndex(i).objectReferenceValue = obstacles[i];
            }

            serializedReferences.FindProperty("player").objectReferenceValue = player;
            serializedReferences.FindProperty("playerSpawnPoint").objectReferenceValue = playerSpawn;
            serializedReferences.FindProperty("door").objectReferenceValue = door;
            serializedReferences.FindProperty("doorLeftPanel").objectReferenceValue = leftDoorPanel;
            serializedReferences.FindProperty("doorRightPanel").objectReferenceValue = rightDoorPanel;
            serializedReferences.FindProperty("corridor").objectReferenceValue = corridor;
            serializedReferences.FindProperty("chargePreview").objectReferenceValue = chargePreview;
            serializedReferences.FindProperty("cameraView").objectReferenceValue = cameraView;
            serializedReferences.FindProperty("ui").objectReferenceValue = ui;
            serializedReferences.FindProperty("projectileMaterial").objectReferenceValue = projectileMaterial;
            serializedReferences.FindProperty("infectionPreviewMaterial").objectReferenceValue = infectionPreviewMaterial;
            serializedReferences.FindProperty("trailMaterial").objectReferenceValue = trailMaterial;
            serializedReferences.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void AssignUiReferences(GameUiView ui, Slider energySlider, Text statusText, GameObject resultPanel, Text resultHintText, Button restartButton, RectTransform safeArea)
        {
            var serializedUi = new SerializedObject(ui);
            serializedUi.FindProperty("energySlider").objectReferenceValue = energySlider;
            serializedUi.FindProperty("statusText").objectReferenceValue = statusText;
            serializedUi.FindProperty("resultPanel").objectReferenceValue = resultPanel;
            serializedUi.FindProperty("resultHintText").objectReferenceValue = resultHintText;
            serializedUi.FindProperty("restartButton").objectReferenceValue = restartButton;
            serializedUi.FindProperty("safeArea").objectReferenceValue = safeArea;
            serializedUi.ApplyModifiedPropertiesWithoutUndo();
        }

        private readonly struct LevelDefinition
        {
            public LevelDefinition(string name, IReadOnlyList<Vector3> obstaclePositions)
            {
                Name = name;
                ObstaclePositions = obstaclePositions;
            }

            public string Name { get; }

            public IReadOnlyList<Vector3> ObstaclePositions { get; }
        }
    }
}
