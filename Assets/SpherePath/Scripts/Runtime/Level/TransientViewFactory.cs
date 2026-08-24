using System.Collections.Generic;
using SpherePath.Configuration;
using SpherePath.Shooting;
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
}
