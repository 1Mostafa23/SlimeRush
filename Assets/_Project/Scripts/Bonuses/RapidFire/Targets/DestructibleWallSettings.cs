using UnityEngine;

[CreateAssetMenu(
    fileName = "DestructibleWallSettings",
    menuName = "SlimeRush/Obstacles/Destructible Wall Settings")]
public class DestructibleWallSettings : ScriptableObject
{
    [SerializeField] private int maxHp = 30;
    [SerializeField] private bool damageSlimesOnContact = true;

    public int MaxHp => Mathf.Max(1, maxHp);
    public bool DamageSlimesOnContact => damageSlimesOnContact;
}
