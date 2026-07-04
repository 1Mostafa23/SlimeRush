public class AILaneJumperWarningState : IAILaneJumperState
{
    private readonly IAILaneJumperStateContext enemy;
    private float timer;

    public AILaneJumperWarningState(IAILaneJumperStateContext enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        timer = 0f;
        enemy.ShowWarning();
    }

    public void Tick(float deltaTime)
    {
        if (enemy.HasPlayerPassed())
        {
            enemy.ChangeToDefeated();
            return;
        }

        timer += deltaTime;

        if (timer >= enemy.Settings.WarningDuration)
            enemy.ChangeToDash();
    }

    public void Exit()
    {
        enemy.HideWarning();
    }
}
