public class SlimeDamageApplier
{
    private readonly ISlimeCrowdDamageCommands slimeCrowdDamageCommands;

    public SlimeDamageApplier(ISlimeCrowdDamageCommands slimeCrowdDamageCommands)
    {
        this.slimeCrowdDamageCommands = slimeCrowdDamageCommands;
    }

    public bool KillSlime(SlimeHitbox slimeHitbox)
    {
        if (slimeHitbox == null)
            return false;

        return slimeCrowdDamageCommands.RemoveSlime(slimeHitbox);
    }
}
