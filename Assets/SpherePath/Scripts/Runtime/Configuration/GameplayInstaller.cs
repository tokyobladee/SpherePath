using SpherePath.GameState;
using SpherePath.Input;
using SpherePath.Obstacles;
using SpherePath.Pathing;
using SpherePath.Player;
using SpherePath.Level;
using SpherePath.Shooting;
using Zenject;

namespace SpherePath.Configuration
{
    public sealed class GameplayInstaller
    {
        public void Install(DiContainer container, GameplayConfiguration configuration, LevelCatalog levelCatalog)
        {
            container.BindInstance(configuration).AsSingle();
            container.BindInstance(levelCatalog).AsSingle();
            container.Bind<LevelLoader>().AsSingle();
            container.Bind<PlayerEnergy>().AsSingle().WithArguments(configuration.MaximumEnergy);
            container.Bind<PlayerSizeService>().AsSingle().WithArguments(configuration.MinimumPlayerRadius, configuration.MaximumPlayerRadius);
            container.Bind<PlayerViabilityService>().AsSingle();
            container.Bind<ChargeMeter>().AsSingle().WithArguments(configuration.MaxChargeTime, configuration.MinimumShotCost);
            container.Bind<ShotChargeService>().AsSingle().WithArguments(configuration.MinimumProjectileRadius, configuration.MaximumProjectileRadius);
            container.Bind<PointerChargeInput>().AsSingle();
            container.Bind<ObstacleClearingService>().AsSingle().WithArguments(configuration.InfectionRadiusMultiplier, configuration.ProjectileImpactRadius);
        }

        public void InstallScene(DiContainer container, LevelViewReferences sceneReferences, GameplayConfiguration configuration)
        {
            container.BindInstance(sceneReferences).AsSingle();
            container.Bind<PathClearanceService>().AsSingle().WithArguments(sceneReferences.Obstacles, configuration.PathLateralClearance, configuration.PathStopDistance);
            container.Bind<TransientViewFactory>().AsSingle();
            container.Bind<GameController>().AsSingle();
        }
    }
}
