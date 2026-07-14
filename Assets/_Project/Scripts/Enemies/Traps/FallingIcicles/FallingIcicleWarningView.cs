using UnityEngine;

public class FallingIcicleWarningView : MonoBehaviour
{
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    private Material materialInstance;

    public static FallingIcicleWarningView Create(Vector3 impactPosition, FallingIcicleTrapSettings settings, Transform parent)
    {
        GameObject warningObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        warningObject.name = "IcicleWarning";

        Collider warningCollider = warningObject.GetComponent<Collider>();
        if (warningCollider != null)
            Destroy(warningCollider);

        warningObject.transform.SetParent(parent, true);
        warningObject.transform.position = new Vector3(
            impactPosition.x,
            impactPosition.y + settings.WarningYOffset,
            impactPosition.z);
        warningObject.transform.localScale = new Vector3(
            settings.ImpactRadius * 2f,
            0.015f,
            (settings.ImpactRadius + settings.ImpactForwardPadding) * 2f);

        FallingIcicleWarningView warningView = warningObject.AddComponent<FallingIcicleWarningView>();
        warningView.ApplyColor(settings.WarningColor);
        return warningView;
    }

    public void Hide()
    {
        Destroy(gameObject);
    }

    private void ApplyColor(Color color)
    {
        Renderer warningRenderer = GetComponent<Renderer>();
        if (warningRenderer == null)
            return;

        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null)
            shader = Shader.Find("Unlit/Color");

        materialInstance = new Material(shader);
        SetMaterialColor(materialInstance, color);
        warningRenderer.sharedMaterial = materialInstance;
    }

    private static void SetMaterialColor(Material material, Color color)
    {
        if (material.HasProperty(BaseColorId))
            material.SetColor(BaseColorId, color);

        if (material.HasProperty(ColorId))
            material.SetColor(ColorId, color);
    }

    private void OnDestroy()
    {
        if (materialInstance != null)
            Destroy(materialInstance);
    }
}
