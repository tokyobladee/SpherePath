using UnityEngine;

namespace SpherePath.Level
{
    public sealed class CorridorIndicator
    {
        private const float VisualPadding = 0.45f;

        private static readonly int PathMinXProperty = Shader.PropertyToID("_PathMinX");
        private static readonly int PathMaxXProperty = Shader.PropertyToID("_PathMaxX");

        private readonly Transform _corridor;
        private readonly Renderer _renderer;
        private readonly MaterialPropertyBlock _propertyBlock = new MaterialPropertyBlock();

        public CorridorIndicator(LevelViewReferences scene)
        {
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
            var visualWidth = pathWidth + VisualPadding * 2f;
            var pathMinX = VisualPadding / visualWidth;
            _corridor.position = new Vector3(midpoint.x, 0.02f, midpoint.z);
            _corridor.localScale = new Vector3(visualWidth, 0.04f, length);

            if (_renderer == null)
            {
                return;
            }

            _renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetFloat(PathMinXProperty, pathMinX);
            _propertyBlock.SetFloat(PathMaxXProperty, 1f - pathMinX);
            _renderer.SetPropertyBlock(_propertyBlock);
        }
    }
}
