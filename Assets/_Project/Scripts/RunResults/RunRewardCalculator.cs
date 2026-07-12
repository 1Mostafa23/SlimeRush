public class RunRewardCalculator : IRunRewardCalculator
{
    private const int CoinsPerSurvivedSlime = 2;
    private const int CoinsPerDefeatedEnemy = 15;
    private const int BossReward = 100;

    public int CalculateCoins(int remainingSlimes, int defeatedEnemies, bool bossDefeated)
    {
        int bossCoins = bossDefeated ? BossReward : 0;
        return remainingSlimes * CoinsPerSurvivedSlime
            + defeatedEnemies * CoinsPerDefeatedEnemy
            + bossCoins;
    }
}
