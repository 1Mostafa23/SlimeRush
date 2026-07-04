using TMPro;
using UnityEngine;

public class EnemyPowerView : MonoBehaviour, IEnemyPowerView
{
    [SerializeField] private TMP_Text powerLabel;

    private void Awake()
    {
        if (powerLabel == null)
            powerLabel = GetComponentInChildren<TMP_Text>(true);
    }

    public void SetPower(int currentPower)
    {
        if (powerLabel != null)
            powerLabel.text = currentPower.ToString();

#if UNITY_EDITOR
        if (!Application.isPlaying && powerLabel != null)
            UnityEditor.EditorUtility.SetDirty(powerLabel);
#endif
    }
}
