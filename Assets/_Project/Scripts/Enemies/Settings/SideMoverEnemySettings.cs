using UnityEngine;

[CreateAssetMenu(
    fileName = "SideMoverEnemySettings",
    menuName = "SlimeRush/Enemies/Side Mover Enemy Settings")]
public class SideMoverEnemySettings : ScriptableObject
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float bounceHeight = 0.12f;
    [SerializeField] private float bounceFrequency = 2.5f;
    [SerializeField] private float squashAmount = 0.06f;
    [SerializeField] private float visualSmoothSpeed = 14f;

    public float MoveSpeed => moveSpeed;
    public float BounceHeight => bounceHeight;
    public float BounceFrequency => bounceFrequency;
    public float SquashAmount => squashAmount;
    public float VisualSmoothSpeed => visualSmoothSpeed;
}
