using System.Threading;
using UnityEngine;

public interface IAILaneJumperEnemyView
{
    AILaneJumperEnemySettings Settings { get; }
    int EnemyPower { get; }
    float ShieldBlockRecoveryDuration { get; }
    IEnemyCombatant Combatant { get; }
    IEnemyClashFeedback ClashFeedback { get; }
    Transform Body { get; }
    Transform Visual { get; }
    Transform LeftLane { get; }
    Transform CenterLane { get; }
    Transform RightLane { get; }
    Transform RootTransform { get; }
    GameObject GameObject { get; }
    CancellationToken DestroyCancellationToken { get; }
    bool IsActiveAndEnabled { get; }

    void SetPowerLabel(int power);
    void ShowWarningAt(float xPosition);
    void HideWarning();
    void DisableClashZone();
    void PlayDefeatFeedback();
}
