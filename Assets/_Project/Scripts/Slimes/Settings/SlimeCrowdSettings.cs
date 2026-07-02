using UnityEngine;

[CreateAssetMenu(
    fileName = "SlimeCrowdSettings",
    menuName = "SlimeRush/Slimes/Slime Crowd Settings")]
public class SlimeCrowdSettings : ScriptableObject
{
    [Header("Starting Crowd")]
    [SerializeField] private int startingSlimeCount = 5;

    [Header("Movement")]
    [SerializeField] private float formationFollowSpeed = 12f;
    [SerializeField] private float damageFormationRebuildDelay = 0.7f;

    public int StartingSlimeCount => startingSlimeCount;
    public float FormationFollowSpeed => formationFollowSpeed;
    public float DamageFormationRebuildDelay => damageFormationRebuildDelay;
}
