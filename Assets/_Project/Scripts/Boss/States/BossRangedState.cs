public class BossRangedState : IBossState
{
    private readonly IBossRuntimeContext bossRuntimeContext;

    public BossRangedState(IBossRuntimeContext bossRuntimeContext)
    {
        this.bossRuntimeContext = bossRuntimeContext;
    }

    public void Enter()
    {
        bossRuntimeContext.RangedAttackController?.StartShooting();
    }

    public void Exit()
    {
        bossRuntimeContext.RangedAttackController?.StopShooting();
    }

    public void Tick(float deltaTime)
    {
    }
}
