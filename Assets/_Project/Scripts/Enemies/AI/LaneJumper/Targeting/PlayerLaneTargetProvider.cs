public class PlayerLaneTargetProvider : ILaneTargetProvider
{
    private readonly PlayerCrowdController playerCrowdController;

    public PlayerLaneTargetProvider(PlayerCrowdController playerCrowdController)
    {
        this.playerCrowdController = playerCrowdController;
    }

    public float TargetX => playerCrowdController.transform.position.x;
}
