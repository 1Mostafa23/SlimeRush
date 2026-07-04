public class AILaneJumperObserveState : IAILaneJumperState
{
    private readonly IAILaneJumperStateContext enemy;

    public AILaneJumperObserveState(IAILaneJumperStateContext enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        if (!enemy.SelectClosestPlayerLane())
        {
            enemy.ChangeToPatrol();
            return;
        }

        enemy.ChangeToWarning();
    }

    public void Tick(float deltaTime)
    {
    }

    public void Exit()
    {
    }
}
