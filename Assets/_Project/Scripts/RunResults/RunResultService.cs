using System;

public class RunResultService : IRunResultService
{
    private readonly ISlimeCrowd slimeCrowd;
    private readonly IRunStatsService runStatsService;
    private readonly IRunRewardCalculator rewardCalculator;
    private readonly ICurrencyWallet currencyWallet;

    private bool isCompleted;

    public event Action<RunResultData> RunCompleted;

    public RunResultService(
        ISlimeCrowd slimeCrowd,
        IRunStatsService runStatsService,
        IRunRewardCalculator rewardCalculator,
        ICurrencyWallet currencyWallet)
    {
        this.slimeCrowd = slimeCrowd;
        this.runStatsService = runStatsService;
        this.rewardCalculator = rewardCalculator;
        this.currencyWallet = currencyWallet;
    }

    public void CompleteRun()
    {
        if (isCompleted)
            return;

        isCompleted = true;

        int remainingSlimes = slimeCrowd.SlimeCount;
        int defeatedEnemies = runStatsService.DefeatedEnemies;
        bool bossDefeated = runStatsService.BossDefeated;
        int totalCoins = rewardCalculator.CalculateCoins(remainingSlimes, defeatedEnemies, bossDefeated);

        currencyWallet.AddCoins(totalCoins);
        RunCompleted?.Invoke(new RunResultData(remainingSlimes, defeatedEnemies, bossDefeated, totalCoins));
    }
}
