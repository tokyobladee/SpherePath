using System;
using SpherePath.Configuration;
using SpherePath.Shooting;
using UnityEngine;

namespace SpherePath.Level
{
    public sealed class ProjectileViewFactory
    {
        private readonly GameplayConfiguration _configuration;
        private readonly LevelViewReferences _level;
        private readonly Action<GameObject> _track;

        public ProjectileViewFactory(GameplayConfiguration configuration, LevelViewReferences level, Action<GameObject> track)
        {
            _configuration = configuration;
            _level = level;
            _track = track;
        }

        public Projectile CreateProjectile(Vector3 position, float radius)
        {
            var projectileObject = UnityEngine.Object.Instantiate(_configuration.ProjectilePrefab);
            projectileObject.transform.position = position;
            projectileObject.transform.rotation = Quaternion.identity;
            SetRendererMaterial(projectileObject, _level.ProjectileMaterial);
            ConfigureTrail(projectileObject, radius);
            var projectile = GetRequiredComponent<Projectile>(projectileObject);
            projectile.Launch(
                _level.Obstacles,
                Vector3.forward,
                radius,
                _configuration.ProjectileSpeed,
                _configuration.ProjectileLifeTime,
                GetTravelDistance(position, radius),
                _configuration.ProjectileLiquidFrequency,
                _configuration.ProjectileLiquidForwardStretch,
                _configuration.ProjectileLiquidSideSquash);
            _track(projectileObject);
            return projectile;
        }

        private void ConfigureTrail(GameObject projectileObject, float radius)
        {
            var trail = GetRequiredComponent<ProjectileTrailView>(projectileObject);
            trail.Configure(
                _level.TrailMaterial,
                _configuration.ProjectileTrailTime,
                radius * _configuration.ProjectileTrailWidthMultiplier,
                _configuration.ProjectileTrailMinVertexDistance);
        }

        private float GetTravelDistance(Vector3 startPosition, float radius)
        {
            var distanceToDoor = Vector3.Dot(_level.DoorPosition - startPosition, Vector3.forward);
            return Mathf.Max(radius, distanceToDoor + radius + _configuration.ProjectileExitDistance);
        }

        private static void SetRendererMaterial(GameObject target, Material material)
        {
            var targetRenderer = GetRequiredComponent<Renderer>(target);
            targetRenderer.sharedMaterial = material;
        }

        private static T GetRequiredComponent<T>(GameObject target) where T : Component
        {
            var component = target.GetComponent<T>();
            if (component == null)
            {
                throw new InvalidOperationException($"{target.name} prefab requires {typeof(T).Name}.");
            }

            return component;
        }
    }
}
