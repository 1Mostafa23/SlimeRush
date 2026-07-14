using System;
using Zenject;

public class PlayerUpgradeRuntimeApplier : IInitializable, IDisposable
{
    private readonly IPlayerUpgradeService upgradeService;
    private readonly ISlimeCrowdCommands slimeCrowdCommands;
    private int observedSlimeCountUpgradeLevel;

    public PlayerUpgradeRuntimeApplier(
        IPlayerUpgradeService upgradeService,
        ISlimeCrowdCommands slimeCrowdCommands)
    {
        this.upgradeService = upgradeService;
        this.slimeCrowdCommands = slimeCrowdCommands;
    }

    public void Initialize()
    {
        observedSlimeCountUpgradeLevel = upgradeService.SlimeCountUpgradeLevel;
        upgradeService.Changed += ApplyRuntimeUpgradeDelta;
    }

    public void Dispose()
    {
        upgradeService.Changed -= ApplyRuntimeUpgradeDelta;
    }

    private void ApplyRuntimeUpgradeDelta()
    {
        int currentLevel = upgradeService.SlimeCountUpgradeLevel;
        int levelDelta = currentLevel - observedSlimeCountUpgradeLevel;

        observedSlimeCountUpgradeLevel = currentLevel;

        if (levelDelta == 0)
            return;

        int slimeDelta = Math.Abs(levelDelta) * upgradeService.SlimesPerUpgrade;

        if (levelDelta > 0)
            slimeCrowdCommands.AddSlimes(slimeDelta);
        else
            slimeCrowdCommands.RemoveSlimes(slimeDelta);
    }
}
