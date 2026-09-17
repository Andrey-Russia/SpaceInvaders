namespace SpaceInvaders.Gameplay.Input
{
    public interface IPlayerInput
    {
        float Movement { get; }
        bool FirePressed { get; }
        bool RestartPressed { get; }
    }
}