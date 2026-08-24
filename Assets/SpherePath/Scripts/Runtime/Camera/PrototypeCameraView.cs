using UnityEngine;

namespace SpherePath.Cameras
{
    public sealed class PrototypeCameraView : MonoBehaviour
    {
        private Transform _cachedTransform;
        private Vector3 _basePosition;
        private Quaternion _baseRotation;
        private Vector3 _followOffset;
        private float _followSpeed = 6f;

        private void Awake()
        {
            _cachedTransform = transform;
            CaptureBasePose();
        }

        public void CaptureBasePose()
        {
            _cachedTransform = transform;
            _basePosition = _cachedTransform.position;
            _baseRotation = _cachedTransform.rotation;
            _followOffset = _basePosition;
        }

        public void SetFollowSettings(Vector3 offset, float followSpeed)
        {
            _followOffset = offset;
            _followSpeed = Mathf.Max(0.01f, followSpeed);
            _basePosition = _cachedTransform.position;
        }

        public void SetShake(float intensity)
        {
            var clampedIntensity = Mathf.Clamp01(intensity);
            var time = Time.time * 45f;
            var x = Mathf.Sin(time) * 0.05f * clampedIntensity;
            var y = Mathf.Cos(time * 1.21f) * 0.04f * clampedIntensity;
            _cachedTransform.position = _basePosition + new Vector3(x, y, 0f);
            _cachedTransform.rotation = _baseRotation * Quaternion.Euler(0f, 0f, Mathf.Sin(time * 0.5f) * clampedIntensity);
        }

        public void Follow(Vector3 targetPosition, float deltaTime)
        {
            _basePosition = Vector3.Lerp(_basePosition, targetPosition + _followOffset, 1f - Mathf.Exp(-_followSpeed * deltaTime));
            _cachedTransform.position = _basePosition;
            _cachedTransform.rotation = _baseRotation;
        }

        public void SnapToFollowTarget(Vector3 targetPosition)
        {
            _basePosition = targetPosition + _followOffset;
            _cachedTransform.position = _basePosition;
            _cachedTransform.rotation = _baseRotation;
        }

        public void ResetShake()
        {
            _cachedTransform.position = _basePosition;
            _cachedTransform.rotation = _baseRotation;
        }
    }
}
