public readonly struct RunResultData
{
    public RunResultData(int remainingSlimes, int defeatedEnemies, bool bossDefeated, int totalCoins)
    {
        RemainingSlimes = remainingSlimes;
        DefeatedEnemies = defeatedEnemies;
        BossDefeated = bossDefeated;
        TotalCoins = totalCoins;
    }

    public int RemainingSlimes { get; }
    public int DefeatedEnemies { get; }
    public bool BossDefeated { get; }
    public int TotalCoins { get; }
}
