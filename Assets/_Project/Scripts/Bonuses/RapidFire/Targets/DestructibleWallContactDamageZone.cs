using UnityEngine;
using Zenject;

public class DestructibleWallContactDamageZone : MonoBehaviour
{
    [SerializeField] private DestructibleWallView wallView;

    private SlimeDamageApplier slimeDamageApplier;
    private int slimeLayer;

    [Inject]
    private void Construct(SlimeDamageApplier slimeDamageApplier)
    {
        this.slimeDamageApplier = slimeDamageApplier;
    }

    private void Awake()
    {
        if (wallView == null)
            wallView = GetComponentInParent<DestructibleWallView>();

        slimeLayer = LayerMask.NameToLayer("Slime");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (wallView == null || wallView.IsDestroyed || !wallView.DamageSlimesOnContact)
            return;

        TryDamageSlime(other);
    }

    private void TryDamageSlime(Collider other)
    {
        if (!other.TryGetComponent(out SlimeHitbox slimeHitbox))
            slimeHitbox = other.GetComponentInParent<SlimeHitbox>();

        if (slimeHitbox == null)
        {
            if (other.gameObject.layer == slimeLayer)
                Debug.LogWarning($"DestructibleWallContactDamageZone: Slime layer object '{other.name}' has no SlimeHitbox.");

            return;
        }

        if (slimeDamageApplier == null)
        {
            Debug.LogError("DestructibleWallContactDamageZone: SlimeDamageApplier was not injected.");
            return;
        }

        SlimeDamageResult damageResult = slimeDamageApplier.TryKillSlime(slimeHitbox);

        if (damageResult == SlimeDamageResult.NotFound)
            Debug.LogWarning($"DestructibleWallContactDamageZone: SlimeHitbox '{slimeHitbox.name}' was not found in the active crowd.");
    }
}
