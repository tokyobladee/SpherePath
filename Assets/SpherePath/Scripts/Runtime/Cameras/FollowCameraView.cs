using UnityEngine;

namespace SpherePath.Cameras
{
    public sealed class FollowCameraView : MonoBehaviour
    {
        [SerializeField] private Vector3 followOffset = new Vector3(0f, 18.5f, -8.5f);
        [SerializeField] private float followSpeed = 6f;

        private Transform _cachedTransform;
        private Vector3 _basePosition;
        private Quaternion _baseRotation;
        private Vector3 _shakeOffset;
        private Quaternion _shakeRotation = Quaternion.identity;

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
            _shakeOffset = Vector3.zero;
            _shakeRotation = Quaternion.identity;
            ApplyPose();
        }

        public void SetFollowSettings(Vector3 offset, float followSpeed)
        {
            if (_cachedTransform == null)
            {
                _cachedTransform = transform;
            }

            this.followOffset = offset;
            this.followSpeed = Mathf.Max(0.01f, followSpeed);
            _basePosition = _cachedTransform.position;
            ApplyPose();
        }

        public void SetShake(float intensity)
        {
            var clampedIntensity = Mathf.Clamp01(intensity);
            var time = Time.time * 45f;
            var x = Mathf.Sin(time) * 0.05f * clampedIntensity;
            var y = Mathf.Cos(time * 1.21f) * 0.04f * clampedIntensity;
            _shakeOffset = new Vector3(x, y, 0f);
            _shakeRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(time * 0.5f) * clampedIntensity);
            ApplyPose();
        }

        public void Follow(Vector3 targetPosition, float deltaTime)
        {
            _basePosition = Vector3.Lerp(_basePosition, targetPosition + followOffset, 1f - Mathf.Exp(-followSpeed * deltaTime));
            ApplyPose();
        }

        public void SnapToFollowTarget(Vector3 targetPosition)
        {
            _basePosition = targetPosition + followOffset;
            ApplyPose();
        }

        public void ResetShake()
        {
            _shakeOffset = Vector3.zero;
            _shakeRotation = Quaternion.identity;
            ApplyPose();
        }

        private void ApplyPose()
        {
            _cachedTransform.SetPositionAndRotation(_basePosition + _baseRotation * _shakeOffset, _baseRotation * _shakeRotation);
        }
    }
}
