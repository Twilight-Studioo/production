namespace Feature.Interface
{
    public delegate void PlayerStateChangeHandler(PlayerStateEvent state);

    public enum PlayerStateEvent
    {
        Idle,
        SwapStart,
        SwapExec,
        SwapCancel,
    }
}