using UnityEngine;
using Zenject;
using System.Collections.Generic;

public class BossDetectionZone : MonoBehaviour
{
    private readonly Dictionary<PlayerCrowdMarker, int> playerCrowdOverlaps = new();

    private IBossStateMachine bossStateMachine;

    [Inject]
    private void Construct(IBossStateMachine bossStateMachine)
    {
        this.bossStateMachine = bossStateMachine;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerCrowdMarker marker = ResolvePlayerCrowdMarker(other);

        if (marker == null)
            return;

        bool wasEmpty = playerCrowdOverlaps.Count == 0;

        if (!playerCrowdOverlaps.TryAdd(marker, 1))
            playerCrowdOverlaps[marker]++;

        if (wasEmpty)
            bossStateMachine.StartRangedPhase();
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerCrowdMarker marker = ResolvePlayerCrowdMarker(other);

        if (marker == null || !playerCrowdOverlaps.TryGetValue(marker, out int overlapCount))
            return;

        overlapCount--;

        if (overlapCount <= 0)
            playerCrowdOverlaps.Remove(marker);
        else
            playerCrowdOverlaps[marker] = overlapCount;

        if (playerCrowdOverlaps.Count == 0)
            bossStateMachine.StopRangedPhase();
    }

    private PlayerCrowdMarker ResolvePlayerCrowdMarker(Collider other)
    {
        if (other.TryGetComponent(out PlayerCrowdMarker marker))
            return marker;

        return other.GetComponentInParent<PlayerCrowdMarker>();
    }
}
