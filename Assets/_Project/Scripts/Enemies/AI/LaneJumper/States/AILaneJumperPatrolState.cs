public class AILaneJumperPatrolState : IAILaneJumperState
{
    private readonly IAILaneJumperStateContext enemy;

    public AILaneJumperPatrolState(IAILaneJumperStateContext enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.HideWarning();
    }

    public void Tick(float deltaTime)
    {
        enemy.TickPatrol(deltaTime);

        if (enemy.CanAttackPlayer())
            enemy.ChangeToObserve();
    }

    public void Exit()
    {
    }
}
