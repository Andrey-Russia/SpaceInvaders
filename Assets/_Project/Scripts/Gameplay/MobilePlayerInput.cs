using UnityEngine;

namespace SpaceInvaders.Gameplay.Input
{
    public sealed class MobilePlayerInput : MonoBehaviour, IPlayerInput
    {
        private bool _movesLeft;
        private bool _movesRight;
        private bool _fires;

        public float Movement => (_movesRight ? 1f : 0f) - (_movesLeft ? 1f : 0f);

        public bool FireHeld => _fires;

        public bool RestartPressed => false;

        public void SetLeftPressed(bool isPressed)
        {
            _movesLeft = isPressed;
        }

        public void SetRightPressed(bool isPressed)
        {
            _movesRight = isPressed;
        }

        public void SetFirePressed(bool isPressed)
        {
            _fires = isPressed;
        }

        private void OnDisable()
        {
            _movesLeft = false;
            _movesRight = false;
            _fires = false;
        }
    }
}