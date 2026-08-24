using SpherePath.Configuration;
using SpherePath.GameState;
using SpherePath.Level;
using UnityEngine;
using Zenject;

namespace SpherePath.Bootstrap
{
    public sealed class GameSessionRunner
    {
        private readonly GameplayConfiguration _configuration;
        private readonly LevelCatalog _levelCatalog;
        private readonly Transform _levelParent;
        private readonly GameplayInstaller _installer = new GameplayInstaller();

        private DiContainer _container;
        private GameController _controller;
        private LevelViewReferences _level;
        private int _levelIndex;
        private bool _isLevelTransitionPending;

        public GameSessionRunner(GameplayConfiguration configuration, LevelCatalog levelCatalog, Transform levelParent, int initialLevelIndex)
        {
            _configuration = configuration;
            _levelCatalog = levelCatalog;
            _levelParent = levelParent;
            _levelIndex = Mathf.Clamp(initialLevelIndex, 0, Mathf.Max(0, levelCatalog.Count - 1));
        }

        public void Start()
        {
            LoadLevel(_levelIndex);
        }

        public void Tick(float deltaTime)
        {
            if (_controller == null)
            {
                return;
            }

            _controller.Tick(deltaTime);

            if (!_isLevelTransitionPending)
            {
                return;
            }

            _isLevelTransitionPending = false;
            LoadNextLevel();
        }

        public void Dispose()
        {
            DisposeLevel();
        }

        private void LoadLevel(int levelIndex)
        {
            DisposeLevel();
            _isLevelTransitionPending = false;
            _container = new DiContainer();
            _installer.Install(_container, _configuration, _levelCatalog);
            var levelLoader = _container.Resolve<LevelLoader>();
            _level = levelLoader.Load(levelIndex, _levelParent);
            _installer.InstallScene(_container, _level, _configuration);
            _controller = _container.Resolve<GameController>();
            _controller.Completed += QueueLevelTransition;
            _controller.Initialize();
            _level.Ui.SetLevelProgress(levelIndex + 1, GetNextLevelLabel(levelIndex), 0f);
        }

        private void QueueLevelTransition()
        {
            _isLevelTransitionPending = true;
        }

        private void LoadNextLevel()
        {
            _levelIndex = (_levelIndex + 1) % _levelCatalog.Count;
            LoadLevel(_levelIndex);
        }

        private string GetNextLevelLabel(int levelIndex)
        {
            var nextLevelIndex = levelIndex + 1;
            return nextLevelIndex >= _levelCatalog.Count ? string.Empty : (nextLevelIndex + 1).ToString();
        }

        private void DisposeLevel()
        {
            if (_controller != null)
            {
                _controller.Completed -= QueueLevelTransition;
                _controller.Dispose();
                _controller = null;
            }

            if (_level != null)
            {
                _level.gameObject.SetActive(false);
                Object.Destroy(_level.gameObject);
                _level = null;
            }

            _container = null;
        }
    }
}
