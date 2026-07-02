using UnityEngine;

[CreateAssetMenu(
    fileName = "CrowdFormationSettings",
    menuName = "SlimeRush/Slimes/Crowd Formation Settings")]
public class CrowdFormationSettings : ScriptableObject
{
    [Header("Crowd Settings")]
    [SerializeField] private float baseSpacing = 1.0f;
    [SerializeField] private float ellipseWidth = 1.15f;
    [SerializeField] private float ellipseDepth = 0.9f;
    [SerializeField] private float frontOffsetZ = -0.35f;
    [SerializeField] private float rowSpacing = 0.75f;
    [SerializeField] private int maxRows = 8;
    [SerializeField] private int maxRowWidth = 10;
    [SerializeField] private int widestRowIndex = 3;

    [Header("Organic Look")]
    [SerializeField] private float randomOffsetAmount = 0.055f;

    public float BaseSpacing => baseSpacing;
    public float EllipseWidth => ellipseWidth;
    public float EllipseDepth => ellipseDepth;
    public float FrontOffsetZ => frontOffsetZ;
    public float RowSpacing => rowSpacing;
    public int MaxRows => maxRows;
    public int MaxRowWidth => maxRowWidth;
    public int WidestRowIndex => widestRowIndex;
    public float RandomOffsetAmount => randomOffsetAmount;
}
