public class SlimeDamageApplier
{
    private readonly ISlimeCrowdDamageCommands slimeCrowdDamageCommands;
    private readonly IDamageBlocker damageBlocker;

    public SlimeDamageApplier(
        ISlimeCrowdDamageCommands slimeCrowdDamageCommands,
        IDamageBlocker damageBlocker)
    {
        this.slimeCrowdDamageCommands = slimeCrowdDamageCommands;
        this.damageBlocker = damageBlocker;
    }

    public bool KillSlime(SlimeHitbox slimeHitbox)
    {
        return TryKillSlime(slimeHitbox) != SlimeDamageResult.NotFound;
    }

    public SlimeDamageResult TryKillSlime(SlimeHitbox slimeHitbox)
    {
        if (slimeHitbox == null)
            return SlimeDamageResult.NotFound;

        if (damageBlocker.TryBlockDamage())
            return SlimeDamageResult.Blocked;

        return slimeCrowdDamageCommands.RemoveSlime(slimeHitbox)
            ? SlimeDamageResult.Applied
            : SlimeDamageResult.NotFound;
    }
}
