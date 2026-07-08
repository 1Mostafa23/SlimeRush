using UnityEngine;
using Zenject;

public class ShieldBonusPickupView : MonoBehaviour
{
    private IShieldService shieldService;

    [Inject]
    private void Construct(IShieldService shieldService)
    {
        this.shieldService = shieldService;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PlayerCrowdMarker _) &&
            other.GetComponentInParent<PlayerCrowdMarker>() == null)
        {
            return;
        }

        shieldService.Activate();
        gameObject.SetActive(false);
    }
}
