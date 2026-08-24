namespace SpherePath.Obstacles
{
    public readonly struct ObstacleClearingResult
    {
        public ObstacleClearingResult(int clearedCount, float infectionRadius)
        {
            ClearedCount = clearedCount;
            InfectionRadius = infectionRadius;
        }

        public int ClearedCount { get; }

        public float InfectionRadius { get; }
    }
}
