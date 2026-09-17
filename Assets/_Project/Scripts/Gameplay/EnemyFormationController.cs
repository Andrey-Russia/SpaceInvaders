using System;
using System.Collections.Generic;
using SpaceInvaders.Gameplay.Configuration;
using SpaceInvaders.Gameplay.Scoring;
using UnityEngine;

namespace SpaceInvaders.Gameplay.Enemies
{
    public sealed class EnemyFormationController : MonoBehaviour
    {
        [SerializeField] private EnemyController _enemyPrefab;

        private readonly List<EnemyController> _enemies = new();

        private GameConfig _config;
        private float _descentTimer;
        private bool _isActive;

        public event Action AllEnemiesDestroyed;
        public event Action DefeatLineReached;

        public IReadOnlyList<EnemyController> Enemies => _enemies;

        public void Initialize(GameConfig config, ScoreService scoreService)
        {
            _config = config;
            _descentTimer = config.FormationDescentInterval;
            _isActive = true;

            transform.position = config.FormationStartPosition;

            CreateFormation(scoreService);
        }

        private void Update()
        {
            if (!_isActive)
            {
                return;
            }

            _descentTimer -= Time.deltaTime;

            if (_descentTimer <= 0f)
            {
                Descend();
                _descentTimer = _config.FormationDescentInterval;
            }
        }

        private void CreateFormation(ScoreService scoreService)
        {
            for (int row = 0; row < _config.FormationRows; row++)
            {
                for (int column = 0; column < _config.FormationColumns; column++)
                {
                    Vector3 localPosition = new(column * _config.FormationSpacing.x, -row * _config.FormationSpacing.y, 0f);

                    EnemyController enemy = Instantiate(_enemyPrefab, transform);

                    enemy.transform.localPosition = localPosition;

                    enemy.Initialize(_config.EnemyHealth, _config.EnemyScoreReward, scoreService, OnEnemyDied);

                    _enemies.Add(enemy);
                }
            }
        }

        private void Descend()
        {
            transform.position += Vector3.down * _config.FormationDescentStep;

            if (HasReachedDefeatLine())
            {
                _isActive = false;
                DefeatLineReached?.Invoke();
            }
        }

        private bool HasReachedDefeatLine()
        {
            foreach (EnemyController enemy in _enemies)
            {
                if (enemy != null && enemy.transform.position.y <= _config.DefeatLineY)
                {
                    return true;
                }
            }

            return false;
        }

        private void OnEnemyDied(EnemyController enemy)
        {
            _enemies.Remove(enemy);

            if (_enemies.Count > 0)
            {
                return;
            }

            _isActive = false;
            AllEnemiesDestroyed?.Invoke();
        }
    }
}