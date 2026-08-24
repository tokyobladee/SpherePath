using SpherePath.Configuration;
using SpherePath.Scene;
using UnityEngine;
using Zenject;

namespace SpherePath.GameState
{
    public sealed class GameEntryPoint : MonoBehaviour
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

        private DiContainer _container;
        private GameController _controller;

        private void Awake()
        {
            LockPortraitOrientation();
            var configuration = CreateConfiguration();
            _container = new DiContainer();
            var installer = new GameplayInstaller();
            installer.Install(_container, configuration);
            var sceneFactory = _container.Resolve<LevelViewFactory>();
            var sceneReferences = sceneFactory.Build();
            installer.InstallScene(_container, sceneReferences, configuration);
            _controller = _container.Resolve<GameController>();
            _controller.Initialize();
        }

        private void Update()
        {
            _controller?.Tick(Time.deltaTime);
        }

        private void OnDestroy()
        {
            _controller?.Dispose();
        }

        private GameplayConfiguration CreateConfiguration()
        {
            return new GameplayConfiguration(
                maximumEnergy,
                minimumEnergy,
                minimumPlayerRadius,
                maximumPlayerRadius,
                maxChargeTime,
                minimumProjectileRadius,
                maximumProjectileRadius,
                minimumShotCost,
                maximumShotCost,
                projectileSpeed,
                projectileLifeTime,
                infectionRadiusMultiplier,
                playerMoveSpeed,
                cameraFollowSpeed,
                doorOpenDistance,
                0.75f,
                new Vector3(0f, maximumPlayerRadius, -12f),
                new Vector3(0f, 1.5f, 16f));
        }

        private void LockPortraitOrientation()
        {
            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;
            Screen.orientation = ScreenOrientation.Portrait;
        }
    }
}
