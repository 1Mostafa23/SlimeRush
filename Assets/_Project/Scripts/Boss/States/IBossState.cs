public interface IBossState
{
    void Enter();
    void Exit();
    void Tick(float deltaTime);
}
