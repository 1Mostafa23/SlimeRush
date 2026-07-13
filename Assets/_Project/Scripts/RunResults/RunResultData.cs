public readonly struct RunResultData
{
    public RunResultData(
        RunResultType resultType,
        int remainingSlimes,
        int defeatedEnemies,
        bool bossDefeated,
        int totalCoins)
    {
        ResultType = resultType;
        RemainingSlimes = remainingSlimes;
        DefeatedEnemies = defeatedEnemies;
        BossDefeated = bossDefeated;
        TotalCoins = totalCoins;
    }

    public RunResultType ResultType { get; }
    public int RemainingSlimes { get; }
    public int DefeatedEnemies { get; }
    public bool BossDefeated { get; }
    public int TotalCoins { get; }

    public bool IsVictory => ResultType == RunResultType.Victory;
}
