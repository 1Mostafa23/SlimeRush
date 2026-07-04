public class AILaneJumperDashState : IAILaneJumperState
{
    private readonly IAILaneJumperStateContext enemy;

    public AILaneJumperDashState(IAILaneJumperStateContext enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.BeginDash();
    }

    public void Tick(float deltaTime)
    {
        if (enemy.IsClashing)
        {
            enemy.ChangeToClash();
            return;
        }

        enemy.TickDash(deltaTime);

        if (enemy.HasReachedDashTarget())
            enemy.ChangeToObserve();
    }

    public void Exit()
    {
    }
}
