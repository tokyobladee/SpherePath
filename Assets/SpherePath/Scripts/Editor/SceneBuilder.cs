using SpherePath.Bootstrap;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpherePath.Editor
{
    public static class SceneBuilder
    {
        [MenuItem("SpherePath/Setup Game Scene")]
        public static void SetupGameScene()
        {
            var scene = EditorSceneManager.OpenScene("Assets/SpherePath/Scenes/SampleScene.unity", OpenSceneMode.Single);
            var controller = Object.FindAnyObjectByType<EntryPoint>();
            if (controller == null)
            {
                var controllerObject = new GameObject("SpherePath Game Controller");
                controllerObject.AddComponent<EntryPoint>();
            }

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

            var light = Object.FindAnyObjectByType<Light>();
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
