using UnityEngine;

public class BossDefeatView : MonoBehaviour
{
    [SerializeField] private GameObject visualRoot;
    [SerializeField] private GameObject healthLabelRoot;

    private void Awake()
    {
        if (visualRoot == null)
            visualRoot = gameObject;
    }

    public void HideBoss()
    {
        if (visualRoot != null)
            visualRoot.SetActive(false);

        if (healthLabelRoot != null)
            healthLabelRoot.SetActive(false);
    }
}
