using SpaceInvaders.Gameplay.Configuration;
using SpaceInvaders.Gameplay.Enemies;
using SpaceInvaders.Gameplay.Flow;
using SpaceInvaders.Gameplay.Input;
using SpaceInvaders.Gameplay.Player;
using SpaceInvaders.Gameplay.Projectiles;
using SpaceInvaders.Gameplay.Scoring;
using SpaceInvaders.Gameplay.State;
using UnityEngine;

namespace SpaceInvaders.Gameplay.Bootstrap
{
    public sealed class GameInstaller : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private GameConfig _config;

        [Header("Player")]
        [SerializeField] private PlayerController _player;
        [SerializeField] private PlayerShooter _playerShooter;
        [SerializeField] private MobilePlayerInput _mobileInput;

        [Header("Projectiles")]
        [SerializeField]
        private PlayerProjectilePool _playerProjectilePool;

        [Header("Enemies")]
        [SerializeField]
        private EnemyFormationController _enemyFormation;

        [Header("Game flow")]
        [SerializeField]
        private GameFlowController _gameFlow;

        private ScoreService _scoreService;
        private GameStateController _gameStateController;

        public ScoreService ScoreService => _scoreService;

        public GameStateController GameStateController => _gameStateController;

        private void Awake()
        {
            if (!ValidateReferences())
            {
                enabled = false;
                return;
            }

            IPlayerInput input = new CompositePlayerInput(new KeyboardPlayerInput(), _mobileInput);

            _scoreService = new ScoreService();
            _gameStateController = new GameStateController();

            _enemyFormation.AllEnemiesDestroyed += _gameStateController.SetVictory;

            _enemyFormation.DefeatLineReached += _gameStateController.SetDefeat;

            _player.Initialize(_config, input);

            _playerShooter.Initialize(_config, input, _playerProjectilePool);

            _gameFlow.Initialize(input);

            _enemyFormation.Initialize(_config, _scoreService);

            _gameStateController.StartGame();
        }

        private void OnDestroy()
        {
            if (_enemyFormation == null ||
                _gameStateController == null)
            {
                return;
            }

            _enemyFormation.AllEnemiesDestroyed -= _gameStateController.SetVictory;

            _enemyFormation.DefeatLineReached -= _gameStateController.SetDefeat;
        }

        private bool ValidateReferences()
        {
            if (_config == null)
            {
                Debug.LogError("GameConfig is not assigned.", this);
                return false;
            }

            if (_player == null || _playerShooter == null)
            {
                Debug.LogError("Player is not assigned.", this);
                return false;
            }

            if (_mobileInput == null)
            {
                Debug.LogError("Mobile input is not assigned.", this);
                return false;
            }

            if (_playerProjectilePool == null)
            {
                Debug.LogError("Player projectile pool is not assigned.", this);
                return false;
            }

            if (_enemyFormation == null)
            {
                Debug.LogError("Enemy formation is not assigned.", this);

                return false;
            }

            if (_gameFlow == null)
            {
                Debug.LogError("Game flow is not assigned.", this);

                return false;
            }

            return true;
        }
    }
}