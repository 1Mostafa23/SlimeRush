public class AILaneJumperStateMachine
{
    private IAILaneJumperState currentState;

    public void ChangeState(IAILaneJumperState nextState)
    {
        if (nextState == null || currentState == nextState)
            return;

        currentState?.Exit();
        currentState = nextState;
        currentState.Enter();
    }

    public void Tick(float deltaTime)
    {
        currentState?.Tick(deltaTime);
    }
}
