using System.Collections.Generic;
using UnityEngine;

public class NearestPlayerLaneSelector : IEnemyLaneSelector
{
    private readonly ILaneTargetProvider laneTargetProvider;

    public NearestPlayerLaneSelector(ILaneTargetProvider laneTargetProvider)
    {
        this.laneTargetProvider = laneTargetProvider;
    }

    public Transform SelectLane(IReadOnlyList<Transform> lanes)
    {
        Transform nearestLane = null;
        float nearestDistance = float.MaxValue;
        float targetX = laneTargetProvider.TargetX;

        for (int i = 0; i < lanes.Count; i++)
        {
            Transform lane = lanes[i];

            if (lane == null)
                continue;

            float distance = Mathf.Abs(lane.position.x - targetX);

            if (distance >= nearestDistance)
                continue;

            nearestDistance = distance;
            nearestLane = lane;
        }

        return nearestLane;
    }
}
