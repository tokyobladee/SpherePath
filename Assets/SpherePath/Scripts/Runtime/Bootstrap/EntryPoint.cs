using SpherePath.Configuration;
using SpherePath.Level;
using UnityEngine;

namespace SpherePath.Bootstrap
{
    public sealed class EntryPoint : MonoBehaviour
    {
        [SerializeField] private GameplayConfiguration configuration;
        [SerializeField] private LevelCatalog levelCatalog;
        [SerializeField] private int initialLevelIndex;
        [SerializeField] private int targetFrameRate = 60;
        [SerializeField] private Transform levelParent;

        private GameSessionRunner _sessionRunner;

        private void Awake()
        {
            StartupSettings.ApplyPerformance(targetFrameRate);
            StartupSettings.ApplyPortraitOrientation();
            ValidateReferences();
            _sessionRunner = new GameSessionRunner(configuration, levelCatalog, levelParent, initialLevelIndex);
            _sessionRunner.Start();
        }

        private void Update()
        {
            _sessionRunner?.Tick(Time.deltaTime);
        }

        private void OnDestroy()
        {
            _sessionRunner?.Dispose();
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
    }
}
