namespace SpaceInvaders.Gameplay.Input
{
    public interface IPlayerInput
    {
        float Movement { get; }
        bool FireHeld { get; }
        bool RestartPressed { get; }
        bool PausePressed { get; }
    }
}