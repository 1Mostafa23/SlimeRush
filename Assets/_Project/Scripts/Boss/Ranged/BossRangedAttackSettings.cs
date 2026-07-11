using UnityEngine;

[CreateAssetMenu(
    fileName = "BossRangedAttackSettings",
    menuName = "SlimeRush/Boss/Boss Ranged Attack Settings")]
public class BossRangedAttackSettings : ScriptableObject
{
    [Header("Shooting")]
    [SerializeField] private float fireInterval = 1.5f;

    [Header("Projectile")]
    [SerializeField] private float projectileSpeed = 12f;
    [SerializeField] private float projectileLifetime = 30f;
    [SerializeField] private float projectileMaxDistance = 200f;

    [Header("Pool")]
    [SerializeField] private int projectilePoolInitialSize = 12;

    [Header("Feedback")]
    [SerializeField] private float preFireScaleMultiplier = 1.08f;
    [SerializeField] private float preFireDuration = 0.08f;

    public float FireInterval => fireInterval;
    public float ProjectileSpeed => projectileSpeed;
    public float ProjectileLifetime => projectileLifetime;
    public float ProjectileMaxDistance => projectileMaxDistance;
    public int ProjectilePoolInitialSize => projectilePoolInitialSize;
    public float PreFireScaleMultiplier => preFireScaleMultiplier;
    public float PreFireDuration => preFireDuration;
}
