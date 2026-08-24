using System.Collections.Generic;
using System;
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
            _vfxFactory = new GameplayVfxFactory(level, Track);
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
                GetTravelDistance(position, radius));
            _track(projectileObject);
            return projectile;
        }

        private void CreateTrail(GameObject projectileObject, float radius)
        {
            var trail = projectileObject.AddComponent<TrailRenderer>();
            trail.sharedMaterial = _level.TrailMaterial;
            trail.time = 0.32f;
            trail.startWidth = radius * 1.55f;
            trail.endWidth = 0f;
            trail.minVertexDistance = 0.02f;
            trail.numCornerVertices = 8;
        }

        private float GetTravelDistance(Vector3 startPosition, float radius)
        {
            var distanceToDoor = Vector3.Dot(_level.DoorPosition - startPosition, Vector3.forward);
            return Mathf.Max(radius, distanceToDoor + radius + _configuration.ProjectileExitDistance);
        }
    }

    public sealed class GameplayVfxFactory
    {
        private readonly LevelViewReferences _level;
        private readonly Action<GameObject> _track;

        public GameplayVfxFactory(LevelViewReferences level, Action<GameObject> track)
        {
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
            previewObject.transform.position = new Vector3(center.x, 0.04f, center.z);
            previewObject.transform.localScale = new Vector3(radius * 2f, 0.02f, radius * 2f);
            UnityEngine.Object.Destroy(previewObject.GetComponent<Collider>());
            previewObject.GetComponent<Renderer>().sharedMaterial = _level.InfectionPreviewMaterial;
            previewObject.AddComponent<TimedSelfDestroy>().SetLifeTime(0.35f);
            _track(previewObject);
            ShowImpactBurst(center, radius);
        }

        public void ShowProjectileBurst(Vector3 center, float radius)
        {
            var particles = CreateBurst("Projectile Burst", center);
            var main = particles.main;
            main.startLifetime = 0.42f;
            main.startSpeed = Mathf.Max(4.5f, radius * 7f);
            main.startSize = Mathf.Max(0.16f, radius * 0.24f);
            main.startColor = new Color(1f, 0.88f, 0.25f, 1f);
            main.gravityModifier = 0.25f;
            main.maxParticles = 96;
            ConfigureShape(particles, Mathf.Max(0.08f, radius * 0.35f));
            particles.Emit(Mathf.Clamp(Mathf.RoundToInt(radius * 48f), 24, 72));
            particles.gameObject.AddComponent<TimedSelfDestroy>().SetLifeTime(0.9f);
        }

        public void ShowObstacleBurst(Vector3 center, float radius)
        {
            var particles = CreateBurst("Obstacle Burst", center);
            var main = particles.main;
            main.startLifetime = 0.55f;
            main.startSpeed = Mathf.Max(2f, radius * 4.5f);
            main.startSize = Mathf.Max(0.12f, radius * 0.28f);
            main.startColor = new Color(0.48f, 0.9f, 0.42f, 1f);
            main.gravityModifier = 1.1f;
            main.maxParticles = 64;
            ConfigureShape(particles, Mathf.Max(0.1f, radius * 0.3f));
            particles.Emit(Mathf.Clamp(Mathf.RoundToInt(radius * 42f), 18, 56));
            particles.gameObject.AddComponent<TimedSelfDestroy>().SetLifeTime(1f);
        }

        private void ShowImpactBurst(Vector3 center, float radius)
        {
            var particles = CreateBurst("Impact Burst", new Vector3(center.x, 0.4f, center.z));
            var main = particles.main;
            main.startLifetime = 0.35f;
            main.startSpeed = Mathf.Max(2f, radius * 2f);
            main.startSize = Mathf.Max(0.12f, radius * 0.18f);
            main.maxParticles = 64;
            ConfigureShape(particles, Mathf.Max(0.1f, radius * 0.2f));
            particles.Emit(Mathf.Clamp(Mathf.RoundToInt(radius * 22f), 12, 48));
            particles.gameObject.AddComponent<TimedSelfDestroy>().SetLifeTime(0.8f);
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
