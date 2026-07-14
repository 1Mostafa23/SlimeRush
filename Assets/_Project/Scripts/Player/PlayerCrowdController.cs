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
    [SerializeField, Min(0.1f)] private float horizontalInputSensitivity = 1.35f;
    [SerializeField, Min(0f)] private float dragStartThresholdPixels = 12f;

    private bool isMoving = true;
    private bool isTouchDragging;
    private bool isMouseDragging;
    private Vector2 touchStartPosition;
    private Vector2 mouseStartPosition;
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
            HandleTouchInput(touch);
            return;
        }

        isTouchDragging = false;

#if UNITY_EDITOR
        HandleMouseInput();
#endif
    }

    private void HandleTouchInput(Touch touch)
    {
        if (touch.phase == TouchPhase.Began)
        {
            touchStartPosition = touch.position;
            isTouchDragging = false;
            return;
        }

        if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            isTouchDragging = false;
            return;
        }

        if (!isTouchDragging)
            isTouchDragging = Vector2.Distance(touch.position, touchStartPosition) >= dragStartThresholdPixels;

        if (isTouchDragging)
            SetTargetXFromScreenPosition(touch.position);
    }

#if UNITY_EDITOR
    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseStartPosition = Input.mousePosition;
            isMouseDragging = false;
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isMouseDragging = false;
            return;
        }

        if (!Input.GetMouseButton(0))
            return;

        Vector2 mousePosition = Input.mousePosition;

        if (!isMouseDragging)
            isMouseDragging = Vector2.Distance(mousePosition, mouseStartPosition) >= dragStartThresholdPixels;

        if (isMouseDragging)
            SetTargetXFromScreenPosition(mousePosition);
    }
#endif

    private void SetTargetXFromScreenPosition(Vector2 screenPosition)
    {
        float screenPercent = screenPosition.x / Screen.width;
        screenPercent = Mathf.Clamp01(0.5f + (screenPercent - 0.5f) * horizontalInputSensitivity);
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
