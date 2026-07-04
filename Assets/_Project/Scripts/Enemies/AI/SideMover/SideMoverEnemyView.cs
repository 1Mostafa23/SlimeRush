using UnityEngine;
using Zenject;

public class SideMoverEnemyView : MonoBehaviour, ISideMoverEnemy
{
    [SerializeField] private Transform body;
    [SerializeField] private Transform visual;
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;
    [SerializeField] private SideMoverEnemySettings settings;

    private SideMoverEnemySystem sideMoverEnemySystem;
    private Vector3 baseVisualLocalPosition;
    private Vector3 baseVisualLocalScale = Vector3.one;
    private float animationTime;
    private bool isMovingRight = true;
    private bool isRegistered;

    [Inject]
    private void Construct(SideMoverEnemySystem sideMoverEnemySystem)
    {
        this.sideMoverEnemySystem = sideMoverEnemySystem;
        TryRegister();
    }

    private void Awake()
    {
        if (visual != null)
        {
            baseVisualLocalPosition = visual.localPosition;
            baseVisualLocalScale = visual.localScale;
        }
    }

    private void OnEnable()
    {
        TryRegister();
    }

    private void OnDisable()
    {
        if (sideMoverEnemySystem == null || !isRegistered)
            return;

        sideMoverEnemySystem.Unregister(this);
        isRegistered = false;
    }

    public void Tick(float deltaTime)
    {
        if (body == null || visual == null || leftPoint == null || rightPoint == null || settings == null)
            return;

        MoveBody(deltaTime);
        AnimateVisual(deltaTime);
    }

    private void MoveBody(float deltaTime)
    {
        Transform targetPoint = isMovingRight ? rightPoint : leftPoint;

        body.position = Vector3.MoveTowards(
            body.position,
            targetPoint.position,
            settings.MoveSpeed * deltaTime
        );

        if (Vector3.SqrMagnitude(body.position - targetPoint.position) <= 0.0001f)
            isMovingRight = !isMovingRight;
    }

    private void AnimateVisual(float deltaTime)
    {
        animationTime += deltaTime;

        float rawWave = Mathf.Sin(animationTime * settings.BounceFrequency * Mathf.PI * 2f);
        float smoothWave = Mathf.SmoothStep(0f, 1f, (rawWave + 1f) * 0.5f);
        float bounce = smoothWave * settings.BounceHeight;
        float squash = smoothWave * settings.SquashAmount;

        Vector3 targetPosition = baseVisualLocalPosition + Vector3.up * bounce;
        Vector3 targetScale = new(
            baseVisualLocalScale.x * (1f + squash),
            baseVisualLocalScale.y * (1f - squash),
            baseVisualLocalScale.z * (1f + squash)
        );

        float smoothAmount = 1f - Mathf.Exp(-settings.VisualSmoothSpeed * deltaTime);
        visual.localPosition = Vector3.Lerp(visual.localPosition, targetPosition, smoothAmount);
        visual.localScale = Vector3.Lerp(visual.localScale, targetScale, smoothAmount);
    }

    private void TryRegister()
    {
        if (!isActiveAndEnabled || sideMoverEnemySystem == null || isRegistered)
            return;

        sideMoverEnemySystem.Register(this);
        isRegistered = true;
    }
}
