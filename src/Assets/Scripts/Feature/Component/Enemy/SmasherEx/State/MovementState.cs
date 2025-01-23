namespace Feature.Component.Enemy.SmasherEx.State
{
    internal abstract record MovementState
    {
        public static MovementState Standby = new Standby();
        public static MovementState ChargeForward = new ChargeForward();
        public static MovementState ThrowingMines = new ThrowingMines();
        public static MovementState ForwardBlow = new ForwardBlow();
        public static MovementState MoveToNearPlayerWithAgent = new MoveToNearPlayerWithAgent();
        public static MovementState MoveToLeavePlayerWithAgent = new MoveToLeavePlayerWithAgent();
    }

    internal record Standby : MovementState;

    internal record ChargeForward : MovementState;

    internal record ThrowingMines : MovementState;

    internal record ForwardBlow : MovementState;

    internal record MoveToNearPlayerWithAgent : MovementState;

    internal record MoveToLeavePlayerWithAgent : MovementState;
}