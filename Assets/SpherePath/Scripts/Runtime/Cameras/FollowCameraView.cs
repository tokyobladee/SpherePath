using UnityEngine;

namespace SpherePath.Cameras
{
    public sealed class FollowCameraView : MonoBehaviour
    {
        private Transform _cachedTransform;
        private Vector3 _followOffset;
        private float _followSpeed;
        private Vector3 _basePosition;
        private Quaternion _baseRotation;
        private Vector3 _shakeOffset;
        private Quaternion _shakeRotation = Quaternion.identity;
        private float _shakeFrequency;
        private float _shakeHorizontalAmplitude;
        private float _shakeVerticalAmplitude;
        private float _shakeVerticalFrequencyMultiplier;
        private float _shakeRotationFrequencyMultiplier;

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

            _followOffset = offset;
            _followSpeed = Mathf.Max(0.01f, followSpeed);
            _basePosition = _cachedTransform.position;
            ApplyPose();
        }

        public void SetShakeSettings(float frequency, float horizontalAmplitude, float verticalAmplitude, float verticalFrequencyMultiplier, float rotationFrequencyMultiplier)
        {
            _shakeFrequency = frequency;
            _shakeHorizontalAmplitude = horizontalAmplitude;
            _shakeVerticalAmplitude = verticalAmplitude;
            _shakeVerticalFrequencyMultiplier = verticalFrequencyMultiplier;
            _shakeRotationFrequencyMultiplier = rotationFrequencyMultiplier;
        }

        public void SetShake(float intensity)
        {
            var clampedIntensity = Mathf.Clamp01(intensity);
            var time = Time.time * _shakeFrequency;
            var x = Mathf.Sin(time) * _shakeHorizontalAmplitude * clampedIntensity;
            var y = Mathf.Cos(time * _shakeVerticalFrequencyMultiplier) * _shakeVerticalAmplitude * clampedIntensity;
            _shakeOffset = new Vector3(x, y, 0f);
            _shakeRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(time * _shakeRotationFrequencyMultiplier) * clampedIntensity);
            ApplyPose();
        }

        public void Follow(Vector3 targetPosition, float deltaTime)
        {
            _basePosition = Vector3.Lerp(_basePosition, targetPosition + _followOffset, 1f - Mathf.Exp(-_followSpeed * deltaTime));
            ApplyPose();
        }

        public void SnapToFollowTarget(Vector3 targetPosition)
        {
            _basePosition = targetPosition + _followOffset;
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
