using UnityEngine;

public class AdaptiveEnemyDashSpeedProvider : IEnemyDashSpeedProvider
{
    private readonly IPlayerCrowdSpeedProvider playerSpeedProvider;

    public AdaptiveEnemyDashSpeedProvider(IPlayerCrowdSpeedProvider playerSpeedProvider)
    {
        this.playerSpeedProvider = playerSpeedProvider;
    }

    public float GetDashSpeed(AILaneJumperEnemySettings settings)
    {
        if (settings == null)
            return 0f;

        float playerSpeed = playerSpeedProvider != null ? playerSpeedProvider.ForwardSpeed : 0f;
        float adaptiveSpeed = settings.DashSpeed + playerSpeed * settings.DashPlayerSpeedMultiplier;

        return Mathf.Clamp(adaptiveSpeed, settings.MinDashSpeed, settings.MaxDashSpeed);
    }
}
