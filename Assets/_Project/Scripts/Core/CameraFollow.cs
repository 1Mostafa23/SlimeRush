using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Follow Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 7f, -10f);
    [SerializeField] private float smoothSpeed = 8f;

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desiredPosition = target.position + offset;
        float followAmount = 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime);

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followAmount
        );
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
