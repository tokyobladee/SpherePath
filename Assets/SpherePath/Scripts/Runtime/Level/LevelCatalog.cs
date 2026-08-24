using System.Collections.Generic;
using UnityEngine;

namespace SpherePath.Level
{
    [CreateAssetMenu(fileName = "LevelCatalog", menuName = "SpherePath/Level Catalog")]
    public sealed class LevelCatalog : ScriptableObject
    {
        [SerializeField] private List<LevelViewReferences> levels = new List<LevelViewReferences>();

        public int Count => levels.Count;

        public LevelViewReferences GetLevel(int index)
        {
            if (levels.Count == 0)
            {
                throw new System.InvalidOperationException($"{nameof(LevelCatalog)} has no levels.");
            }

            var clampedIndex = Mathf.Clamp(index, 0, levels.Count - 1);
            var level = levels[clampedIndex];
            if (level == null)
            {
                throw new System.InvalidOperationException($"{nameof(LevelCatalog)} has a missing level at index {clampedIndex}.");
            }

            return level;
        }

        public void Validate()
        {
            if (levels.Count == 0)
            {
                throw new System.InvalidOperationException($"{nameof(LevelCatalog)} requires at least one level.");
            }

            for (var i = 0; i < levels.Count; i++)
            {
                if (levels[i] == null)
                {
                    throw new System.InvalidOperationException($"{nameof(LevelCatalog)} has a missing level at index {i}.");
                }
            }
        }
    }
}
