using UnityEngine;
using Zenject;

public class BossFightService : IBossFightService, ITickable
{
    private const int SlimeDamagePerTick = 5;
    private const int BossDamagePerTick = 5;
    private const float FightTickInterval = 1f;

    private readonly ISlimeCrowd slimeCrowd;
    private readonly ISlimeCrowdCommands slimeCrowdCommands;
    private readonly IPlayerCrowdMovementController playerMovementController;
    private readonly IBossCrowdFormationController bossCrowdFormationController;
    private readonly BossCombatant bossCombatant;
    private readonly BossCameraController bossCameraController;
    private readonly BossHitFeedback bossHitFeedback;

    private Collider bossTrigger;
    private bool isFighting;
    private float elapsedTime;

    public BossFightService(
        ISlimeCrowd slimeCrowd,
        ISlimeCrowdCommands slimeCrowdCommands,
        IPlayerCrowdMovementController playerMovementController,
        IBossCrowdFormationController bossCrowdFormationController,
        BossCombatant bossCombatant,
        BossCameraController bossCameraController,
        BossHitFeedback bossHitFeedback)
    {
        this.slimeCrowd = slimeCrowd;
        this.slimeCrowdCommands = slimeCrowdCommands;
        this.playerMovementController = playerMovementController;
        this.bossCrowdFormationController = bossCrowdFormationController;
        this.bossCombatant = bossCombatant;
        this.bossCameraController = bossCameraController;
        this.bossHitFeedback = bossHitFeedback;
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
        bossCameraController.FocusOnBoss();
    }

    public void Tick()
    {
        if (!isFighting)
            return;

        elapsedTime += Time.deltaTime;

        if (elapsedTime < FightTickInterval)
            return;

        elapsedTime = 0f;
        ApplyFightTick();
    }

    private void ApplyFightTick()
    {
        if (slimeCrowd.SlimeCount <= 0)
        {
            StopCloseFight(false);
            Debug.Log("BossFightService: Player crowd defeated. Defeat flow hook.");
            return;
        }

        slimeCrowdCommands.RemoveSlimes(SlimeDamagePerTick);
        bossCombatant.TakeDamage(BossDamagePerTick);
        bossHitFeedback.PlayHit();

        if (bossCombatant.IsDefeated)
        {
            StopCloseFight(true);

            if (bossTrigger != null)
                bossTrigger.enabled = false;

            Debug.Log("BossFightService: Boss defeated. Victory/reward hook.");
            return;
        }

        if (slimeCrowd.SlimeCount <= 0)
        {
            StopCloseFight(false);
            Debug.Log("BossFightService: Player crowd defeated. Defeat flow hook.");
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
        bossCameraController.StopFocus();
    }
}
