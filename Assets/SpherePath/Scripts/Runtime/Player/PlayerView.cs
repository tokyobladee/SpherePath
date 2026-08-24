using UnityEngine;

namespace SpherePath.Player
{
    public sealed class PlayerView : MonoBehaviour
    {
        private Transform _cachedTransform;

        public Vector3 Position => _cachedTransform.position;

        public float Radius { get; private set; }

        private void Awake()
        {
            _cachedTransform = transform;
        }

        public void SetRadius(float radius)
        {
            Radius = Mathf.Max(0.05f, radius);
            var diameter = Radius * 2f;
            _cachedTransform.localScale = new Vector3(diameter, diameter, diameter);
        }

        public void SetPosition(Vector3 position)
        {
            _cachedTransform.position = position;
        }

        public void SetChargeFeedback(float charge)
        {
            var stretch = Mathf.Lerp(1f, 0.82f, charge);
            var width = Mathf.Lerp(1f, 1.12f, charge);
            var diameter = Radius * 2f;
            _cachedTransform.localScale = new Vector3(diameter * width, diameter * stretch, diameter * width);
        }

        public void SetIdleFeedback()
        {
            var pulse = 1f + Mathf.Sin(Time.time * 3f) * 0.025f;
            var diameter = Radius * 2f;
            _cachedTransform.localScale = Vector3.one * (diameter * pulse);
        }
    }
}
