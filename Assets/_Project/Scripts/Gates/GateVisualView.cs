using TMPro;
using UnityEngine;

public class GateVisualView : MonoBehaviour
{
    [SerializeField] private TMP_Text labelText;

    private void Awake()
    {
        if (labelText == null)
            labelText = GetComponentInChildren<TMP_Text>();
    }

    public void SetLabel(string label)
    {
        if (labelText == null)
            return;

        labelText.text = label;
    }
}
