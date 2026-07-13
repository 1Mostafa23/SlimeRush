using UnityEngine;

public class PlayerCrowdController : MonoBehaviour,
    IPlayerCrowdSpeedProvider,
    IPlayerCrowdMovementController,
    IPlayerCrowdPositionProvider
{
    [Header("Forward Movement")]
    [SerializeField] private float forwardSpeed = 5f;

    [Header("Horizontal Movement")]
    [SerializeField] private float horizontalSpeed = 8f;
    [SerializeField] private float horizontalLimit = 3f;

    private bool isMoving = true;
    private float targetX;

    public float ForwardSpeed => isMoving ? forwardSpeed : 0f;
    public float PositionZ => transform.position.z;

    private void Awake()
    {
        targetX = transform.position.x;
    }

    private void Update()
    {
        if (!isMoving)
            return;

        HandleHorizontalInput();
        MoveForward();
        MoveHorizontal();
    }

    private void HandleHorizontalInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            SetTargetXFromScreenPosition(touch.position);
            return;
        }

#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            SetTargetXFromScreenPosition(Input.mousePosition);
        }
#endif
    }

    private void SetTargetXFromScreenPosition(Vector2 screenPosition)
    {
        float screenPercent = screenPosition.x / Screen.width;
        float normalizedX = Mathf.Lerp(-horizontalLimit, horizontalLimit, screenPercent);

        targetX = Mathf.Clamp(normalizedX, -horizontalLimit, horizontalLimit);
    }

    private void MoveForward()
    {
        Vector3 movement = Vector3.forward * forwardSpeed * Time.deltaTime;
        transform.position += movement;
    }

    private void MoveHorizontal()
    {
        Vector3 currentPosition = transform.position;
        float followAmount = 1f - Mathf.Exp(-horizontalSpeed * Time.deltaTime);

        float newX = Mathf.Lerp(
            currentPosition.x,
            targetX,
            followAmount
        );

        transform.position = new Vector3(
            newX,
            currentPosition.y,
            currentPosition.z
        );
    }

    public void StopMovement()
    {
        SetInputEnabled(false);
    }

    public void StartMovement()
    {
        SetInputEnabled(true);
    }

    public void SetInputEnabled(bool enabled)
    {
        isMoving = enabled;
    }

    public int GetSlimeCount()
    {
        return transform.childCount;
    }
}
