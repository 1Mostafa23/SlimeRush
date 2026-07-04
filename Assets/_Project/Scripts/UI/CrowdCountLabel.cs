using TMPro;
using UnityEngine;
using Zenject;

public class CrowdCountLabel : MonoBehaviour, ICrowdCountView
{
    [Header("References")]
    [SerializeField] private TMP_Text countText;

    private ISlimeCrowd slimeCrowd;
    private bool isSubscribed;

    [Inject]
    private void Construct(ISlimeCrowd slimeCrowd)
    {
        this.slimeCrowd = slimeCrowd;
        TrySubscribe();
    }

    private void Awake()
    {
        if (countText == null)
            countText = GetComponentInChildren<TMP_Text>();
    }

    private void OnEnable()
    {
        TrySubscribe();
    }

    private void OnDisable()
    {
        if (slimeCrowd == null || !isSubscribed)
            return;

        slimeCrowd.OnSlimeCountChanged -= SetCount;
        isSubscribed = false;
    }

    public void SetCount(int slimeCount)
    {
        if (countText == null)
            return;

        countText.text = slimeCount.ToString();
    }

    private void TrySubscribe()
    {
        if (!isActiveAndEnabled || slimeCrowd == null || isSubscribed)
            return;

        slimeCrowd.OnSlimeCountChanged += SetCount;
        isSubscribed = true;
        SetCount(slimeCrowd.SlimeCount);
    }
}
