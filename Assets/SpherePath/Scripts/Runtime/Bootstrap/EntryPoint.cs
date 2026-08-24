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
        [SerializeField] private LevelViewReferences levelPrefab;
        [SerializeField] private Transform levelParent;

        private DiContainer _container;
        private GameController _controller;
        private LevelViewReferences _level;

        private void Awake()
        {
            LockPortraitOrientation();
            ValidateReferences();
            _level = CreateLevel();
            _container = new DiContainer();
            var installer = new GameplayInstaller();
            installer.Install(_container, configuration);
            installer.InstallScene(_container, _level, configuration);
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

            if (levelPrefab == null)
            {
                throw new System.InvalidOperationException($"{nameof(EntryPoint)} requires {nameof(levelPrefab)}.");
            }

            configuration.Validate();
        }

        private LevelViewReferences CreateLevel()
        {
            var level = levelPrefab.gameObject.scene.IsValid()
                ? levelPrefab
                : Instantiate(levelPrefab, levelParent);
            level.Validate();
            return level;
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
