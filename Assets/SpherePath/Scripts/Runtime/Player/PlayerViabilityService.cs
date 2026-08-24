namespace SpherePath.Player
{
    public sealed class PlayerViabilityService
    {
        private readonly PlayerEnergy _energy;

        public PlayerViabilityService(PlayerEnergy energy)
        {
            _energy = energy;
        }

        public bool IsDepleted => _energy.IsDepleted;

        public bool CanSpend(float energyCost)
        {
            return _energy.CanSpend(energyCost);
        }
    }
}
