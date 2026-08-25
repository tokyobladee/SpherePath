using UnityEngine;

namespace SpherePath.Bootstrap
{
    public static class StartupSettings
    {
        private const int DefaultTargetFrameRate = 60;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void ApplyBeforeSplashScreen()
        {
            ApplyPortraitOrientation();
            ApplyPerformance(DefaultTargetFrameRate);
        }

        public static void ApplyPerformance(int targetFrameRate)
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = Mathf.Max(30, targetFrameRate);
        }

        public static void ApplyPortraitOrientation()
        {
            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;
            Screen.orientation = ScreenOrientation.Portrait;
        }
    }
}
