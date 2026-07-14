using System;

public interface IPlayerUpgradeService
{
    int SlimeCountUpgradeLevel { get; }
    int SlimeCountBonus { get; }
    int SlimesPerUpgrade { get; }
    int NextSlimeCountUpgradeCost { get; }
    bool IsSlimeCountUpgradeMaxed { get; }
    event Action Changed;

    int GetStartingSlimeCount(int baseSlimeCount);
    bool TryBuySlimeCountUpgrade();
    void SetSlimeCountUpgradeLevel(int level);
}
