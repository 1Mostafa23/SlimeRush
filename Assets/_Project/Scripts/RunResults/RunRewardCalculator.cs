public class RunRewardCalculator : IRunRewardCalculator
{
    private const int CoinsPerSurvivedSlime = 2;
    private const int VictoryCoinsPerDefeatedEnemy = 15;
    private const int DefeatCoinsPerDefeatedEnemy = 10;
    private const int BossReward = 100;

    public int CalculateCoins(RunResultType resultType, int remainingSlimes, int defeatedEnemies, bool bossDefeated)
    {
        if (resultType == RunResultType.Defeat)
            return defeatedEnemies * DefeatCoinsPerDefeatedEnemy;

        return remainingSlimes * CoinsPerSurvivedSlime
            + defeatedEnemies * VictoryCoinsPerDefeatedEnemy
            + BossReward;
    }
}
