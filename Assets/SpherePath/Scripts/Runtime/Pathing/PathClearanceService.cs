using System.Collections.Generic;
using SpherePath.Obstacles;
using UnityEngine;

namespace SpherePath.Pathing
{
    public sealed class PathClearanceService
    {
        private readonly IReadOnlyList<Obstacle> _obstacles;
        private readonly float _safePadding;

        public PathClearanceService(IReadOnlyList<Obstacle> obstacles, float safePadding)
        {
            _obstacles = obstacles;
            _safePadding = Mathf.Max(0f, safePadding);
        }

        public Vector3 GetReachablePosition(Vector3 start, Vector3 target, float playerRadius)
        {
            var direction = (target - start).normalized;
            var bestDistance = Vector3.Distance(start, target);
            var corridorRadius = Mathf.Max(0f, playerRadius);

            foreach (var obstacle in _obstacles)
            {
                if (obstacle == null || obstacle.IsCleared)
                {
                    continue;
                }

                var toObstacle = obstacle.Position - start;
                var forwardDistance = Vector3.Dot(toObstacle, direction);
                if (forwardDistance <= 0f || forwardDistance >= bestDistance)
                {
                    continue;
                }

                var closestPoint = start + direction * forwardDistance;
                var lateralDistance = Vector3.Distance(closestPoint, obstacle.Position);
                if (lateralDistance <= corridorRadius + obstacle.Radius + _safePadding)
                {
                    bestDistance = Mathf.Max(0f, forwardDistance - corridorRadius - obstacle.Radius - _safePadding);
                }
            }

            return start + direction * bestDistance;
        }
    }
}
