using System.Collections.Generic;
using UnityEngine;

public class CrowdFollowFormationState : ICrowdMovementState
{
    private readonly SlimeCrowdSettings settings;

    public CrowdFollowFormationState(SlimeCrowdSettings settings)
    {
        this.settings = settings;
    }

    public void Enter()
    {
    }

    public void Tick(IReadOnlyList<GameObject> slimes, IReadOnlyList<Vector3> targetLocalPositions, float deltaTime)
    {
        if (targetLocalPositions.Count != slimes.Count)
            return;

        float followAmount = 1f - Mathf.Exp(-settings.FormationFollowSpeed * deltaTime);

        for (int i = 0; i < slimes.Count; i++)
        {
            Transform slimeTransform = slimes[i].transform;
            slimeTransform.localPosition = Vector3.Lerp(
                slimeTransform.localPosition,
                targetLocalPositions[i],
                followAmount
            );
            slimeTransform.localRotation = Quaternion.identity;
        }
    }

    public void Exit()
    {
    }
}
