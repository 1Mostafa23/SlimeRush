using UnityEngine;

public class AILaneJumperVisualAnimator : IAILaneJumperVisualAnimator
{
    private Transform visual;
    private AILaneJumperEnemySettings settings;
    private Vector3 baseLocalPosition;
    private Vector3 baseLocalScale = Vector3.one;
    private float animationTime;

    public void Configure(Transform visual, AILaneJumperEnemySettings settings)
    {
        this.visual = visual;
        this.settings = settings;

        if (visual == null)
            return;

        baseLocalPosition = visual.localPosition;
        baseLocalScale = visual.localScale;
    }

    public void Tick(float deltaTime)
    {
        if (visual == null || settings == null)
            return;

        animationTime += deltaTime;

        float rawWave = Mathf.Sin(animationTime * settings.BounceFrequency * Mathf.PI * 2f);
        float smoothWave = Mathf.SmoothStep(0f, 1f, (rawWave + 1f) * 0.5f);
        float bounce = smoothWave * settings.BounceHeight;
        float squash = smoothWave * settings.SquashAmount;

        Vector3 targetPosition = baseLocalPosition + Vector3.up * bounce;
        Vector3 targetScale = new(
            baseLocalScale.x * (1f + squash),
            baseLocalScale.y * (1f - squash),
            baseLocalScale.z * (1f + squash)
        );

        float smoothAmount = 1f - Mathf.Exp(-settings.VisualSmoothSpeed * deltaTime);
        visual.localPosition = Vector3.Lerp(visual.localPosition, targetPosition, smoothAmount);
        visual.localScale = Vector3.Lerp(visual.localScale, targetScale, smoothAmount);
    }
}
