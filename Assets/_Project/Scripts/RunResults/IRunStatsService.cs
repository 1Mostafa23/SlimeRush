public interface IRunStatsService
{
    int DefeatedEnemies { get; }
    bool BossDefeated { get; }

    void RegisterEnemyDefeated();
    void RegisterBossDefeated();
    void Reset();
}
