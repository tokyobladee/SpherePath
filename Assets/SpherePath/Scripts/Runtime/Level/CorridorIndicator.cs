using UnityEngine;
using SpherePath.Configuration;

namespace SpherePath.Level
{
    public sealed class CorridorIndicator
    {
        private static readonly int PathMinXProperty = Shader.PropertyToID("_PathMinX");
        private static readonly int PathMaxXProperty = Shader.PropertyToID("_PathMaxX");

        private readonly GameplayConfiguration _configuration;
        private readonly Transform _corridor;
        private readonly Renderer _renderer;
        private MaterialPropertyBlock _propertyBlock;

        public CorridorIndicator(GameplayConfiguration configuration, LevelViewReferences scene)
        {
            _configuration = configuration;
            _corridor = scene.Corridor;
            _renderer = _corridor.GetComponent<Renderer>();
        }

        public void Update(Vector3 start, Vector3 target, float playerRadius)
        {
            var startFlat = new Vector3(start.x, 0f, start.z);
            var targetFlat = new Vector3(target.x, 0f, target.z);
            var midpoint = (startFlat + targetFlat) * 0.5f;
            var length = Vector3.Distance(startFlat, targetFlat);
            var pathWidth = playerRadius * 2f;
            var visualWidth = pathWidth + _configuration.CorridorVisualPadding * 2f;
            var pathMinX = _configuration.CorridorVisualPadding / visualWidth;
            _corridor.position = new Vector3(midpoint.x, _configuration.CorridorGroundOffset, midpoint.z);
            _corridor.localScale = new Vector3(visualWidth, _configuration.CorridorHeight, length);

            if (_renderer == null)
            {
                return;
            }

            if (_propertyBlock == null)
            {
                _propertyBlock = new MaterialPropertyBlock();
            }

            _renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetFloat(PathMinXProperty, pathMinX);
            _propertyBlock.SetFloat(PathMaxXProperty, 1f - pathMinX);
            _renderer.SetPropertyBlock(_propertyBlock);
        }
    }
}
