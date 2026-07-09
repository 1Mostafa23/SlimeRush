using UnityEngine;

public class ShieldBonusFloatingVisual : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float rotationSpeed = 45f;
    [SerializeField] private float bobAmplitude = 0.18f;
    [SerializeField] private float bobFrequency = 1.25f;

    private Vector3 startLocalPosition;
    private float timeOffset;

    private void Awake()
    {
        if (target == null)
            target = transform;

        startLocalPosition = target.localPosition;
        timeOffset = Random.value * Mathf.PI * 2f;
    }

    private void OnEnable()
    {
        if (target != null)
            startLocalPosition = target.localPosition;
    }

    private void Update()
    {
        if (target == null)
            return;

        float bobOffset = Mathf.Sin(Time.time * bobFrequency + timeOffset) * bobAmplitude;
        target.localPosition = startLocalPosition + Vector3.up * bobOffset;
        target.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
    }
}
