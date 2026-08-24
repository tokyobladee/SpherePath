namespace SpherePath.Shooting
{
    public readonly struct ChargedShot
    {
        public ChargedShot(float projectileRadius, float energyCost, float normalizedCharge)
        {
            ProjectileRadius = projectileRadius;
            EnergyCost = energyCost;
            NormalizedCharge = normalizedCharge;
        }

        public float ProjectileRadius { get; }

        public float EnergyCost { get; }

        public float NormalizedCharge { get; }
    }
}
