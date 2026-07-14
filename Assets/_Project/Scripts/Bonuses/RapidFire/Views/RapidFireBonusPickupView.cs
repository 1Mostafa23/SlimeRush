using UnityEngine;
using Zenject;

public class RapidFireBonusPickupView : MonoBehaviour
{
    [SerializeField] private float duration = 2f;

    private IRapidFireService rapidFireService;

    [Inject]
    private void Construct(IRapidFireService rapidFireService)
    {
        this.rapidFireService = rapidFireService;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PlayerCrowdMarker _) &&
            other.GetComponentInParent<PlayerCrowdMarker>() == null)
        {
            return;
        }

        rapidFireService.Activate(duration);
        gameObject.SetActive(false);
    }
}
