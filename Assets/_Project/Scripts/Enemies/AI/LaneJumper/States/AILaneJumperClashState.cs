public class AILaneJumperClashState : IAILaneJumperState
{
    private readonly IAILaneJumperStateContext enemy;

    public AILaneJumperClashState(IAILaneJumperStateContext enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.HideWarning();
        enemy.ResetClashDamageTimer();
    }

    public void Tick(float deltaTime)
    {
        EnemyClashTickResult result = enemy.TickClash(deltaTime);

        if (result == EnemyClashTickResult.EnemyDefeated)
        {
            enemy.ChangeToDefeated();
            return;
        }

        if (result == EnemyClashTickResult.CrowdDefeated)
        {
            // Game over hook: route to a level/game state service when it exists.
            return;
        }

        if (!enemy.IsClashing)
            enemy.ChangeToObserve();
    }

    public void Exit()
    {
    }
}
