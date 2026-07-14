using UnityEngine;

[CreateAssetMenu(
    fileName = "FallingIcicleTrapSettings",
    menuName = "SlimeRush/Traps/Falling Icicle Trap Settings")]
public class FallingIcicleTrapSettings : ScriptableObject
{
    [Header("Pattern")]
    [SerializeField] private float warningDuration = 0.65f;
    [SerializeField] private float waveInterval = 0.35f;

    [Header("Icicle")]
    [SerializeField] private float spawnHeight = 9f;
    [SerializeField] private float fallSpeed = 16f;
    [SerializeField] private float icicleScaleMultiplier = 1.5f;
    [SerializeField] private float impactY = 0.15f;

    [Header("Damage")]
    [SerializeField] private float impactRadius = 0.75f;
    [SerializeField] private float impactForwardPadding = 0.75f;
    [SerializeField] private float impactDamageHeight = 1.6f;

    [Header("Warning")]
    [SerializeField] private float warningYOffset = 0.03f;
    [SerializeField] private Color warningColor = new(1f, 0f, 0f, 0.45f);

    public float WarningDuration => Mathf.Max(0.05f, warningDuration);
    public float WaveInterval => Mathf.Max(0f, waveInterval);
    public float SpawnHeight => Mathf.Max(0.1f, spawnHeight);
    public float FallSpeed => Mathf.Max(0.1f, fallSpeed);
    public float IcicleScaleMultiplier => Mathf.Max(0.1f, icicleScaleMultiplier);
    public float ImpactY => impactY;
    public float ImpactRadius => Mathf.Max(0.05f, impactRadius);
    public float ImpactForwardPadding => Mathf.Max(0f, impactForwardPadding);
    public float ImpactDamageHeight => Mathf.Max(0.1f, impactDamageHeight);
    public float WarningYOffset => warningYOffset;
    public Color WarningColor => warningColor;
}
