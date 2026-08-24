using UnityEngine;

namespace SpherePath.Level
{
    public sealed class LevelLoader
    {
        private readonly LevelCatalog _catalog;

        public LevelLoader(LevelCatalog catalog)
        {
            _catalog = catalog;
        }

        public LevelViewReferences Load(int levelIndex, Transform parent)
        {
            var prefab = _catalog.GetLevel(levelIndex);
            var level = prefab.gameObject.scene.IsValid()
                ? prefab
                : Object.Instantiate(prefab, parent);
            level.Validate();
            return level;
        }
    }
}
