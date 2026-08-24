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
        [SerializeField] private float minimumShotCost = 0.65f;
        [SerializeField] private float projectileSpeed = 18f;
        [SerializeField] private float projectileLifeTime = 4f;
        [SerializeField] private float infectionRadiusMultiplier = 2.25f;
        [SerializeField] private float playerMoveSpeed = 7f;
        [SerializeField] private float cameraFollowSpeed = 6f;
        [SerializeField] private float doorOpenDistance = 5f;
        [SerializeField] private float pathLateralClearance = 0f;
        [SerializeField] private float pathStopDistance = 4f;

        public float MaximumEnergy => Mathf.Max(0f, maximumEnergy);

        public float MinimumPlayerRadius => Mathf.Max(0.01f, minimumPlayerRadius);

        public float MaximumPlayerRadius => Mathf.Max(MinimumPlayerRadius, maximumPlayerRadius);

        public float MaxChargeTime => Mathf.Max(0.01f, maxChargeTime);

        public float MinimumShotCost => Mathf.Max(0f, minimumShotCost);

        public float ProjectileSpeed => Mathf.Max(0.01f, projectileSpeed);

        public float ProjectileLifeTime => Mathf.Max(0.01f, projectileLifeTime);

        public float InfectionRadiusMultiplier => Mathf.Max(0.01f, infectionRadiusMultiplier);

        public float PlayerMoveSpeed => Mathf.Max(0.01f, playerMoveSpeed);

        public float CameraFollowSpeed => Mathf.Max(0.01f, cameraFollowSpeed);

        public float DoorOpenDistance => Mathf.Max(0f, doorOpenDistance);

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

            if (maxChargeTime <= 0f || projectileSpeed <= 0f || projectileLifeTime <= 0f || infectionRadiusMultiplier <= 0f || playerMoveSpeed <= 0f || cameraFollowSpeed <= 0f)
            {
                throw new System.InvalidOperationException($"{nameof(GameplayConfiguration)} contains non-positive timing, speed, or multiplier values.");
            }
        }
    }
}
