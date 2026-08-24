using UnityEngine;

namespace SpherePath.Configuration
{
    public sealed class GameplayConfiguration
    {
        public GameplayConfiguration(
            float maximumEnergy,
            float minimumEnergy,
            float minimumPlayerRadius,
            float maximumPlayerRadius,
            float maxChargeTime,
            float minimumProjectileRadius,
            float maximumProjectileRadius,
            float minimumShotCost,
            float maximumShotCost,
            float projectileSpeed,
            float projectileLifeTime,
            float infectionRadiusMultiplier,
            float playerMoveSpeed,
            float cameraFollowSpeed,
            float doorOpenDistance,
            float pathSafePadding,
            Vector3 playerStartPosition,
            Vector3 doorPosition)
        {
            MaximumEnergy = maximumEnergy;
            MinimumEnergy = minimumEnergy;
            MinimumPlayerRadius = minimumPlayerRadius;
            MaximumPlayerRadius = maximumPlayerRadius;
            MaxChargeTime = maxChargeTime;
            MinimumProjectileRadius = minimumProjectileRadius;
            MaximumProjectileRadius = maximumProjectileRadius;
            MinimumShotCost = minimumShotCost;
            MaximumShotCost = maximumShotCost;
            ProjectileSpeed = projectileSpeed;
            ProjectileLifeTime = projectileLifeTime;
            InfectionRadiusMultiplier = infectionRadiusMultiplier;
            PlayerMoveSpeed = playerMoveSpeed;
            CameraFollowSpeed = cameraFollowSpeed;
            DoorOpenDistance = doorOpenDistance;
            PathSafePadding = pathSafePadding;
            PlayerStartPosition = playerStartPosition;
            DoorPosition = doorPosition;
        }

        public float MaximumEnergy { get; }

        public float MinimumEnergy { get; }

        public float MinimumPlayerRadius { get; }

        public float MaximumPlayerRadius { get; }

        public float MaxChargeTime { get; }

        public float MinimumProjectileRadius { get; }

        public float MaximumProjectileRadius { get; }

        public float MinimumShotCost { get; }

        public float MaximumShotCost { get; }

        public float ProjectileSpeed { get; }

        public float ProjectileLifeTime { get; }

        public float InfectionRadiusMultiplier { get; }

        public float PlayerMoveSpeed { get; }

        public float CameraFollowSpeed { get; }

        public float DoorOpenDistance { get; }

        public float PathSafePadding { get; }

        public Vector3 PlayerStartPosition { get; }

        public Vector3 DoorPosition { get; }
    }
}
