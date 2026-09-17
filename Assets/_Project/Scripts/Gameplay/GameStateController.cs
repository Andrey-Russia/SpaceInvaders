using System;
using UnityEngine;

namespace SpaceInvaders.Gameplay.State
{
    public sealed class GameStateController
    {
        public event Action<GameState> StateChanged;

        public GameState CurrentState { get; private set; }

        public bool IsPlaying => CurrentState == GameState.Playing;

        public void StartGame()
        {
            ChangeState(GameState.Playing);
        }

        public void Pause()
        {
            if (CurrentState == GameState.Playing)
            {
                ChangeState(GameState.Paused);
            }
        }

        public void Resume()
        {
            if (CurrentState == GameState.Paused)
            {
                ChangeState(GameState.Playing);
            }
        }

        public void SetVictory()
        {
            if (CurrentState == GameState.Playing)
            {
                ChangeState(GameState.Victory);
            }
        }

        public void SetDefeat()
        {
            if (CurrentState == GameState.Playing)
            {
                ChangeState(GameState.Defeat);
            }
        }

        private void ChangeState(GameState state)
        {
            if (CurrentState == state)
            {
                return;
            }

            CurrentState = state;

            Time.timeScale = state == GameState.Playing ? 1f : 0f;

            StateChanged?.Invoke(state);
        }
    }
}