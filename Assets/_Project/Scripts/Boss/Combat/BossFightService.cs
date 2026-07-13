using UnityEngine;
using Zenject;

public class BossFightService : IBossFightService, ITickable
{
    private readonly ISlimeCrowd slimeCrowd;
    private readonly ISlimeCrowdCommands slimeCrowdCommands;
    private readonly IPlayerCrowdMovementController playerMovementController;
    private readonly IBossCrowdFormationController bossCrowdFormationController;
    private readonly BossCombatant bossCombatant;
    private readonly IBossRuntimeContext bossRuntimeContext;
    private readonly IRunStatsService runStatsService;
    private readonly IRunResultService runResultService;
    private readonly BossClashSettings clashSettings;

    private Collider bossTrigger;
    private bool isFighting;
    private float elapsedTime;

    public BossFightService(
        ISlimeCrowd slimeCrowd,
        ISlimeCrowdCommands slimeCrowdCommands,
        IPlayerCrowdMovementController playerMovementController,
        IBossCrowdFormationController bossCrowdFormationController,
        BossCombatant bossCombatant,
        IBossRuntimeContext bossRuntimeContext,
        IRunStatsService runStatsService,
        IRunResultService runResultService,
        BossClashSettings clashSettings)
    {
        this.slimeCrowd = slimeCrowd;
        this.slimeCrowdCommands = slimeCrowdCommands;
        this.playerMovementController = playerMovementController;
        this.bossCrowdFormationController = bossCrowdFormationController;
        this.bossCombatant = bossCombatant;
        this.bossRuntimeContext = bossRuntimeContext;
        this.runStatsService = runStatsService;
        this.runResultService = runResultService;
        this.clashSettings = clashSettings;
    }

    public void StartCloseFight(Transform fightPoint, Collider bossTrigger)
    {
        if (isFighting || bossCombatant.IsDefeated)
            return;

        this.bossTrigger = bossTrigger;
        elapsedTime = 0f;
        isFighting = true;

        playerMovementController.StopMovement();
        bossCrowdFormationController.EnterBossFormation(fightPoint);
        bossRuntimeContext.CameraController?.FocusOnBoss();
    }

    public void Tick()
    {
        if (!isFighting)
            return;

        elapsedTime += Time.deltaTime;

        if (elapsedTime < clashSettings.FightTickInterval)
            return;

        elapsedTime = 0f;
        ApplyFightTick();
    }

    private void ApplyFightTick()
    {
        int currentSlimeCount = slimeCrowd.SlimeCount;

        if (currentSlimeCount <= 0)
        {
            StopCloseFight(false);
            runResultService.CompleteRun();
            Debug.Log("BossFightService: Player crowd defeated. Defeat flow hook.");
            return;
        }

        int nextSlimeCount = currentSlimeCount - clashSettings.SlimeDamagePerTick;

        if (nextSlimeCount <= 0)
        {
            slimeCrowdCommands.RemoveSlimes(clashSettings.SlimeDamagePerTick);
            StopCloseFight(false);
            runResultService.CompleteRun();
            Debug.Log("BossFightService: Player crowd defeated. Boss survives simultaneous lethal tick.");
            return;
        }

        int nextBossHp = bossCombatant.CurrentHp - clashSettings.BossDamagePerTick;

        slimeCrowdCommands.RemoveSlimes(clashSettings.SlimeDamagePerTick);
        bossCombatant.TakeDamage(clashSettings.BossDamagePerTick);
        bossRuntimeContext.HitFeedback?.PlayHit();

        if (nextBossHp <= 0)
            runStatsService.RegisterBossDefeated();

        if (nextBossHp <= 0)
        {
            StopCloseFight(true);

            if (bossTrigger != null)
                bossTrigger.enabled = false;

            runResultService.CompleteRun();
            Debug.Log("BossFightService: Boss defeated. Victory/reward hook.");
            return;
        }
    }

    public void StopCloseFight(bool resumePlayerMovement)
    {
        if (!isFighting)
            return;

        isFighting = false;

        if (resumePlayerMovement)
            playerMovementController.StartMovement();

        bossCrowdFormationController.ExitBossFormation();
        bossRuntimeContext.CameraController?.StopFocus();
    }
}
