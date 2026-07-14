using System;

public class RunResultService : IRunResultService
{
    private readonly ISlimeCrowd slimeCrowd;
    private readonly IRunStatsService runStatsService;
    private readonly IRunRewardCalculator rewardCalculator;
    private readonly ICurrencyWallet currencyWallet;
    private readonly ISfxService sfxService;

    public bool IsCompleted { get; private set; }

    public event Action<RunResultData> RunCompleted;

    public RunResultService(
        ISlimeCrowd slimeCrowd,
        IRunStatsService runStatsService,
        IRunRewardCalculator rewardCalculator,
        ICurrencyWallet currencyWallet,
        ISfxService sfxService)
    {
        this.slimeCrowd = slimeCrowd;
        this.runStatsService = runStatsService;
        this.rewardCalculator = rewardCalculator;
        this.currencyWallet = currencyWallet;
        this.sfxService = sfxService;
    }

    public void CompleteRun()
    {
        if (IsCompleted)
            return;

        IsCompleted = true;

        int remainingSlimes = slimeCrowd.SlimeCount;
        int defeatedEnemies = runStatsService.DefeatedEnemies;
        bool bossDefeated = runStatsService.BossDefeated;
        RunResultType resultType = remainingSlimes <= 0
            ? RunResultType.Defeat
            : bossDefeated
                ? RunResultType.Victory
                : RunResultType.Defeat;
        int totalCoins = rewardCalculator.CalculateCoins(resultType, remainingSlimes, defeatedEnemies, bossDefeated);

        currencyWallet.AddCoins(totalCoins);
        if (resultType == RunResultType.Victory)
            sfxService.PlayVictoryReward();

        RunCompleted?.Invoke(new RunResultData(resultType, remainingSlimes, defeatedEnemies, bossDefeated, totalCoins));
    }
}
