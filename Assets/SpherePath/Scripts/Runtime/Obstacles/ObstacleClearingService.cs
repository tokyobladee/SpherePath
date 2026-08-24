using System.Collections.Generic;
using UnityEngine;

namespace SpherePath.Obstacles
{
    public sealed class ObstacleClearingService
    {
        private readonly float _infectionRadiusMultiplier;

        public ObstacleClearingService(float infectionRadiusMultiplier)
        {
            _infectionRadiusMultiplier = Mathf.Max(0.01f, infectionRadiusMultiplier);
        }

        public ObstacleClearingResult ClearFromImpact(IReadOnlyList<PrototypeObstacle> obstacles, PrototypeObstacle impactObstacle, float projectileRadius)
        {
            if (obstacles == null || impactObstacle == null)
            {
                return new ObstacleClearingResult(0, 0f);
            }

            var infectionRadius = Mathf.Max(0f, projectileRadius) * _infectionRadiusMultiplier;
            var clearedCount = 0;

            foreach (var obstacle in obstacles)
            {
                if (obstacle == null || obstacle.IsCleared)
                {
                    continue;
                }

                if (Vector3.Distance(impactObstacle.Position, obstacle.Position) <= infectionRadius)
                {
                    obstacle.Clear();
                    clearedCount++;
                }
            }

            return new ObstacleClearingResult(clearedCount, infectionRadius);
        }
    }
}
