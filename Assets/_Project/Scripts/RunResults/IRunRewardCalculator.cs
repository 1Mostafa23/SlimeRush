public interface IRunRewardCalculator
{
    int CalculateCoins(RunResultType resultType, int remainingSlimes, int defeatedEnemies, bool bossDefeated);
}
