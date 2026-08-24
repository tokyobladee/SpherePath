using UnityEngine;

namespace SpherePath.Configuration
{
    [CreateAssetMenu(fileName = "GameplayConfiguration", menuName = "SpherePath/Gameplay Configuration")]
    public sealed class GameplayConfiguration : ScriptableObject
    {
        [SerializeField] private float maximumEnergy = 10f;
        [SerializeField] private float minimumPlayerRadius = 0.35f;
        [SerializeField] private float maximumPlayerRadius = 1.1f;
        [SerializeField] private float maxChargeTime = 1.6f;
        [SerializeField] private float minimumShotCost = 0.25f;
        [SerializeField] private float minimumProjectileRadius = 0.18f;
        [SerializeField] private float maximumProjectileRadius = 1.1f;
        [SerializeField] private float projectileSpeed = 18f;
        [SerializeField] private float projectileLifeTime = 4f;
        [SerializeField] private float projectileExitDistance = 1.5f;
        [SerializeField] private float projectileImpactRadius = 0.9f;
        [SerializeField] private float infectionRadiusMultiplier = 2.25f;
        [SerializeField] private float playerMoveSpeed = 7f;
        [SerializeField] private float playerJumpDistance = 1.35f;
        [SerializeField] private float playerJumpHeight = 0.65f;
        [SerializeField] private float cameraFollowSpeed = 6f;
        [SerializeField] private float doorOpenDistance = 5f;
        [SerializeField] private float levelCompleteDistance = 1.2f;
        [SerializeField] private float pathLateralClearance = 0f;
        [SerializeField] private float pathStopDistance = 4f;

        public float MaximumEnergy => Mathf.Max(0f, maximumEnergy);

        public float MinimumPlayerRadius => Mathf.Max(0.01f, minimumPlayerRadius);

        public float MaximumPlayerRadius => Mathf.Max(MinimumPlayerRadius, maximumPlayerRadius);

        public float MaxChargeTime => Mathf.Max(0.01f, maxChargeTime);

        public float MinimumShotCost => Mathf.Max(0f, minimumShotCost);

        public float MinimumProjectileRadius => Mathf.Max(0.01f, minimumProjectileRadius);

        public float MaximumProjectileRadius => Mathf.Max(MinimumProjectileRadius, maximumProjectileRadius);

        public float ProjectileSpeed => Mathf.Max(0.01f, projectileSpeed);

        public float ProjectileLifeTime => Mathf.Max(0.01f, projectileLifeTime);

        public float ProjectileExitDistance => Mathf.Max(0f, projectileExitDistance);

        public float ProjectileImpactRadius => Mathf.Max(0f, projectileImpactRadius);

        public float InfectionRadiusMultiplier => Mathf.Max(0.01f, infectionRadiusMultiplier);

        public float PlayerMoveSpeed => Mathf.Max(0.01f, playerMoveSpeed);

        public float PlayerJumpDistance => Mathf.Max(0.1f, playerJumpDistance);

        public float PlayerJumpHeight => Mathf.Max(0f, playerJumpHeight);

        public float CameraFollowSpeed => Mathf.Max(0.01f, cameraFollowSpeed);

        public float DoorOpenDistance => Mathf.Max(0f, doorOpenDistance);

        public float LevelCompleteDistance => Mathf.Max(0f, levelCompleteDistance);

        public float PathLateralClearance => Mathf.Max(0f, pathLateralClearance);

        public float PathStopDistance => Mathf.Max(0f, pathStopDistance);

        public void Validate()
        {
            if (maximumEnergy <= 0f)
            {
                throw new System.InvalidOperationException($"{nameof(GameplayConfiguration)} requires positive {nameof(maximumEnergy)}.");
            }

            if (maximumPlayerRadius < minimumPlayerRadius)
            {
                throw new System.InvalidOperationException($"{nameof(GameplayConfiguration)} requires {nameof(maximumPlayerRadius)} to be greater than or equal to {nameof(minimumPlayerRadius)}.");
            }

            if (maximumProjectileRadius < minimumProjectileRadius)
            {
                throw new System.InvalidOperationException($"{nameof(GameplayConfiguration)} requires {nameof(maximumProjectileRadius)} to be greater than or equal to {nameof(minimumProjectileRadius)}.");
            }

            if (maxChargeTime <= 0f || projectileSpeed <= 0f || projectileLifeTime <= 0f || infectionRadiusMultiplier <= 0f || playerMoveSpeed <= 0f || cameraFollowSpeed <= 0f)
            {
                throw new System.InvalidOperationException($"{nameof(GameplayConfiguration)} contains non-positive timing, speed, or multiplier values.");
            }

            if (projectileExitDistance < 0f)
            {
                throw new System.InvalidOperationException($"{nameof(GameplayConfiguration)} requires non-negative {nameof(projectileExitDistance)}.");
            }

            if (projectileImpactRadius < 0f)
            {
                throw new System.InvalidOperationException($"{nameof(GameplayConfiguration)} requires non-negative {nameof(projectileImpactRadius)}.");
            }

            if (doorOpenDistance < 0f || levelCompleteDistance < 0f)
            {
                throw new System.InvalidOperationException($"{nameof(GameplayConfiguration)} requires non-negative door distances.");
            }

            if (playerJumpDistance <= 0f || playerJumpHeight < 0f)
            {
                throw new System.InvalidOperationException($"{nameof(GameplayConfiguration)} requires valid jump tuning values.");
            }
        }
    }
}
