using UnityEngine;

namespace SpherePath.Player
{
    public sealed class PlayerSizeService
    {
        private readonly float _minimumRadius;
        private readonly float _maximumRadius;

        public PlayerSizeService(float minimumRadius, float maximumRadius)
        {
            _minimumRadius = Mathf.Max(0.01f, minimumRadius);
            _maximumRadius = Mathf.Max(_minimumRadius, maximumRadius);
        }

        public float GetRadius(float normalizedEnergy)
        {
            return Mathf.Lerp(_minimumRadius, _maximumRadius, Mathf.Clamp01(normalizedEnergy));
        }
    }
}
