using UnityEngine;

public class BossCameraController : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] private Transform bossCameraPoint;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 6f;

    private bool isFocused;
    private PlayerCrowdController playerCrowdController;

    [Zenject.Inject]
    private void Construct(PlayerCrowdController playerCrowdController)
    {
        this.playerCrowdController = playerCrowdController;
    }

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera != null && cameraFollow == null)
            cameraFollow = targetCamera.GetComponent<CameraFollow>();
    }

    private void LateUpdate()
    {
        if (!isFocused || targetCamera == null || bossCameraPoint == null)
            return;

        Transform cameraTransform = targetCamera.transform;

        cameraTransform.position = Vector3.Lerp(
            cameraTransform.position,
            bossCameraPoint.position,
            moveSpeed * Time.deltaTime);

        cameraTransform.rotation = Quaternion.Slerp(
            cameraTransform.rotation,
            bossCameraPoint.rotation,
            rotationSpeed * Time.deltaTime);
    }

    public void FocusOnBoss()
    {
        if (cameraFollow != null)
            cameraFollow.enabled = false;

        isFocused = true;
    }

    public void StopFocus()
    {
        isFocused = false;

        if (cameraFollow != null)
        {
            if (playerCrowdController != null)
                cameraFollow.SetTarget(playerCrowdController.transform);

            cameraFollow.enabled = true;
        }
    }
}
