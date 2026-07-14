using UnityEngine;

[CreateAssetMenu(
    fileName = "SlimeCountUpgradeSettings",
    menuName = "SlimeRush/Upgrades/Slime Count Upgrade Settings")]
public class SlimeCountUpgradeSettings : ScriptableObject
{
    [SerializeField] private int baseCost = 250;
    [SerializeField] private float costGrowthMultiplier = 1.45f;
    [SerializeField] private int quadraticCostPerLevel = 50;
    [SerializeField] private int slimesPerUpgrade = 1;
    [SerializeField] private int maxUpgradeLevel = 40;

    public int SlimesPerUpgrade => Mathf.Max(1, slimesPerUpgrade);
    public int MaxUpgradeLevel => Mathf.Max(1, maxUpgradeLevel);

    public int GetCost(int currentLevel)
    {
        int safeLevel = Mathf.Max(0, currentLevel);
        float exponentialCost = Mathf.Max(1, baseCost) * Mathf.Pow(Mathf.Max(1f, costGrowthMultiplier), safeLevel);
        int rawCost = Mathf.CeilToInt(exponentialCost + quadraticCostPerLevel * safeLevel * safeLevel);
        return Mathf.CeilToInt(rawCost / 10f) * 10;
    }
}
