public class EnemyClashTickContext
{
    public IEnemyCombatant Combatant { get; set; }
    public IEnemyClashFeedback Feedback { get; set; }
    public IDamageBlockedReaction BlockedReaction { get; set; }
    public float TickInterval { get; set; }
    public float BlockedRecoveryDuration { get; set; }
    public float ElapsedTime { get; set; }
}
