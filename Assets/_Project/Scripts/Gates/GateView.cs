using UnityEngine;
using Zenject;

public class GateView : MonoBehaviour
{
    [SerializeField] private GateOperationType operationType;
    [SerializeField] private int value = 1;
    [SerializeField] private GateGroupView gateGroupView;
    [SerializeField] private GateVisualView visualView;

    private IGateEffectApplier gateEffectApplier;
    private bool isUsed;

    [Inject]
    private void Construct(IGateEffectApplier gateEffectApplier)
    {
        this.gateEffectApplier = gateEffectApplier;
    }

    private void Awake()
    {
        if (gateGroupView == null)
            gateGroupView = GetComponentInParent<GateGroupView>();

        if (visualView == null)
            visualView = GetComponentInChildren<GateVisualView>();
    }

    private void Start()
    {
        UpdateVisualLabel();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isUsed)
            return;

        if (gateGroupView != null && !gateGroupView.CanUseGate())
            return;

        if (!other.TryGetComponent(out PlayerCrowdController _))
            return;

        if (gateEffectApplier == null)
        {
            Debug.LogError("GateView: GateEffectApplier was not injected.");
            return;
        }

        gateEffectApplier.Apply(operationType, value);

        if (gateGroupView != null)
            gateGroupView.MarkUsed();

        DisableGate();
    }

    private void DisableGate()
    {
        isUsed = true;
        gameObject.SetActive(false);
    }

    private void OnValidate()
    {
        if (value < 0)
            value = 0;

        if (gateGroupView == null)
            gateGroupView = GetComponentInParent<GateGroupView>();

        if (visualView == null)
            visualView = GetComponentInChildren<GateVisualView>();

        if (visualView != null)
            visualView.SetLabel(GetEditorDisplayText());
    }

    private void UpdateVisualLabel()
    {
        if (visualView == null || gateEffectApplier == null)
            return;

        visualView.SetLabel(gateEffectApplier.GetDisplayText(operationType, value));
    }

    private string GetEditorDisplayText()
    {
        return operationType switch
        {
            GateOperationType.Add => $"+{value}",
            GateOperationType.Multiply => $"x{Mathf.Max(1, value)}",
            GateOperationType.Subtract => $"-{value}",
            _ => value.ToString()
        };
    }
}
