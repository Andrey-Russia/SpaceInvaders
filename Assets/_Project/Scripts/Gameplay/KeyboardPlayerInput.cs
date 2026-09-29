using UnityEngine.InputSystem;

namespace SpaceInvaders.Gameplay.Input
{
    public sealed class KeyboardPlayerInput : IPlayerInput
    {
        public float Movement
        {
            get
            {
                Keyboard keyboard = Keyboard.current;

                if (keyboard == null)
                {
                    return 0f;
                }

                bool movesLeft = keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed;

                bool movesRight = keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed;

                return (movesRight ? 1f : 0f) - (movesLeft ? 1f : 0f);
            }
        }

        public bool FireHeld => Keyboard.current != null && Keyboard.current.spaceKey.isPressed;

        public bool RestartPressed => Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame;

        public bool PausePressed => Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
    }
}