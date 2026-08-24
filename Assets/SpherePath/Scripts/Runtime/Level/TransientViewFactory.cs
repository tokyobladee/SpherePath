using System;
using System.Collections.Generic;
using SpherePath.Configuration;
using SpherePath.Shooting;
using SpherePath.VFX;
using UnityEngine;

namespace SpherePath.Level
{
    public sealed class TransientViewFactory
    {
        private readonly List<GameObject> _transientObjects = new List<GameObject>();
        private readonly ProjectileViewFactory _projectileFactory;
        private readonly GameplayVfxFactory _vfxFactory;

        public TransientViewFactory(GameplayConfiguration configuration, LevelViewReferences level)
        {
            _projectileFactory = new ProjectileViewFactory(configuration, level, Track);
            _vfxFactory = new GameplayVfxFactory(configuration, level, Track);
        }

        public Projectile CreateProjectile(Vector3 position, float radius)
        {
            return _projectileFactory.CreateProjectile(position, radius);
        }

        public void ShowInfectionRadius(Vector3 center, float radius)
        {
            _vfxFactory.ShowInfectionRadius(center, radius);
        }

        public void ShowProjectileBurst(Vector3 center, float radius)
        {
            _vfxFactory.ShowProjectileBurst(center, radius);
        }

        public void ShowObstacleBurst(Vector3 center, float radius)
        {
            _vfxFactory.ShowObstacleBurst(center, radius);
        }

        public void ClearTransients()
        {
            for (var i = _transientObjects.Count - 1; i >= 0; i--)
            {
                var transientObject = _transientObjects[i];
                _transientObjects.RemoveAt(i);

                if (transientObject == null)
                {
                    continue;
                }

                var projectile = transientObject.GetComponent<Projectile>();
                if (projectile != null)
                {
                    projectile.Cancel();
                    continue;
                }

                UnityEngine.Object.Destroy(transientObject);
            }
        }

        private void Track(GameObject transientObject)
        {
            _transientObjects.Add(transientObject);
        }
    }

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
            var projectileObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            projectileObject.name = "Projectile";
            projectileObject.transform.position = position;
            projectileObject.GetComponent<Renderer>().sharedMaterial = _level.ProjectileMaterial;
            UnityEngine.Object.Destroy(projectileObject.GetComponent<Collider>());
            CreateTrail(projectileObject, radius);
            var projectile = projectileObject.AddComponent<Projectile>();
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

        private void CreateTrail(GameObject projectileObject, float radius)
        {
            var trail = projectileObject.AddComponent<TrailRenderer>();
            trail.sharedMaterial = _level.TrailMaterial;
            trail.time = _configuration.ProjectileTrailTime;
            trail.startWidth = radius * _configuration.ProjectileTrailWidthMultiplier;
            trail.endWidth = 0f;
            trail.minVertexDistance = _configuration.ProjectileTrailMinVertexDistance;
            trail.numCornerVertices = _configuration.ProjectileTrailCornerVertices;
        }

        private float GetTravelDistance(Vector3 startPosition, float radius)
        {
            var distanceToDoor = Vector3.Dot(_level.DoorPosition - startPosition, Vector3.forward);
            return Mathf.Max(radius, distanceToDoor + radius + _configuration.ProjectileExitDistance);
        }
    }

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

            var previewObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            previewObject.name = "Infection Radius Preview";
            previewObject.transform.position = new Vector3(center.x, _configuration.InfectionPreviewGroundOffset, center.z);
            previewObject.transform.localScale = new Vector3(radius * 2f, _configuration.InfectionPreviewHeight, radius * 2f);
            UnityEngine.Object.Destroy(previewObject.GetComponent<Collider>());
            previewObject.GetComponent<Renderer>().sharedMaterial = _level.InfectionPreviewMaterial;
            previewObject.AddComponent<TimedSelfDestroy>().SetLifeTime(_configuration.InfectionPreviewLifetime);
            _track(previewObject);
            ShowImpactBurst(center, radius);
        }

        public void ShowProjectileBurst(Vector3 center, float radius)
        {
            var particles = CreateBurst("Projectile Burst", center);
            var main = particles.main;
            main.startLifetime = _configuration.ProjectileBurstLifetime;
            main.startSpeed = Mathf.Max(_configuration.ProjectileBurstMinimumSpeed, radius * _configuration.ProjectileBurstSpeedMultiplier);
            main.startSize = Mathf.Max(_configuration.ProjectileBurstMinimumSize, radius * _configuration.ProjectileBurstSizeMultiplier);
            main.startColor = _configuration.ProjectileBurstColor;
            main.gravityModifier = _configuration.ProjectileBurstGravity;
            main.maxParticles = _configuration.ProjectileBurstMaxParticles;
            ConfigureShape(particles, Mathf.Max(_configuration.ProjectileBurstMinimumShapeRadius, radius * _configuration.ProjectileBurstShapeRadiusMultiplier));
            particles.Emit(Mathf.Clamp(Mathf.RoundToInt(radius * _configuration.ProjectileBurstParticleMultiplier), _configuration.ProjectileBurstMinimumParticleCount, _configuration.ProjectileBurstMaximumParticleCount));
            particles.gameObject.AddComponent<TimedSelfDestroy>().SetLifeTime(_configuration.ProjectileBurstObjectLifetime);
        }

        public void ShowObstacleBurst(Vector3 center, float radius)
        {
            var particles = CreateBurst("Obstacle Burst", center);
            var main = particles.main;
            main.startLifetime = _configuration.ObstacleBurstLifetime;
            main.startSpeed = Mathf.Max(_configuration.ObstacleBurstMinimumSpeed, radius * _configuration.ObstacleBurstSpeedMultiplier);
            main.startSize = Mathf.Max(_configuration.ObstacleBurstMinimumSize, radius * _configuration.ObstacleBurstSizeMultiplier);
            main.startColor = _configuration.ObstacleBurstColor;
            main.gravityModifier = _configuration.ObstacleBurstGravity;
            main.maxParticles = _configuration.ObstacleBurstMaxParticles;
            ConfigureShape(particles, Mathf.Max(_configuration.ObstacleBurstMinimumShapeRadius, radius * _configuration.ObstacleBurstShapeRadiusMultiplier));
            particles.Emit(Mathf.Clamp(Mathf.RoundToInt(radius * _configuration.ObstacleBurstParticleMultiplier), _configuration.ObstacleBurstMinimumParticleCount, _configuration.ObstacleBurstMaximumParticleCount));
            particles.gameObject.AddComponent<TimedSelfDestroy>().SetLifeTime(_configuration.ObstacleBurstObjectLifetime);
        }

        private void ShowImpactBurst(Vector3 center, float radius)
        {
            var particles = CreateBurst("Impact Burst", new Vector3(center.x, _configuration.ImpactBurstHeight, center.z));
            var main = particles.main;
            main.startLifetime = _configuration.ImpactBurstLifetime;
            main.startSpeed = Mathf.Max(_configuration.ImpactBurstMinimumSpeed, radius * _configuration.ImpactBurstSpeedMultiplier);
            main.startSize = Mathf.Max(_configuration.ImpactBurstMinimumSize, radius * _configuration.ImpactBurstSizeMultiplier);
            main.maxParticles = _configuration.ImpactBurstMaxParticles;
            ConfigureShape(particles, Mathf.Max(_configuration.ImpactBurstMinimumShapeRadius, radius * _configuration.ImpactBurstShapeRadiusMultiplier));
            particles.Emit(Mathf.Clamp(Mathf.RoundToInt(radius * _configuration.ImpactBurstParticleMultiplier), _configuration.ImpactBurstMinimumParticleCount, _configuration.ImpactBurstMaximumParticleCount));
            particles.gameObject.AddComponent<TimedSelfDestroy>().SetLifeTime(_configuration.ImpactBurstObjectLifetime);
        }

        private ParticleSystem CreateBurst(string name, Vector3 position)
        {
            var burstObject = new GameObject(name);
            _track(burstObject);
            burstObject.transform.position = position;
            var particles = burstObject.AddComponent<ParticleSystem>();
            var main = particles.main;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            var emission = particles.emission;
            emission.enabled = false;
            var renderer = particles.GetComponent<ParticleSystemRenderer>();
            renderer.sharedMaterial = _level.TrailMaterial;
            return particles;
        }

        private static void ConfigureShape(ParticleSystem particles, float radius)
        {
            var shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = radius;
        }
    }
}
