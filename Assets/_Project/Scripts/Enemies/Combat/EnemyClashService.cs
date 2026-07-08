public class EnemyClashService : IEnemyClashService
{
    private readonly ISlimeCrowd slimeCrowd;
    private readonly ISlimeCrowdCommands slimeCrowdCommands;
    private readonly IDamageBlocker damageBlocker;
    private readonly ICameraImpactService cameraImpactService;

    public EnemyClashService(
        ISlimeCrowd slimeCrowd,
        ISlimeCrowdCommands slimeCrowdCommands,
        IDamageBlocker damageBlocker,
        ICameraImpactService cameraImpactService)
    {
        this.slimeCrowd = slimeCrowd;
        this.slimeCrowdCommands = slimeCrowdCommands;
        this.damageBlocker = damageBlocker;
        this.cameraImpactService = cameraImpactService;
    }

    public EnemyClashTickResult Tick(EnemyClashTickContext context, float deltaTime)
    {
        context.ElapsedTime += deltaTime;

        if (context.ElapsedTime < context.TickInterval)
            return EnemyClashTickResult.None;

        context.ElapsedTime = 0f;

        if (slimeCrowd.SlimeCount <= 0)
            return EnemyClashTickResult.CrowdDefeated;

        if (damageBlocker.TryBlockDamage())
        {
            context.Combatant.ReducePower(1);

            if (context.BlockedReaction != null)
                context.BlockedReaction.OnDamageBlocked();
            else
                context.Feedback?.PlayBlocked();

            cameraImpactService.PlaySmallImpact();
            context.ElapsedTime = -context.BlockedRecoveryDuration;

            return context.Combatant.IsDefeated
                ? EnemyClashTickResult.EnemyDefeated
                : EnemyClashTickResult.TickApplied;
        }

        slimeCrowdCommands.RemoveSlimes(1);
        context.Combatant.ReducePower(1);
        context.Feedback?.PlayTick();
        cameraImpactService.PlaySmallImpact();

        if (context.Combatant.IsDefeated)
            return EnemyClashTickResult.EnemyDefeated;

        if (slimeCrowd.SlimeCount <= 0)
            return EnemyClashTickResult.CrowdDefeated;

        return EnemyClashTickResult.TickApplied;
    }
}
