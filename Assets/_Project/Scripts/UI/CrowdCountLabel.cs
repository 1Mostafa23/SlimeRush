using TMPro;
using UnityEngine;

public class CrowdCountLabel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SlimeCrowdManager slimeCrowdManager;
    [SerializeField] private TMP_Text countText;

    private void Awake()
    {
        if (countText == null)
            countText = GetComponentInChildren<TMP_Text>();
    }

    private void OnEnable()
    {
        if (slimeCrowdManager == null)
            return;

        slimeCrowdManager.OnSlimeCountChanged += UpdateCountText;
        UpdateCountText(slimeCrowdManager.SlimeCount);
    }

    private void OnDisable()
    {
        if (slimeCrowdManager == null)
            return;

        slimeCrowdManager.OnSlimeCountChanged -= UpdateCountText;
    }

    private void UpdateCountText(int slimeCount)
    {
        if (countText == null)
            return;

        countText.text = slimeCount.ToString();
    }
}