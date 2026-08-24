using SpherePath.Configuration;
using SpherePath.GameState;
using SpherePath.Level;
using UnityEngine;
using Zenject;

namespace SpherePath.Bootstrap
{
    public sealed class EntryPoint : MonoBehaviour
    {
        [SerializeField] private GameplayConfiguration configuration;
        [SerializeField] private LevelCatalog levelCatalog;
        [SerializeField] private int initialLevelIndex;
        [SerializeField] private Transform levelParent;

        private DiContainer _container;
        private GameController _controller;

        private void Awake()
        {
            LockPortraitOrientation();
            ValidateReferences();
            _container = new DiContainer();
            var installer = new GameplayInstaller();
            installer.Install(_container, configuration, levelCatalog);
            var levelLoader = _container.Resolve<LevelLoader>();
            var level = levelLoader.Load(initialLevelIndex, levelParent);
            installer.InstallScene(_container, level, configuration);
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

        private void ValidateReferences()
        {
            if (configuration == null)
            {
                throw new System.InvalidOperationException($"{nameof(EntryPoint)} requires {nameof(configuration)}.");
            }

            if (levelCatalog == null)
            {
                throw new System.InvalidOperationException($"{nameof(EntryPoint)} requires {nameof(levelCatalog)}.");
            }

            configuration.Validate();
            levelCatalog.Validate();
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
