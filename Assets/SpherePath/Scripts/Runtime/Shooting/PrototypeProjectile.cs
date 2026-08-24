using System;
using System.Collections.Generic;
using SpherePath.Obstacles;
using UnityEngine;

namespace SpherePath.Shooting
{
    public sealed class PrototypeProjectile : MonoBehaviour
    {
        private IReadOnlyList<PrototypeObstacle> _obstacles;
        private Vector3 _direction;
        private float _radius;
        private float _speed;
        private float _lifeTime;

        public event Action<PrototypeObstacle, float> HitObstacle;
        public event Action Expired;

        public void Launch(IReadOnlyList<PrototypeObstacle> obstacles, Vector3 direction, float radius, float speed, float lifeTime)
        {
            _obstacles = obstacles;
            _direction = direction.normalized;
            _radius = radius;
            _speed = speed;
            _lifeTime = lifeTime;
            transform.localScale = Vector3.one * (_radius * 2f);
        }

        private void Update()
        {
            transform.position += _direction * (_speed * Time.deltaTime);
            _lifeTime -= Time.deltaTime;

            var hitObstacle = FindHitObstacle();
            if (hitObstacle != null)
            {
                HitObstacle?.Invoke(hitObstacle, _radius);
                Destroy(gameObject);
                return;
            }

            if (_lifeTime <= 0f)
            {
                Expired?.Invoke();
                Destroy(gameObject);
            }
        }

        private PrototypeObstacle FindHitObstacle()
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
