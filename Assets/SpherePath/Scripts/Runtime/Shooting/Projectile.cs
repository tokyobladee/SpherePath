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
        private bool _isActive;

        public event Action<Obstacle, float> HitObstacle;
        public event Action Expired;

        public void Launch(IReadOnlyList<Obstacle> obstacles, Vector3 direction, float radius, float speed, float lifeTime, float maxTravelDistance)
        {
            _obstacles = obstacles;
            _direction = direction.normalized;
            _radius = radius;
            _speed = speed;
            _lifeTime = lifeTime;
            _travelDistance = 0f;
            _maxTravelDistance = Mathf.Max(0f, maxTravelDistance);
            _isActive = true;
            transform.localScale = Vector3.one * (_radius * 2f);
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

            var stepDistance = _speed * Time.deltaTime;
            transform.position += _direction * stepDistance;
            _travelDistance += stepDistance;
            _lifeTime -= Time.deltaTime;

            var hitObstacle = FindHitObstacle();
            if (hitObstacle != null)
            {
                _isActive = false;
                HitObstacle?.Invoke(hitObstacle, _radius);
                Destroy(gameObject);
                return;
            }

            if (_lifeTime <= 0f || _travelDistance >= _maxTravelDistance)
            {
                _isActive = false;
                Expired?.Invoke();
                Destroy(gameObject);
            }
        }

        private Obstacle FindHitObstacle()
        {
            if (_obstacles == null)
            {
                return null;
            }

            foreach (var obstacle in _obstacles)
            {
                if (obstacle == null || obstacle.IsCleared)
                {
                    continue;
                }

                var distance = Vector3.Distance(transform.position, obstacle.Position);
                if (distance <= _radius + obstacle.Radius)
                {
                    return obstacle;
                }
            }

            return null;
        }
    }
}
