using UnityEngine;

namespace SpherePath.Obstacles
{
    public readonly struct ObstacleClearingResult
    {
        public ObstacleClearingResult(int clearedCount, float infectionRadius, Vector3 impactPosition)
        {
            ClearedCount = clearedCount;
            InfectionRadius = infectionRadius;
            ImpactPosition = impactPosition;
        }

        public int ClearedCount { get; }

        public float InfectionRadius { get; }

        public Vector3 ImpactPosition { get; }
    }
}
