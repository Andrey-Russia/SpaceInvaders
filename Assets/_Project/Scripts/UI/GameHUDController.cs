using System.Collections.Generic;
using SpaceInvaders.Gameplay.Flow;
using SpaceInvaders.Gameplay.Player;
using SpaceInvaders.Gameplay.Scoring;
using SpaceInvaders.Gameplay.State;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceInvaders.UI.Gameplay
{
    public sealed class GameHUDController : MonoBehaviour
    {
        [Header("Score")]
        [SerializeField] private TMP_Text _scoreText;

        [Header("Health")]
        [SerializeField] private Transform _healthContainer;
        [SerializeField] private Image _healthIconPrefab;

        [Header("Result")]
        [SerializeField] private GameObject _resultPanel;
        [SerializeField] private TMP_Text _resultTitle;
        [SerializeField] private TMP_Text _finalScoreText;

        [Header("Pause")]
        [SerializeField] private GameObject _pausePanel;

        private readonly List<Image> _healthIcons = new();

        private ScoreService _scoreService;
        private PlayerHealth _playerHealth;
        private GameStateController _gameStateController;
        private GameFlowController _gameFlow;

        public void Initialize(ScoreService scoreService, PlayerHealth playerHealth, GameStateController gameStateController, GameFlowController gameFlow)
        {
            _scoreService = scoreService;
            _playerHealth = playerHealth;
            _gameStateController = gameStateController;
            _gameFlow = gameFlow;

            CreateHealthIcons(playerHealth.MaxHealth);

            UpdateScore(scoreService.CurrentScore);
            UpdateHealth(playerHealth.CurrentHealth);

            _resultPanel.SetActive(false);
            _pausePanel.SetActive(false);

            _scoreService.ScoreChanged += UpdateScore;
            _playerHealth.HealthChanged += UpdateHealth;
            _gameStateController.StateChanged += OnStateChanged;
        }

        private void OnDestroy()
        {
            if (_scoreService != null)
            {
                _scoreService.ScoreChanged -= UpdateScore;
            }

            if (_playerHealth != null)
            {
                _playerHealth.HealthChanged -= UpdateHealth;
            }

            if (_gameStateController != null)
            {
                _gameStateController.StateChanged -= OnStateChanged;
            }
        }

        public void RestartGame()
        {
            _gameFlow.RestartGame();
        }

        public void ReturnToMainMenu()
        {
            _gameFlow.ReturnToMainMenu();
        }

        private void CreateHealthIcons(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                Image icon = Instantiate(_healthIconPrefab, _healthContainer);

                _healthIcons.Add(icon);
            }
        }

        private void UpdateScore(int score)
        {
            _scoreText.text = $"Счёт: {score}";
        }

        private void UpdateHealth(int health)
        {
            for (int i = 0; i < _healthIcons.Count; i++)
            {
                _healthIcons[i].gameObject.SetActive(i < health);
            }
        }

        private void OnStateChanged(GameState state)
        {
            _pausePanel.SetActive(
                state == GameState.Paused);

            if (state == GameState.Playing ||
                state == GameState.Paused)
            {
                _resultPanel.SetActive(false);
                return;
            }

            if (state != GameState.Victory &&
                state != GameState.Defeat)
            {
                return;
            }

            _resultTitle.text = state == GameState.Victory
                ? "Победа!"
                : "Поражение";

            _finalScoreText.text =
                $"Счёт: {_scoreService.CurrentScore}";

            _resultPanel.SetActive(true);
        }

        public void PauseGame()
        {
            _gameFlow.PauseGame();
        }

        public void ResumeGame()
        {
            _gameFlow.ResumeGame();
        }
    }
}