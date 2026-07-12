public class RunStatsService : IRunStatsService
{
    public int DefeatedEnemies { get; private set; }
    public bool BossDefeated { get; private set; }

    public void RegisterEnemyDefeated()
    {
        DefeatedEnemies++;
    }

    public void RegisterBossDefeated()
    {
        BossDefeated = true;
    }

    public void Reset()
    {
        DefeatedEnemies = 0;
        BossDefeated = false;
    }
}
