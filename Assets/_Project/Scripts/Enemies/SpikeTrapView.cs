using UnityEngine;
using Zenject;

public class SpikeTrapView : MonoBehaviour
{
    private SlimeDamageApplier slimeDamageApplier;
    private int slimeLayer;

    [Inject]
    private void Construct(SlimeDamageApplier slimeDamageApplier)
    {
        this.slimeDamageApplier = slimeDamageApplier;
    }

    private void Awake()
    {
        slimeLayer = LayerMask.NameToLayer("Slime");
    }

    private void OnTriggerEnter(Collider other)
    {
        TryDamageSlime(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryDamageSlime(other);
    }

    private void TryDamageSlime(Collider other)
    {
        if (!other.TryGetComponent(out SlimeHitbox slimeHitbox))
            slimeHitbox = other.GetComponentInParent<SlimeHitbox>();

        if (slimeHitbox == null)
        {
            if (other.gameObject.layer == slimeLayer)
                Debug.LogWarning($"SpikeTrapView: Slime layer object '{other.name}' has no SlimeHitbox.");

            return;
        }

        if (slimeDamageApplier == null)
        {
            Debug.LogError("SpikeTrapView: SlimeDamageApplier was not injected.");
            return;
        }

        bool wasKilled = slimeDamageApplier.KillSlime(slimeHitbox);

        if (!wasKilled)
            Debug.LogWarning($"SpikeTrapView: SlimeHitbox '{slimeHitbox.name}' was not found in the active crowd.");
    }
}
