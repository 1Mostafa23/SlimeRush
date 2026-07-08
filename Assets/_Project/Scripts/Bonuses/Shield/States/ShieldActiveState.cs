using UnityEngine;

public class ShieldActiveState : ShieldStateBase
{
    private const float ConsumeGraceDuration = 0.7f;

    private ShieldConsumedState consumedState;
    private float consumeAllowedTime;

    public void SetConsumedState(ShieldConsumedState consumedState)
    {
        this.consumedState = consumedState;
    }

    public override void Enter()
    {
        consumeAllowedTime = Time.time + ConsumeGraceDuration;
        StateMachine.NotifyActivated();
    }

    public override bool TryConsume()
    {
        if (Time.time < consumeAllowedTime)
            return true;

        StateMachine.ChangeState(consumedState);
        return true;
    }
}
