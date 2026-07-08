public class ShieldDamageBlocker : IDamageBlocker
{
    private readonly IShieldService shieldService;

    public ShieldDamageBlocker(IShieldService shieldService)
    {
        this.shieldService = shieldService;
    }

    public bool TryBlockDamage()
    {
        return shieldService.TryConsumeShield();
    }
}
