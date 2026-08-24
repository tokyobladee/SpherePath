using UnityEngine;

namespace SpherePath.Player
{
    public sealed class PlayerView : MonoBehaviour
    {
        private Transform _cachedTransform;
        private float _minimumRenderedRadius;
        private float _chargeVerticalScale;
        private float _chargeHorizontalScale;
        private float _idlePulseFrequency;
        private float _idlePulseScale;

        public Vector3 Position => _cachedTransform.position;

        public float Radius { get; private set; }

        private void Awake()
        {
            _cachedTransform = transform;
        }

        public void ApplyVisualTuning(float minimumRenderedRadius, float chargeVerticalScale, float chargeHorizontalScale, float idlePulseFrequency, float idlePulseScale)
        {
            _minimumRenderedRadius = minimumRenderedRadius;
            _chargeVerticalScale = chargeVerticalScale;
            _chargeHorizontalScale = chargeHorizontalScale;
            _idlePulseFrequency = idlePulseFrequency;
            _idlePulseScale = idlePulseScale;
        }

        public void SetRadius(float radius)
        {
            Radius = Mathf.Max(_minimumRenderedRadius, radius);
            var diameter = Radius * 2f;
            _cachedTransform.localScale = new Vector3(diameter, diameter, diameter);
        }

        public void SetPosition(Vector3 position)
        {
            _cachedTransform.position = position;
        }

        public void SetChargeFeedback(float charge, float targetRadius)
        {
            var stretch = Mathf.Lerp(1f, _chargeVerticalScale, charge);
            var width = Mathf.Lerp(1f, _chargeHorizontalScale, charge);
            var diameter = Mathf.Max(_minimumRenderedRadius, targetRadius) * 2f;
            _cachedTransform.localScale = new Vector3(diameter * width, diameter * stretch, diameter * width);
        }

        public void SetIdleFeedback()
        {
            var pulse = 1f + Mathf.Sin(Time.time * _idlePulseFrequency) * _idlePulseScale;
            var diameter = Radius * 2f;
            _cachedTransform.localScale = Vector3.one * (diameter * pulse);
        }
    }
}
