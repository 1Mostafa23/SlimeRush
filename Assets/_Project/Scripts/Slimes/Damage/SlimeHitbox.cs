using UnityEngine;

public class SlimeHitbox : MonoBehaviour
{
    [SerializeField] private Transform slimeRoot;

    public GameObject SlimeObject => slimeRoot != null ? slimeRoot.gameObject : gameObject;

    private void Awake()
    {
        if (slimeRoot == null)
            slimeRoot = transform;
    }
}
