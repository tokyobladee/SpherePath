using System.Collections.Generic;
using SpherePath.Obstacles;
using UnityEngine;

namespace SpherePath.Pathing
{
    public sealed class PathClearanceService
    {
        private readonly IReadOnlyList<Obstacle> _obstacles;
        private readonly float _lateralClearance;
        private readonly float _stopDistance;

        public PathClearanceService(IReadOnlyList<Obstacle> obstacles, float lateralClearance, float stopDistance)
        {
            _obstacles = obstacles;
            _lateralClearance = Mathf.Max(0f, lateralClearance);
            _stopDistance = Mathf.Max(0f, stopDistance);
        }

        public Vector3 GetReachablePosition(Vector3 start, Vector3 target, float playerRadius)
        {
            var startFlat = new Vector3(start.x, 0f, start.z);
            var targetFlat = new Vector3(target.x, 0f, target.z);
            var direction = (targetFlat - startFlat).normalized;
            var bestDistance = Vector3.Distance(startFlat, targetFlat);
            var corridorRadius = Mathf.Max(0f, playerRadius);

            foreach (var obstacle in _obstacles)
            {
                if (obstacle == null || obstacle.IsCleared)
                {
                    continue;
                }

                var obstaclePosition = new Vector3(obstacle.Position.x, 0f, obstacle.Position.z);
                var toObstacle = obstaclePosition - startFlat;
                var forwardDistance = Vector3.Dot(toObstacle, direction);
                if (forwardDistance <= 0f || forwardDistance >= bestDistance)
                {
                    continue;
                }

                var closestPoint = startFlat + direction * forwardDistance;
                var lateralDistance = Vector3.Distance(closestPoint, obstaclePosition);
                if (lateralDistance <= corridorRadius + obstacle.Radius + _lateralClearance)
                {
                    bestDistance = Mathf.Max(0f, forwardDistance - corridorRadius - obstacle.Radius - _stopDistance);
                }
            }

            var reachablePosition = startFlat + direction * bestDistance;
            return new Vector3(reachablePosition.x, start.y, reachablePosition.z);
        }
    }
}
