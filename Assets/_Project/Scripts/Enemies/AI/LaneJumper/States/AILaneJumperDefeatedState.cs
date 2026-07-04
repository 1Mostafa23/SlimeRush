public class AILaneJumperDefeatedState : IAILaneJumperState
{
    private readonly IAILaneJumperStateContext enemy;

    public AILaneJumperDefeatedState(IAILaneJumperStateContext enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.Defeat();
    }

    public void Tick(float deltaTime)
    {
    }

    public void Exit()
    {
    }
}
