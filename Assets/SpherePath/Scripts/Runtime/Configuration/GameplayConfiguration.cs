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
        [SerializeField] private Vector3 cameraFollowOffset = new Vector3(0f, 18.5f, -8.5f);
        [SerializeField] private float cameraFollowSpeed = 6f;
        [SerializeField] private float doorOpenDistance = 5f;
        [SerializeField] private float levelCompleteDistance = 1.2f;
        [SerializeField] private float pathLateralClearance = 0f;
        [SerializeField] private float pathStopDistance = 4f;
        [SerializeField] private float completionDelay = 0.75f;
        [SerializeField] private float chargePreviewGap = 0.25f;
        [SerializeField] private float projectileHitShakeDuration = 0.25f;
        [SerializeField] private float movementStartDistance = 0.05f;
        [SerializeField] private float minimumPathMoveDistance = 0.8f;
        [SerializeField] private float movementArrivalDistance = 0.02f;
        [SerializeField] private float corridorVisualPadding = 0.45f;
        [SerializeField] private float corridorGroundOffset = 0.02f;
        [SerializeField] private float corridorHeight = 0.04f;
        [SerializeField] private float doorClosedLeftPanelX = -0.45f;
        [SerializeField] private float doorOpenLeftPanelX = -0.9f;
        [SerializeField] private float doorClosedRightPanelX = 0.45f;
        [SerializeField] private float doorOpenRightPanelX = 0.9f;
        [SerializeField] private float doorOpenSpeed = 4f;
        [SerializeField] private float minimumRenderedRadius = 0.05f;
        [SerializeField] private float playerChargeVerticalScale = 0.92f;
        [SerializeField] private float playerChargeHorizontalScale = 1.05f;
        [SerializeField] private float playerIdlePulseFrequency = 2.6f;
        [SerializeField] private float playerIdlePulseScale = 0.035f;
        [SerializeField] private float cameraShakeFrequency = 45f;
        [SerializeField] private float cameraShakeHorizontalAmplitude = 0.05f;
        [SerializeField] private float cameraShakeVerticalAmplitude = 0.04f;
        [SerializeField] private float cameraShakeVerticalFrequencyMultiplier = 1.21f;
        [SerializeField] private float cameraShakeRotationFrequencyMultiplier = 0.5f;
        [SerializeField] private float projectileLiquidFrequency = 12f;
        [SerializeField] private float projectileLiquidForwardStretch = 0.06f;
        [SerializeField] private float projectileLiquidSideSquash = 0.035f;
        [SerializeField] private float projectileTrailTime = 0.34f;
        [SerializeField] private float projectileTrailWidthMultiplier = 0.45f;
        [SerializeField] private float projectileTrailMinVertexDistance = 0.02f;
        [SerializeField] private float infectionPreviewGroundOffset = 0.04f;
        [SerializeField] private float infectionPreviewHeight = 0.02f;
        [SerializeField] private float infectionPreviewLifetime = 0.35f;
        [SerializeField] private float projectileBurstLifetime = 0.48f;
        [SerializeField] private float projectileBurstMinimumSpeed = 5f;
        [SerializeField] private float projectileBurstSpeedMultiplier = 8f;
        [SerializeField] private float projectileBurstMinimumSize = 0.2f;
        [SerializeField] private float projectileBurstSizeMultiplier = 0.32f;
        [SerializeField] private Color projectileBurstColor = new Color(1f, 0.78f, 0.18f, 1f);
        [SerializeField] private float projectileBurstGravity = 0.25f;
        [SerializeField] private int projectileBurstMaxParticles = 80;
        [SerializeField] private float projectileBurstMinimumShapeRadius = 0.08f;
        [SerializeField] private float projectileBurstShapeRadiusMultiplier = 0.35f;
        [SerializeField] private float projectileBurstParticleMultiplier = 66f;
        [SerializeField] private int projectileBurstMinimumParticleCount = 32;
        [SerializeField] private int projectileBurstMaximumParticleCount = 76;
        [SerializeField] private float projectileBurstObjectLifetime = 0.9f;
        [SerializeField] private float obstacleBurstLifetime = 0.48f;
        [SerializeField] private float obstacleBurstMinimumSpeed = 2f;
        [SerializeField] private float obstacleBurstSpeedMultiplier = 4.5f;
        [SerializeField] private float obstacleBurstMinimumSize = 0.15f;
        [SerializeField] private float obstacleBurstSizeMultiplier = 0.34f;
        [SerializeField] private Color obstacleBurstColor = new Color(0.58f, 0.95f, 0.5f, 1f);
        [SerializeField] private float obstacleBurstGravity = 1.35f;
        [SerializeField] private int obstacleBurstMaxParticles = 80;
        [SerializeField] private float obstacleBurstMinimumShapeRadius = 0.1f;
        [SerializeField] private float obstacleBurstShapeRadiusMultiplier = 0.3f;
        [SerializeField] private float obstacleBurstParticleMultiplier = 70f;
        [SerializeField] private int obstacleBurstMinimumParticleCount = 34;
        [SerializeField] private int obstacleBurstMaximumParticleCount = 80;
        [SerializeField] private float obstacleBurstObjectLifetime = 1f;
        [SerializeField] private float impactBurstHeight = 0.4f;
        [SerializeField] private float impactBurstLifetime = 0.28f;
        [SerializeField] private float impactBurstMinimumSpeed = 1.7f;
        [SerializeField] private float impactBurstSpeedMultiplier = 1.6f;
        [SerializeField] private float impactBurstMinimumSize = 0.18f;
        [SerializeField] private float impactBurstSizeMultiplier = 0.26f;
        [SerializeField] private Color impactBurstColor = new Color(1f, 0.86f, 0.26f, 1f);
        [SerializeField] private float impactBurstGravity = 0.3f;
        [SerializeField] private int impactBurstMaxParticles = 64;
        [SerializeField] private float impactBurstMinimumShapeRadius = 0.1f;
        [SerializeField] private float impactBurstShapeRadiusMultiplier = 0.2f;
        [SerializeField] private float impactBurstParticleMultiplier = 46f;
        [SerializeField] private int impactBurstMinimumParticleCount = 28;
        [SerializeField] private int impactBurstMaximumParticleCount = 64;
        [SerializeField] private float impactBurstObjectLifetime = 0.75f;
        [SerializeField] private float obstacleClearFlashDuration = 0.5f;
        [SerializeField] private float obstacleClearShrinkDuration = 0.12f;
        [SerializeField] private Color obstacleVisualBaseColor = new Color(0.24f, 0.62f, 0.3f, 1f);
        [SerializeField] private Color obstacleVisualAccentColor = new Color(0.42f, 0.86f, 0.48f, 1f);
        [SerializeField] private float obstacleVisualColorVariation = 0.35f;
        [SerializeField] private float obstacleVisualHeightVariation = 0.12f;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private GameObject infectionPreviewPrefab;
        [SerializeField] private GameObject projectileBurstPrefab;
        [SerializeField] private GameObject obstacleBurstPrefab;
        [SerializeField] private GameObject impactBurstPrefab;

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

        public Vector3 CameraFollowOffset => cameraFollowOffset;

        public float CameraFollowSpeed => Mathf.Max(0.01f, cameraFollowSpeed);

        public float DoorOpenDistance => Mathf.Max(0f, doorOpenDistance);

        public float LevelCompleteDistance => Mathf.Max(0f, levelCompleteDistance);

        public float PathLateralClearance => Mathf.Max(0f, pathLateralClearance);

        public float PathStopDistance => Mathf.Max(0f, pathStopDistance);

        public float CompletionDelay => Mathf.Max(0f, completionDelay);

        public float ChargePreviewGap => Mathf.Max(0f, chargePreviewGap);

        public float ProjectileHitShakeDuration => Mathf.Max(0.01f, projectileHitShakeDuration);

        public float MovementStartDistance => Mathf.Max(0f, movementStartDistance);

        public float MinimumPathMoveDistance => Mathf.Max(MovementStartDistance, minimumPathMoveDistance);

        public float MovementArrivalDistance => Mathf.Max(0f, movementArrivalDistance);

        public float CorridorVisualPadding => Mathf.Max(0f, corridorVisualPadding);

        public float CorridorGroundOffset => corridorGroundOffset;

        public float CorridorHeight => Mathf.Max(0.01f, corridorHeight);

        public float DoorClosedLeftPanelX => doorClosedLeftPanelX;

        public float DoorOpenLeftPanelX => doorOpenLeftPanelX;

        public float DoorClosedRightPanelX => doorClosedRightPanelX;

        public float DoorOpenRightPanelX => doorOpenRightPanelX;

        public float DoorOpenSpeed => Mathf.Max(0.01f, doorOpenSpeed);

        public float MinimumRenderedRadius => Mathf.Max(0.01f, minimumRenderedRadius);

        public float PlayerChargeVerticalScale => Mathf.Max(0.01f, playerChargeVerticalScale);

        public float PlayerChargeHorizontalScale => Mathf.Max(0.01f, playerChargeHorizontalScale);

        public float PlayerIdlePulseFrequency => Mathf.Max(0f, playerIdlePulseFrequency);

        public float PlayerIdlePulseScale => Mathf.Max(0f, playerIdlePulseScale);

        public float CameraShakeFrequency => Mathf.Max(0f, cameraShakeFrequency);

        public float CameraShakeHorizontalAmplitude => Mathf.Max(0f, cameraShakeHorizontalAmplitude);

        public float CameraShakeVerticalAmplitude => Mathf.Max(0f, cameraShakeVerticalAmplitude);

        public float CameraShakeVerticalFrequencyMultiplier => cameraShakeVerticalFrequencyMultiplier;

        public float CameraShakeRotationFrequencyMultiplier => cameraShakeRotationFrequencyMultiplier;

        public float ProjectileLiquidFrequency => Mathf.Max(0f, projectileLiquidFrequency);

        public float ProjectileLiquidForwardStretch => Mathf.Max(0f, projectileLiquidForwardStretch);

        public float ProjectileLiquidSideSquash => Mathf.Max(0f, projectileLiquidSideSquash);

        public float ProjectileTrailTime => Mathf.Max(0f, projectileTrailTime);

        public float ProjectileTrailWidthMultiplier => Mathf.Max(0f, projectileTrailWidthMultiplier);

        public float ProjectileTrailMinVertexDistance => Mathf.Max(0f, projectileTrailMinVertexDistance);

        public float InfectionPreviewGroundOffset => infectionPreviewGroundOffset;

        public float InfectionPreviewHeight => Mathf.Max(0.01f, infectionPreviewHeight);

        public float InfectionPreviewLifetime => Mathf.Max(0f, infectionPreviewLifetime);

        public float ProjectileBurstLifetime => Mathf.Max(0f, projectileBurstLifetime);

        public float ProjectileBurstMinimumSpeed => Mathf.Max(0f, projectileBurstMinimumSpeed);

        public float ProjectileBurstSpeedMultiplier => Mathf.Max(0f, projectileBurstSpeedMultiplier);

        public float ProjectileBurstMinimumSize => Mathf.Max(0f, projectileBurstMinimumSize);

        public float ProjectileBurstSizeMultiplier => Mathf.Max(0f, projectileBurstSizeMultiplier);

        public Color ProjectileBurstColor => projectileBurstColor;

        public float ProjectileBurstGravity => projectileBurstGravity;

        public int ProjectileBurstMaxParticles => Mathf.Max(0, projectileBurstMaxParticles);

        public float ProjectileBurstMinimumShapeRadius => Mathf.Max(0f, projectileBurstMinimumShapeRadius);

        public float ProjectileBurstShapeRadiusMultiplier => Mathf.Max(0f, projectileBurstShapeRadiusMultiplier);

        public float ProjectileBurstParticleMultiplier => Mathf.Max(0f, projectileBurstParticleMultiplier);

        public int ProjectileBurstMinimumParticleCount => Mathf.Max(0, projectileBurstMinimumParticleCount);

        public int ProjectileBurstMaximumParticleCount => Mathf.Max(ProjectileBurstMinimumParticleCount, projectileBurstMaximumParticleCount);

        public float ProjectileBurstObjectLifetime => Mathf.Max(0f, projectileBurstObjectLifetime);

        public float ObstacleBurstLifetime => Mathf.Max(0f, obstacleBurstLifetime);

        public float ObstacleBurstMinimumSpeed => Mathf.Max(0f, obstacleBurstMinimumSpeed);

        public float ObstacleBurstSpeedMultiplier => Mathf.Max(0f, obstacleBurstSpeedMultiplier);

        public float ObstacleBurstMinimumSize => Mathf.Max(0f, obstacleBurstMinimumSize);

        public float ObstacleBurstSizeMultiplier => Mathf.Max(0f, obstacleBurstSizeMultiplier);

        public Color ObstacleBurstColor => obstacleBurstColor;

        public float ObstacleBurstGravity => obstacleBurstGravity;

        public int ObstacleBurstMaxParticles => Mathf.Max(0, obstacleBurstMaxParticles);

        public float ObstacleBurstMinimumShapeRadius => Mathf.Max(0f, obstacleBurstMinimumShapeRadius);

        public float ObstacleBurstShapeRadiusMultiplier => Mathf.Max(0f, obstacleBurstShapeRadiusMultiplier);

        public float ObstacleBurstParticleMultiplier => Mathf.Max(0f, obstacleBurstParticleMultiplier);

        public int ObstacleBurstMinimumParticleCount => Mathf.Max(0, obstacleBurstMinimumParticleCount);

        public int ObstacleBurstMaximumParticleCount => Mathf.Max(ObstacleBurstMinimumParticleCount, obstacleBurstMaximumParticleCount);

        public float ObstacleBurstObjectLifetime => Mathf.Max(0f, obstacleBurstObjectLifetime);

        public float ImpactBurstHeight => impactBurstHeight;

        public float ImpactBurstLifetime => Mathf.Max(0f, impactBurstLifetime);

        public float ImpactBurstMinimumSpeed => Mathf.Max(0f, impactBurstMinimumSpeed);

        public float ImpactBurstSpeedMultiplier => Mathf.Max(0f, impactBurstSpeedMultiplier);

        public float ImpactBurstMinimumSize => Mathf.Max(0f, impactBurstMinimumSize);

        public float ImpactBurstSizeMultiplier => Mathf.Max(0f, impactBurstSizeMultiplier);

        public Color ImpactBurstColor => impactBurstColor;

        public float ImpactBurstGravity => impactBurstGravity;

        public int ImpactBurstMaxParticles => Mathf.Max(0, impactBurstMaxParticles);

        public float ImpactBurstMinimumShapeRadius => Mathf.Max(0f, impactBurstMinimumShapeRadius);

        public float ImpactBurstShapeRadiusMultiplier => Mathf.Max(0f, impactBurstShapeRadiusMultiplier);

        public float ImpactBurstParticleMultiplier => Mathf.Max(0f, impactBurstParticleMultiplier);

        public int ImpactBurstMinimumParticleCount => Mathf.Max(0, impactBurstMinimumParticleCount);

        public int ImpactBurstMaximumParticleCount => Mathf.Max(ImpactBurstMinimumParticleCount, impactBurstMaximumParticleCount);

        public float ImpactBurstObjectLifetime => Mathf.Max(0f, impactBurstObjectLifetime);

        public float ObstacleClearFlashDuration => Mathf.Max(0f, obstacleClearFlashDuration);

        public float ObstacleClearShrinkDuration => Mathf.Max(0.01f, obstacleClearShrinkDuration);

        public Color ObstacleVisualBaseColor => obstacleVisualBaseColor;

        public Color ObstacleVisualAccentColor => obstacleVisualAccentColor;

        public float ObstacleVisualColorVariation => Mathf.Clamp01(obstacleVisualColorVariation);

        public float ObstacleVisualHeightVariation => Mathf.Max(0f, obstacleVisualHeightVariation);

        public GameObject ProjectilePrefab => projectilePrefab;

        public GameObject InfectionPreviewPrefab => infectionPreviewPrefab;

        public GameObject ProjectileBurstPrefab => projectileBurstPrefab;

        public GameObject ObstacleBurstPrefab => obstacleBurstPrefab;

        public GameObject ImpactBurstPrefab => impactBurstPrefab;

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

            if (projectilePrefab == null || infectionPreviewPrefab == null || projectileBurstPrefab == null || obstacleBurstPrefab == null || impactBurstPrefab == null)
            {
                throw new System.InvalidOperationException($"{nameof(GameplayConfiguration)} has missing transient prefabs.");
            }
        }
    }
}
