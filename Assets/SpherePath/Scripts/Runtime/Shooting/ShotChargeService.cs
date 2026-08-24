using SpherePath.Player;

namespace SpherePath.Shooting
{
    public sealed class ShotChargeService
    {
        private readonly PlayerEnergy _energy;
        private readonly ChargeMeter _chargeMeter;

        public ShotChargeService(PlayerEnergy energy, ChargeMeter chargeMeter)
        {
            _energy = energy;
            _chargeMeter = chargeMeter;
        }

        public float NormalizedCharge => _chargeMeter.Normalized;

        public float ProjectileRadius => _chargeMeter.ProjectileRadius;

        public bool CanAffordMinimumShot => _energy.CanSpend(_chargeMeter.MinimumEnergyCost);

        public void Begin()
        {
            _chargeMeter.Reset();
        }

        public void Tick(float deltaTime)
        {
            _chargeMeter.Tick(deltaTime);
        }

        public ChargedShot CreateShot()
        {
            return new ChargedShot(_chargeMeter.ProjectileRadius, _chargeMeter.EnergyCost, _chargeMeter.Normalized);
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
    }
}
