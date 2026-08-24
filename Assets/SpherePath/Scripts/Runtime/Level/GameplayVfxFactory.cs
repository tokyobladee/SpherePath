using System;
using SpherePath.Configuration;
using SpherePath.VFX;
using UnityEngine;

namespace SpherePath.Level
{
    public sealed class GameplayVfxFactory
    {
        private readonly GameplayConfiguration _configuration;
        private readonly LevelViewReferences _level;
        private readonly Action<GameObject> _track;

        public GameplayVfxFactory(GameplayConfiguration configuration, LevelViewReferences level, Action<GameObject> track)
        {
            _configuration = configuration;
            _level = level;
            _track = track;
        }

        public void ShowInfectionRadius(Vector3 center, float radius)
        {
            if (radius <= 0f)
            {
                return;
            }

            var previewObject = UnityEngine.Object.Instantiate(_configuration.InfectionPreviewPrefab);
            previewObject.transform.position = new Vector3(center.x, _configuration.InfectionPreviewGroundOffset, center.z);
            previewObject.transform.localScale = new Vector3(radius * 2f, _configuration.InfectionPreviewHeight, radius * 2f);
            SetRendererMaterial(previewObject, _level.InfectionPreviewMaterial);
            UnityEngine.Object.Destroy(previewObject, _configuration.InfectionPreviewLifetime);
            _track(previewObject);
            ShowImpactBurst(center, radius);
        }

        public void ShowProjectileBurst(Vector3 center, float radius)
        {
            ShowBurst(
                _configuration.ProjectileBurstPrefab,
                center,
                _configuration.ProjectileBurstColor,
                _configuration.ProjectileBurstLifetime,
                Mathf.Max(_configuration.ProjectileBurstMinimumSpeed, radius * _configuration.ProjectileBurstSpeedMultiplier),
                Mathf.Max(_configuration.ProjectileBurstMinimumSize, radius * _configuration.ProjectileBurstSizeMultiplier),
                _configuration.ProjectileBurstGravity,
                _configuration.ProjectileBurstMaxParticles,
                Mathf.Max(_configuration.ProjectileBurstMinimumShapeRadius, radius * _configuration.ProjectileBurstShapeRadiusMultiplier),
                Mathf.Clamp(Mathf.RoundToInt(radius * _configuration.ProjectileBurstParticleMultiplier), _configuration.ProjectileBurstMinimumParticleCount, _configuration.ProjectileBurstMaximumParticleCount),
                _configuration.ProjectileBurstObjectLifetime);
        }

        public void ShowObstacleBurst(Vector3 center, float radius)
        {
            ShowBurst(
                _configuration.ObstacleBurstPrefab,
                center,
                _configuration.ObstacleBurstColor,
                _configuration.ObstacleBurstLifetime,
                Mathf.Max(_configuration.ObstacleBurstMinimumSpeed, radius * _configuration.ObstacleBurstSpeedMultiplier),
                Mathf.Max(_configuration.ObstacleBurstMinimumSize, radius * _configuration.ObstacleBurstSizeMultiplier),
                _configuration.ObstacleBurstGravity,
                _configuration.ObstacleBurstMaxParticles,
                Mathf.Max(_configuration.ObstacleBurstMinimumShapeRadius, radius * _configuration.ObstacleBurstShapeRadiusMultiplier),
                Mathf.Clamp(Mathf.RoundToInt(radius * _configuration.ObstacleBurstParticleMultiplier), _configuration.ObstacleBurstMinimumParticleCount, _configuration.ObstacleBurstMaximumParticleCount),
                _configuration.ObstacleBurstObjectLifetime);
        }

        private void ShowImpactBurst(Vector3 center, float radius)
        {
            ShowBurst(
                _configuration.ImpactBurstPrefab,
                new Vector3(center.x, _configuration.ImpactBurstHeight, center.z),
                _configuration.ImpactBurstColor,
                _configuration.ImpactBurstLifetime,
                Mathf.Max(_configuration.ImpactBurstMinimumSpeed, radius * _configuration.ImpactBurstSpeedMultiplier),
                Mathf.Max(_configuration.ImpactBurstMinimumSize, radius * _configuration.ImpactBurstSizeMultiplier),
                _configuration.ImpactBurstGravity,
                _configuration.ImpactBurstMaxParticles,
                Mathf.Max(_configuration.ImpactBurstMinimumShapeRadius, radius * _configuration.ImpactBurstShapeRadiusMultiplier),
                Mathf.Clamp(Mathf.RoundToInt(radius * _configuration.ImpactBurstParticleMultiplier), _configuration.ImpactBurstMinimumParticleCount, _configuration.ImpactBurstMaximumParticleCount),
                _configuration.ImpactBurstObjectLifetime);
        }

        private void ShowBurst(GameObject prefab, Vector3 position, Color color, float lifetime, float speed, float size, float gravity, int maxParticles, float shapeRadius, int particleCount, float objectLifetime)
        {
            var burstObject = UnityEngine.Object.Instantiate(prefab);
            _track(burstObject);
            var burst = GetRequiredComponent<ParticleBurstView>(burstObject);
            burst.Play(position, _level.TrailMaterial, color, lifetime, speed, size, gravity, maxParticles, shapeRadius, particleCount, objectLifetime);
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
