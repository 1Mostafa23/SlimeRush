public class BossRangedState : IBossState
{
    private readonly IBossRangedAttackController rangedAttackController;

    public BossRangedState(IBossRangedAttackController rangedAttackController)
    {
        this.rangedAttackController = rangedAttackController;
    }

    public void Enter()
    {
        rangedAttackController.StartShooting();
    }

    public void Exit()
    {
        rangedAttackController.StopShooting();
    }

    public void Tick(float deltaTime)
    {
    }
}
