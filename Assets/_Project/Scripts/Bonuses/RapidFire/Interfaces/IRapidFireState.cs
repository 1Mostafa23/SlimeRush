public interface IRapidFireState
{
    void Enter();
    void Exit();
    void Activate(float duration);
    void Tick(float deltaTime);
}
