using UnityEngine;

public class RunStatsEnemyDefeatHandler : IEnemyDefeatHandler
{
    private readonly IRunStatsService runStatsService;
    private readonly DisableEnemyDefeatHandler innerHandler;

    public RunStatsEnemyDefeatHandler(
        IRunStatsService runStatsService,
        DisableEnemyDefeatHandler innerHandler)
    {
        this.runStatsService = runStatsService;
        this.innerHandler = innerHandler;
    }

    public void Defeat(GameObject enemyRoot)
    {
        runStatsService.RegisterEnemyDefeated();
        innerHandler.Defeat(enemyRoot);
    }
}
