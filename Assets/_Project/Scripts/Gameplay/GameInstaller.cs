using SpaceInvaders.Gameplay.Configuration;
using SpaceInvaders.Gameplay.Enemies;
using SpaceInvaders.Gameplay.Flow;
using SpaceInvaders.Gameplay.Input;
using SpaceInvaders.Gameplay.Player;
using SpaceInvaders.Gameplay.Projectiles;
using SpaceInvaders.Gameplay.Scoring;
using SpaceInvaders.Gameplay.State;
using SpaceInvaders.Infrastructure.Audio;
using SpaceInvaders.UI.Gameplay;
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
        [SerializeField] private PlayerHealth _playerHealth;
        [SerializeField] private MobilePlayerInput _mobileInput;

        [Header("Projectiles")]
        [SerializeField]
        private PlayerProjectilePool _playerProjectilePool;

        [SerializeField]
        private EnemyProjectilePool _enemyProjectilePool;

        [Header("Enemies")]
        [SerializeField]
        private EnemyFormationController _enemyFormation;

        [SerializeField]
        private EnemyShooterController _enemyShooter;

        [Header("Game flow")]
        [SerializeField]
        private GameFlowController _gameFlow;

        [Header("UI")]
        [SerializeField]
        private GameHUDController _gameHud;

        private ScoreService _scoreService;
        private GameStateController _gameStateController;
        private AudioManager _audioManager;

        public ScoreService ScoreService => _scoreService;

        public GameStateController GameStateController =>
            _gameStateController;

        private void Awake()
        {
            if (!ValidateReferences())
            {
                enabled = false;
                return;
            }

            IPlayerInput input = new CompositePlayerInput(
                new KeyboardPlayerInput(),
                _mobileInput);

            _scoreService = new ScoreService();
            _gameStateController = new GameStateController();
            _audioManager = AudioManager.Instance;

            SubscribeToEvents();

            _audioManager?.PlayGameplayMusic();

            _playerHealth.Initialize(
                GameConfig.InitialPlayerHealth);

            _player.Initialize(
                _config,
                input);

            _playerShooter.Initialize(
                _config,
                input,
                _playerProjectilePool);

            _gameFlow.Initialize(
                input,
                _gameStateController);
            _gameHud.Initialize(
                _scoreService,
                _playerHealth,
                _gameStateController,
                _gameFlow);

            _enemyFormation.Initialize(
                _config,
                _scoreService);

            _enemyShooter.Initialize(
                _config,
                _enemyFormation,
                _enemyProjectilePool);

            _gameStateController.StartGame();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            _enemyFormation.AllEnemiesDestroyed +=
                _gameStateController.SetVictory;

            _enemyFormation.DefeatLineReached +=
                _gameStateController.SetDefeat;

            _playerHealth.Died +=
                _gameStateController.SetDefeat;

            _playerShooter.ShotFired += OnPlayerShot;
            _enemyShooter.ShotFired += OnEnemyShot;
            _enemyFormation.EnemyDamaged += OnEnemyDamaged;
            _enemyFormation.EnemyDestroyed += OnEnemyDestroyed;
            _enemyFormation.Descended += OnFormationDescended;
            _playerHealth.Damaged += OnPlayerDamaged;
            _gameStateController.StateChanged += OnGameStateChanged;
        }

        private void UnsubscribeFromEvents()
        {
            if (_enemyFormation != null &&
                _gameStateController != null)
            {
                _enemyFormation.AllEnemiesDestroyed -=
                    _gameStateController.SetVictory;

                _enemyFormation.DefeatLineReached -=
                    _gameStateController.SetDefeat;
            }

            if (_playerHealth != null &&
                _gameStateController != null)
            {
                _playerHealth.Died -=
                    _gameStateController.SetDefeat;
            }

            if (_playerShooter != null)
            {
                _playerShooter.ShotFired -= OnPlayerShot;
            }

            if (_enemyShooter != null)
            {
                _enemyShooter.ShotFired -= OnEnemyShot;
            }

            if (_enemyFormation != null)
            {
                _enemyFormation.EnemyDamaged -= OnEnemyDamaged;
                _enemyFormation.EnemyDestroyed -= OnEnemyDestroyed;
                _enemyFormation.Descended -= OnFormationDescended;
            }

            if (_playerHealth != null)
            {
                _playerHealth.Damaged -= OnPlayerDamaged;
            }

            if (_gameStateController != null)
            {
                _gameStateController.StateChanged -= OnGameStateChanged;
            }
        }

        private void OnPlayerShot() => _audioManager?.PlayPlayerShot();

        private void OnEnemyShot() => _audioManager?.PlayEnemyShot();

        private void OnEnemyDamaged() => _audioManager?.PlayProjectileHit();

        private void OnEnemyDestroyed() => _audioManager?.PlayEnemyDestroyed();

        private void OnFormationDescended() => _audioManager?.PlayEnemyStep();

        private void OnPlayerDamaged() => _audioManager?.PlayPlayerDamaged();

        private void OnGameStateChanged(GameState state)
        {
            if (state == GameState.Victory)
            {
                _audioManager?.PlayVictory();
            }
            else if (state == GameState.Defeat)
            {
                _audioManager?.PlayDefeat();
            }
        }

        private bool ValidateReferences()
        {
            if (_config == null)
            {
                Debug.LogError(
                    "GameConfig is not assigned.",
                    this);

                return false;
            }

            if (_player == null)
            {
                Debug.LogError(
                    "PlayerController is not assigned.",
                    this);

                return false;
            }

            if (_playerShooter == null)
            {
                Debug.LogError(
                    "PlayerShooter is not assigned.",
                    this);

                return false;
            }

            if (_playerHealth == null)
            {
                Debug.LogError(
                    "PlayerHealth is not assigned.",
                    this);

                return false;
            }

            if (_mobileInput == null)
            {
                Debug.LogError(
                    "MobilePlayerInput is not assigned.",
                    this);

                return false;
            }

            if (_playerProjectilePool == null)
            {
                Debug.LogError(
                    "PlayerProjectilePool is not assigned.",
                    this);

                return false;
            }

            if (_enemyProjectilePool == null)
            {
                Debug.LogError(
                    "EnemyProjectilePool is not assigned.",
                    this);

                return false;
            }

            if (_enemyFormation == null)
            {
                Debug.LogError(
                    "EnemyFormationController is not assigned.",
                    this);

                return false;
            }

            if (_enemyShooter == null)
            {
                Debug.LogError(
                    "EnemyShooterController is not assigned.",
                    this);

                return false;
            }

            if (_gameFlow == null)
            {
                Debug.LogError(
                    "GameFlowController is not assigned.",
                    this);

                return false;
            }

            if (_gameHud == null)
            {
                Debug.LogError(
                    "GameHUDController is not assigned.",
                    this);

                return false;
            }

            return true;
        }
    }
}
