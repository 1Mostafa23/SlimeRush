public class AILaneJumperPatrolState : IAILaneJumperState
{
    private readonly IAILaneJumperStateContext enemy;
    private float timer;

    public AILaneJumperPatrolState(IAILaneJumperStateContext enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        timer = 0f;
        enemy.HideWarning();
    }

    public void Tick(float deltaTime)
    {
        if (enemy.HasPlayerPassed())
        {
            enemy.ChangeToDefeated();
            return;
        }

        timer += deltaTime;
        enemy.TickPatrol(deltaTime);

        if (timer >= enemy.Settings.PatrolDuration)
            enemy.ChangeToObserve();
    }

    public void Exit()
    {
    }
}
