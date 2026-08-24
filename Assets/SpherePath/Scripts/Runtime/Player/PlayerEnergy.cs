using System;
using UnityEngine;

namespace SpherePath.Player
{
    public sealed class PlayerEnergy
    {
        private readonly float _minimumEnergy;

        public PlayerEnergy(float maximumEnergy, float minimumEnergy)
        {
            MaximumEnergy = Mathf.Max(maximumEnergy, minimumEnergy);
            _minimumEnergy = Mathf.Max(0f, minimumEnergy);
            CurrentEnergy = MaximumEnergy;
        }

        public float MaximumEnergy { get; }

        public float CurrentEnergy { get; private set; }

        public float Normalized => MaximumEnergy <= 0f ? 0f : CurrentEnergy / MaximumEnergy;

        public bool IsBelowMinimum => CurrentEnergy <= _minimumEnergy;

        public event Action<float> Changed;

        public bool CanSpend(float amount)
        {
            return CurrentEnergy - Mathf.Max(0f, amount) >= _minimumEnergy;
        }

        public void Spend(float amount)
        {
            CurrentEnergy = Mathf.Max(0f, CurrentEnergy - Mathf.Max(0f, amount));
            Changed?.Invoke(Normalized);
        }

        public void Reset()
        {
            CurrentEnergy = MaximumEnergy;
            Changed?.Invoke(Normalized);
        }
    }
}
