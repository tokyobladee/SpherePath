using System;
using System.Collections.Generic;
using SpherePath.Obstacles;
using UnityEngine;

namespace SpherePath.Shooting
{
    public sealed class Projectile : MonoBehaviour
    {
        private IReadOnlyList<Obstacle> _obstacles;
        private Vector3 _direction;
        private float _radius;
        private float _speed;
        private float _lifeTime;
        private float _travelDistance;
        private float _maxTravelDistance;
        private float _launchTime;
        private float _liquidFrequency;
        private float _liquidForwardStretch;
        private float _liquidSideSquash;
        private bool _isActive;

        public event Action<Obstacle, float, Vector3> HitObstacle;
        public event Action<Vector3, float> Expired;

        public void Launch(
            IReadOnlyList<Obstacle> obstacles,
            Vector3 direction,
            float radius,
            float speed,
            float lifeTime,
            float maxTravelDistance,
            float liquidFrequency,
            float liquidForwardStretch,
            float liquidSideSquash)
        {
            _obstacles = obstacles;
            _direction = direction.normalized;
            _radius = radius;
            _speed = speed;
            _lifeTime = lifeTime;
            _travelDistance = 0f;
            _maxTravelDistance = Mathf.Max(0f, maxTravelDistance);
            _launchTime = Time.time;
            _liquidFrequency = liquidFrequency;
            _liquidForwardStretch = liquidForwardStretch;
            _liquidSideSquash = liquidSideSquash;
            _isActive = true;
            UpdateLiquidScale();
        }

        public void Cancel()
        {
            _isActive = false;
            HitObstacle = null;
            Expired = null;
            Destroy(gameObject);
        }

        private void Update()
        {
            if (!_isActive)
            {
                return;
            }

            var startPosition = transform.position;
            var stepDistance = _speed * Time.deltaTime;
            var targetPosition = startPosition + _direction * stepDistance;
            transform.position = targetPosition;
            _travelDistance += stepDistance;
            _lifeTime -= Time.deltaTime;
            UpdateLiquidScale();

            var hitObstacle = FindHitObstacle(startPosition, targetPosition, out var hitPosition);
            if (hitObstacle != null)
            {
                _isActive = false;
                transform.position = hitPosition;
                HitObstacle?.Invoke(hitObstacle, _radius, hitPosition);
                Destroy(gameObject);
                return;
            }

            if (_lifeTime <= 0f || _travelDistance >= _maxTravelDistance)
            {
                _isActive = false;
                Expired?.Invoke(transform.position, _radius);
                Destroy(gameObject);
            }
        }

        private Obstacle FindHitObstacle(Vector3 startPosition, Vector3 targetPosition, out Vector3 hitPosition)
        {
            hitPosition = targetPosition;

            if (_obstacles == null)
            {
                return null;
            }

            var flatStartPosition = new Vector3(startPosition.x, 0f, startPosition.z);
            var flatTargetPosition = new Vector3(targetPosition.x, 0f, targetPosition.z);
            var segment = flatTargetPosition - flatStartPosition;
            var segmentLengthSquared = segment.sqrMagnitude;
            var bestProgress = float.MaxValue;
            Obstacle bestObstacle = null;

            foreach (var obstacle in _obstacles)
            {
                if (obstacle == null || obstacle.IsCleared)
                {
                    continue;
                }

                var obstaclePosition = new Vector3(obstacle.Position.x, 0f, obstacle.Position.z);
                var progress = segmentLengthSquared <= Mathf.Epsilon
                    ? 0f
                    : Mathf.Clamp01(Vector3.Dot(obstaclePosition - flatStartPosition, segment) / segmentLengthSquared);
                var closestPosition = flatStartPosition + segment * progress;
                var distance = Vector3.Distance(closestPosition, obstaclePosition);
                if (distance <= _radius + obstacle.Radius)
                {
                    if (progress >= bestProgress)
                    {
                        continue;
                    }

                    bestProgress = progress;
                    bestObstacle = obstacle;
                    hitPosition = Vector3.Lerp(startPosition, targetPosition, progress);
                }
            }

            return bestObstacle;
        }

        private void UpdateLiquidScale()
        {
            var diameter = _radius * 2f;
            var time = (Time.time - _launchTime) * _liquidFrequency;
            var forwardStretch = 1f + Mathf.Sin(time) * _liquidForwardStretch;
            var sideSquash = 1f - Mathf.Sin(time) * _liquidSideSquash;
            transform.localScale = new Vector3(diameter * sideSquash, diameter * sideSquash, diameter * forwardStretch);
        }
    }
}
