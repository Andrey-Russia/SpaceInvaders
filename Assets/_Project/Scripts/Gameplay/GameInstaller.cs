using SpaceInvaders.Gameplay.Configuration;
using SpaceInvaders.Gameplay.Input;
using SpaceInvaders.Gameplay.Player;
using UnityEngine;

namespace SpaceInvaders.Gameplay
{
    public sealed class GameInstaller : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private GameConfig _config;

        [Header("Scene objects")]
        [SerializeField] private PlayerController _player;

        [SerializeField] private MobilePlayerInput _mobileInput;

        private void Awake()
        {
            if (_config == null)
            {
                Debug.LogError("GameConfig is not assigned.", this);
                enabled = false;
                return;
            }

            if (_player == null)
            {
                Debug.LogError("Player is not assigned.", this);
                enabled = false;
                return;
            }

            if (_mobileInput == null)
            {
                Debug.LogError("MobilePlayerInput is not assigned.", this);
                enabled = false;
                return;
            }

            IPlayerInput input = new CompositePlayerInput(new KeyboardPlayerInput(), _mobileInput);
            _player.Initialize(_config, input);
        }
    }
}