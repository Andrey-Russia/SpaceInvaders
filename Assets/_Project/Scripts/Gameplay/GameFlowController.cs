using SpaceInvaders.Core;
using SpaceInvaders.Gameplay.Input;
using UnityEngine;

namespace SpaceInvaders.Gameplay.Flow
{
    public sealed class GameFlowController : MonoBehaviour
    {
        private IPlayerInput _input;
        private bool _isInitialized;

        public void Initialize(IPlayerInput input)
        {
            _input = input;
            _isInitialized = true;
        }

        private void Update()
        {
            if (!_isInitialized)
            {
                return;
            }

            if (_input.RestartPressed)
            {
                RestartGame();
            }
        }

        public void RestartGame()
        {
            GameBootstrapper.Instance.RestartGame();
        }

        public void ReturnToMainMenu()
        {
            GameBootstrapper.Instance.LoadMainMenu();
        }
    }
}