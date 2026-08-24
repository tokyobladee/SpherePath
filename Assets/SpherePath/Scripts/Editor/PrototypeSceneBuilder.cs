using SpherePath.GameState;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpherePath.Editor
{
    public static class PrototypeSceneBuilder
    {
        [MenuItem("SpherePath/Setup Prototype Scene")]
        public static void SetupPrototypeScene()
        {
            var scene = EditorSceneManager.OpenScene("Assets/SpherePath/Scenes/SampleScene.unity", OpenSceneMode.Single);
            var controller = Object.FindFirstObjectByType<PrototypeGameController>();
            if (controller == null)
            {
                var controllerObject = new GameObject("SpherePath Prototype Controller");
                controllerObject.AddComponent<PrototypeGameController>();
            }

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

            var light = Object.FindFirstObjectByType<Light>();
            if (light == null)
            {
                var lightObject = new GameObject("Directional Light");
                light = lightObject.AddComponent<Light>();
                light.type = LightType.Directional;
            }

            light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            light.intensity = 2.4f;
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }
    }
}
