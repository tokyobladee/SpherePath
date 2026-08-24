using UnityEngine;

namespace SpherePath.Shooting
{
    public sealed class ChargeMeter
    {
        private readonly float _maxChargeTime;
        private readonly float _minProjectileRadius;
        private readonly float _maxProjectileRadius;
        private readonly float _minEnergyCost;
        private readonly float _maxEnergyCost;

        private float _chargeTime;

        public ChargeMeter(float maxChargeTime, float minProjectileRadius, float maxProjectileRadius, float minEnergyCost, float maxEnergyCost)
        {
            _maxChargeTime = Mathf.Max(0.01f, maxChargeTime);
            _minProjectileRadius = Mathf.Max(0.01f, minProjectileRadius);
            _maxProjectileRadius = Mathf.Max(_minProjectileRadius, maxProjectileRadius);
            _minEnergyCost = Mathf.Max(0f, minEnergyCost);
            _maxEnergyCost = Mathf.Max(_minEnergyCost, maxEnergyCost);
        }

        public float Normalized => Mathf.Clamp01(_chargeTime / _maxChargeTime);

        public float ProjectileRadius => Mathf.Lerp(_minProjectileRadius, _maxProjectileRadius, Normalized);

        public float EnergyCost => Mathf.Lerp(_minEnergyCost, _maxEnergyCost, Normalized);

        public float MinimumEnergyCost => _minEnergyCost;

        public void Reset()
        {
            _chargeTime = 0f;
        }

        public void Tick(float deltaTime)
        {
            _chargeTime = Mathf.Min(_maxChargeTime, _chargeTime + Mathf.Max(0f, deltaTime));
        }
    }
}
