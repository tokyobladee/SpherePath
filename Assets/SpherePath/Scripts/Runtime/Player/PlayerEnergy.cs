using System;
using UnityEngine;

namespace SpherePath.Player
{
    public sealed class PlayerEnergy
    {
        public PlayerEnergy(float maximumEnergy)
        {
            MaximumEnergy = Mathf.Max(0f, maximumEnergy);
            CurrentEnergy = MaximumEnergy;
        }

        public float MaximumEnergy { get; }

        public float CurrentEnergy { get; private set; }

        public float Normalized => MaximumEnergy <= 0f ? 0f : CurrentEnergy / MaximumEnergy;

        public bool IsDepleted => CurrentEnergy <= Mathf.Epsilon;

        public event Action<float> Changed;

        public bool CanSpend(float amount)
        {
            return CurrentEnergy - Mathf.Max(0f, amount) >= 0f;
        }

        public void Spend(float amount)
        {
            CurrentEnergy = Mathf.Max(0f, CurrentEnergy - Mathf.Max(0f, amount));
            Changed?.Invoke(Normalized);
        }

        public void Deplete()
        {
            CurrentEnergy = 0f;
            Changed?.Invoke(Normalized);
        }

        public void Reset()
        {
            CurrentEnergy = MaximumEnergy;
            Changed?.Invoke(Normalized);
        }
    }
}
