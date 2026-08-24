using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace SpherePath.Editor
{
    public static class AndroidBuildRunner
    {
        private const string ScenePath = "Assets/SpherePath/Scenes/Main.unity";
        private const string OutputPath = "Builds/Android/SpherePath.apk";

        [MenuItem("SpherePath/Build Android APK")]
        public static void BuildApk()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(OutputPath));
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

            var options = new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = OutputPath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new BuildFailedException(report.summary.result.ToString());
            }
        }
    }
}
