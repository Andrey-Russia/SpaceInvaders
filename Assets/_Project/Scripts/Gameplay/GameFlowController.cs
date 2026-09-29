using SpaceInvaders.Core;
using SpaceInvaders.Gameplay.Input;
using SpaceInvaders.Gameplay.State;
using UnityEngine;

namespace SpaceInvaders.Gameplay.Flow
{
    public sealed class GameFlowController : MonoBehaviour
    {
        private IPlayerInput _input;
        private GameStateController _gameStateController;
        private bool _isInitialized;

        public void Initialize(
            IPlayerInput input,
            GameStateController gameStateController)
        {
            _input = input;
            _gameStateController = gameStateController;
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
                return;
            }

            if (_input.PausePressed)
            {
                TogglePause();
            }
        }

        public void TogglePause()
        {
            if (_gameStateController.CurrentState == GameState.Playing)
            {
                PauseGame();
            }
            else if (
                _gameStateController.CurrentState == GameState.Paused)
            {
                ResumeGame();
            }
        }

        public void PauseGame()
        {
            _gameStateController.Pause();
        }

        public void ResumeGame()
        {
            _gameStateController.Resume();
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