using UnityEngine;

namespace SpherePath.Shooting
{
    public sealed class ChargeMeter
    {
        private readonly float _maxChargeTime;
        private readonly float _minEnergyCost;

        private float _chargeTime;

        public ChargeMeter(float maxChargeTime, float minEnergyCost)
        {
            _maxChargeTime = Mathf.Max(0.01f, maxChargeTime);
            _minEnergyCost = Mathf.Max(0f, minEnergyCost);
        }

        public float Normalized => Mathf.Clamp01(_chargeTime / _maxChargeTime);

        public float GetEnergyCost(float energyBudget)
        {
            var budget = Mathf.Max(0f, energyBudget);
            var minimumCost = Mathf.Min(_minEnergyCost, budget);
            return Mathf.Lerp(minimumCost, budget, Normalized);
        }

        public bool HasReachedEnergyBudget(float energyBudget)
        {
            var budget = Mathf.Max(0f, energyBudget);
            return GetEnergyCost(budget) >= budget - Mathf.Epsilon;
        }

        public void ClampToEnergyBudget(float energyBudget)
        {
            if (Mathf.Max(0f, energyBudget) <= Mathf.Epsilon)
            {
                _chargeTime = 0f;
                return;
            }

            _chargeTime = Mathf.Min(_chargeTime, _maxChargeTime);
        }

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
