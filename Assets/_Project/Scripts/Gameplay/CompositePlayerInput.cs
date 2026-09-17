using UnityEngine;

namespace SpaceInvaders.Gameplay.Input
{
    public sealed class CompositePlayerInput : IPlayerInput
    {
        private readonly IPlayerInput[] _inputs;

        public CompositePlayerInput(params IPlayerInput[] inputs)
        {
            _inputs = inputs;
        }

        public float Movement
        {
            get
            {
                float movement = 0f;

                foreach (IPlayerInput input in _inputs)
                {
                    movement += input.Movement;
                }

                return Mathf.Clamp(movement, -1f, 1f);
            }
        }

        public bool FireHeld
        {
            get
            {
                foreach (IPlayerInput input in _inputs)
                {
                    if (input.FireHeld)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public bool RestartPressed
        {
            get
            {
                foreach (IPlayerInput input in _inputs)
                {
                    if (input.RestartPressed)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}