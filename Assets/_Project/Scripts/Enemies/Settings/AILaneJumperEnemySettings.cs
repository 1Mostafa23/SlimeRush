using UnityEngine;
using System;

[CreateAssetMenu(
    fileName = "AILaneJumperEnemySettings",
    menuName = "SlimeRush/Enemies/AI Lane Jumper Enemy Settings")]
public class AILaneJumperEnemySettings : ScriptableObject, IEnemyPowerSettings
{
    public static event Action<AILaneJumperEnemySettings> SettingsChanged;

    [Header("Patrol")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float patrolDuration = 1.5f;
    [SerializeField] private float laneReachDistance = 0.05f;

    [Header("Attack")]
    [SerializeField] private float warningDuration = 0.7f;
    [SerializeField] private float dashSpeed = 8f;
    [SerializeField] private float dashPlayerSpeedMultiplier = 0.75f;
    [SerializeField] private float minDashSpeed = 6f;
    [SerializeField] private float maxDashSpeed = 14f;
    [SerializeField] private float jumpHeight = 0.6f;

    [Header("Clash")]
    [SerializeField] private int enemyPower = 5;
    [SerializeField] private float clashTickInterval = 0.35f;
    [SerializeField] private float defeatDeactivateDelay = 0.35f;

    [Header("Lifecycle")]
    [SerializeField] private float playerPassedZOffset = 2f;

    [Header("Visual")]
    [SerializeField] private float bounceHeight = 0.1f;
    [SerializeField] private float bounceFrequency = 2.5f;
    [SerializeField] private float squashAmount = 0.06f;
    [SerializeField] private float visualSmoothSpeed = 14f;

    public float PatrolSpeed => patrolSpeed;
    public float PatrolDuration => patrolDuration;
    public float LaneReachDistance => laneReachDistance;
    public float WarningDuration => warningDuration;
    public float DashSpeed => dashSpeed;
    public float DashPlayerSpeedMultiplier => dashPlayerSpeedMultiplier;
    public float MinDashSpeed => minDashSpeed;
    public float MaxDashSpeed => maxDashSpeed;
    public float JumpHeight => jumpHeight;
    public int EnemyPower => enemyPower;
    public float ClashTickInterval => clashTickInterval;
    public float DefeatDeactivateDelay => defeatDeactivateDelay;
    public float PlayerPassedZOffset => playerPassedZOffset;
    public float BounceHeight => bounceHeight;
    public float BounceFrequency => bounceFrequency;
    public float SquashAmount => squashAmount;
    public float VisualSmoothSpeed => visualSmoothSpeed;

    private void OnValidate()
    {
        enemyPower = Mathf.Max(1, enemyPower);
        patrolDuration = Mathf.Max(0f, patrolDuration);
        dashSpeed = Mathf.Max(0f, dashSpeed);
        dashPlayerSpeedMultiplier = Mathf.Max(0f, dashPlayerSpeedMultiplier);
        minDashSpeed = Mathf.Max(0f, minDashSpeed);
        maxDashSpeed = Mathf.Max(minDashSpeed, maxDashSpeed);
        SettingsChanged?.Invoke(this);
    }
}
