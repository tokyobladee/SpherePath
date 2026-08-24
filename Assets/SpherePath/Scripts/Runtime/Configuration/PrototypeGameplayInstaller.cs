using SpherePath.Input;
using SpherePath.Obstacles;
using SpherePath.Player;
using SpherePath.Shooting;
using UnityEngine;
using Zenject;

namespace SpherePath.Configuration
{
    public sealed class PrototypeGameplayInstaller : MonoInstaller
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
        [SerializeField] private float infectionRadiusMultiplier = 2.25f;

        public override void InstallBindings()
        {
            Container.Bind<PlayerEnergy>().AsSingle().WithArguments(maximumEnergy, minimumEnergy);
            Container.Bind<PlayerSizeService>().AsSingle().WithArguments(minimumPlayerRadius, maximumPlayerRadius);
            Container.Bind<PlayerViabilityService>().AsSingle();
            Container.Bind<ChargeMeter>().AsSingle().WithArguments(maxChargeTime, minimumProjectileRadius, maximumProjectileRadius, minimumShotCost, maximumShotCost);
            Container.Bind<ShotChargeService>().AsSingle();
            Container.Bind<PointerChargeInput>().AsSingle();
            Container.Bind<ObstacleClearingService>().AsSingle().WithArguments(infectionRadiusMultiplier);
            Container.Bind<ObstacleFieldLayout>().AsSingle();
        }
    }
}
