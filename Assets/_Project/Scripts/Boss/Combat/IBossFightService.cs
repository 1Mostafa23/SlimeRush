using UnityEngine;

public interface IBossFightService
{
    void StartCloseFight(Transform fightPoint, Collider bossTrigger);
    void StopCloseFight(bool resumePlayerMovement);
}
