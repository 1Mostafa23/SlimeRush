using System.Collections;
using UnityEngine;

public class BossHitFeedback : MonoBehaviour
{
    [SerializeField] private Transform visual;
    [SerializeField] private float hitScaleMultiplier = 1.08f;
    [SerializeField] private float hitDuration = 0.08f;

    private Coroutine hitRoutine;
    private Vector3 initialScale;

    private void Awake()
    {
        if (visual == null)
            visual = transform;

        initialScale = visual.localScale;
    }

    public void PlayHit()
    {
        if (!isActiveAndEnabled || visual == null)
            return;

        if (hitRoutine != null)
            StopCoroutine(hitRoutine);

        hitRoutine = StartCoroutine(PlayHitRoutine());
    }

    private IEnumerator PlayHitRoutine()
    {
        visual.localScale = initialScale * hitScaleMultiplier;
        yield return new WaitForSeconds(hitDuration);
        visual.localScale = initialScale;
        hitRoutine = null;
    }
}
