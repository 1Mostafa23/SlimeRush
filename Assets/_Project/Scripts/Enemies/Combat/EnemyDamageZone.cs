using UnityEngine;
using Zenject;

public class EnemyDamageZone : MonoBehaviour
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

    private void TryDamageSlime(Collider other)
    {
        if (!other.TryGetComponent(out SlimeHitbox slimeHitbox))
            slimeHitbox = other.GetComponentInParent<SlimeHitbox>();

        if (slimeHitbox == null)
        {
            if (other.gameObject.layer == slimeLayer)
                Debug.LogWarning($"EnemyDamageZone: Slime layer object '{other.name}' has no SlimeHitbox.");

            return;
        }

        if (slimeDamageApplier == null)
        {
            Debug.LogError("EnemyDamageZone: SlimeDamageApplier was not injected.");
            return;
        }

        SlimeDamageResult damageResult = slimeDamageApplier.TryKillSlime(slimeHitbox);

        if (damageResult == SlimeDamageResult.Blocked)
        {
            GetComponentInParent<IDamageBlockedReaction>()?.OnDamageBlocked();
            return;
        }

        if (damageResult == SlimeDamageResult.NotFound)
            Debug.LogWarning($"EnemyDamageZone: SlimeHitbox '{slimeHitbox.name}' was not found in the active crowd.");
    }
}
