using SpherePath.Player;
using UnityEngine;

namespace SpherePath.Shooting
{
    public sealed class ShotChargeService
    {
        private readonly PlayerEnergy _energy;
        private readonly PlayerSizeService _playerSize;
        private readonly ChargeMeter _chargeMeter;

        public ShotChargeService(PlayerEnergy energy, PlayerSizeService playerSize, ChargeMeter chargeMeter)
        {
            _energy = energy;
            _playerSize = playerSize;
            _chargeMeter = chargeMeter;
        }

        public float NormalizedCharge => _chargeMeter.Normalized;

        public float ProjectileRadius => GetProjectileRadius(EnergyCost);

        public float EnergyCost => _chargeMeter.GetEnergyCost(_energy.CurrentEnergy);

        public void Begin()
        {
            _chargeMeter.Reset();
        }

        public bool TickUntilEnergyLimit(float deltaTime)
        {
            _chargeMeter.Tick(deltaTime);

            if (!_chargeMeter.HasReachedEnergyBudget(_energy.CurrentEnergy))
            {
                return false;
            }

            _chargeMeter.ClampToEnergyBudget(_energy.CurrentEnergy);
            return true;
        }

        public ChargedShot CreateShot()
        {
            var energyCost = Mathf.Min(EnergyCost, _energy.CurrentEnergy);
            return new ChargedShot(GetProjectileRadius(energyCost), energyCost, _chargeMeter.Normalized);
        }

        public bool TrySpend(ChargedShot shot)
        {
            if (!_energy.CanSpend(shot.EnergyCost))
            {
                return false;
            }

            _energy.Spend(shot.EnergyCost);
            return true;
        }

        private float GetProjectileRadius(float energyCost)
        {
            if (_energy.MaximumEnergy <= Mathf.Epsilon)
            {
                return _playerSize.GetRadius(0f);
            }

            return _playerSize.GetRadius(Mathf.Clamp01(energyCost / _energy.MaximumEnergy));
        }
    }
}
