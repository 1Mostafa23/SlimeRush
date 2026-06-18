using TMPro;
using UnityEngine;
using Zenject;

public class CrowdCountLabel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text countText;

    private ISlimeCrowd slimeCrowd;

    [Inject]
    private void Construct(ISlimeCrowd slimeCrowd)
    {
        this.slimeCrowd = slimeCrowd;
    }

    private void Awake()
    {
        if (countText == null)
            countText = GetComponentInChildren<TMP_Text>();
    }

    private void OnEnable()
    {
        if (slimeCrowd == null)
            return;

        slimeCrowd.OnSlimeCountChanged += UpdateCountText;
        UpdateCountText(slimeCrowd.SlimeCount);
    }

    private void OnDisable()
    {
        if (slimeCrowd == null)
            return;

        slimeCrowd.OnSlimeCountChanged -= UpdateCountText;
    }

    private void UpdateCountText(int slimeCount)
    {
        if (countText == null)
            return;

        countText.text = slimeCount.ToString();
    }
}
