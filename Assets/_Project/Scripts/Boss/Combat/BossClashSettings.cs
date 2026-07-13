using UnityEngine;

[CreateAssetMenu(
    fileName = "BossClashSettings",
    menuName = "SlimeRush/Boss/Boss Clash Settings")]
public class BossClashSettings : ScriptableObject
{
    [Header("Damage")]
    [SerializeField] private int slimeDamagePerTick = 5;
    [SerializeField] private int bossDamagePerTick = 5;

    [Header("Timing")]
    [SerializeField] private float fightTickInterval = 0.5f;

    public int SlimeDamagePerTick => Mathf.Max(1, slimeDamagePerTick);
    public int BossDamagePerTick => Mathf.Max(1, bossDamagePerTick);
    public float FightTickInterval => Mathf.Max(0.05f, fightTickInterval);
}
