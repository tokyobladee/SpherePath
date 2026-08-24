using System.Collections.Generic;
using UnityEngine;

namespace SpherePath.Obstacles
{
    public sealed class ObstacleClearingService
    {
        private readonly float _infectionRadiusMultiplier;
        private readonly float _impactRadius;

        public ObstacleClearingService(float infectionRadiusMultiplier, float impactRadius)
        {
            _infectionRadiusMultiplier = Mathf.Max(0.01f, infectionRadiusMultiplier);
            _impactRadius = Mathf.Max(0f, impactRadius);
        }

        public ObstacleClearingResult ClearFromImpact(IReadOnlyList<Obstacle> obstacles, Obstacle impactObstacle, Vector3 impactPosition, float projectileRadius)
        {
            if (obstacles == null || impactObstacle == null)
            {
                return new ObstacleClearingResult(0, 0f, impactPosition);
            }

            var infectionRadius = Mathf.Max(_impactRadius, Mathf.Max(0f, projectileRadius) * _infectionRadiusMultiplier);
            var clearedCount = 0;

            foreach (var obstacle in obstacles)
            {
                if (obstacle == null || obstacle.IsCleared)
                {
                    continue;
                }

                if (GetFlatDistance(impactPosition, obstacle.Position) <= infectionRadius + obstacle.Radius)
                {
                    obstacle.Clear();
                    clearedCount++;
                }
            }

            return new ObstacleClearingResult(clearedCount, infectionRadius, impactPosition);
        }

        private static float GetFlatDistance(Vector3 first, Vector3 second)
        {
            return Vector3.Distance(new Vector3(first.x, 0f, first.z), new Vector3(second.x, 0f, second.z));
        }
    }
}
