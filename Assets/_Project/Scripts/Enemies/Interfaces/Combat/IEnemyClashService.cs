public interface IEnemyClashService
{
    EnemyClashTickResult Tick(EnemyClashTickContext context, float deltaTime);
}
