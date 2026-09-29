using System;
using System.Collections.Generic;
using SpaceInvaders.Gameplay.Configuration;
using SpaceInvaders.Gameplay.Scoring;
using UnityEngine;

namespace SpaceInvaders.Gameplay.Enemies
{
    public sealed class EnemyFormationController : MonoBehaviour
    {
        [SerializeField] private EnemyController[] _enemyPrefabs;

        private readonly List<EnemyController> _enemies = new();

        private GameConfig _config;
        private float _descentTimer;
        private bool _isActive;

        public event Action AllEnemiesDestroyed;
        public event Action DefeatLineReached;
        public event Action EnemyDamaged;
        public event Action EnemyDestroyed;
        public event Action Descended;

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
            float horizontalSpacing = CalculateHorizontalSpacing();
            float formationWidth = (_config.FormationColumns - 1) * horizontalSpacing;
            float firstColumnX = -formationWidth * 0.5f;

            CenterFormationOnCamera();

            for (int row = 0; row < _config.FormationRows; row++)
            {
                for (int column = 0; column < _config.FormationColumns; column++)
                {
                    Vector3 localPosition = new(
                        firstColumnX + column * horizontalSpacing,
                        -row * _config.FormationSpacing.y,
                        0f);

                    int prefabIndex = Mathf.Min(row * _enemyPrefabs.Length / _config.FormationRows, _enemyPrefabs.Length - 1);

                    EnemyController enemy = Instantiate(_enemyPrefabs[prefabIndex], transform);

                    enemy.transform.localPosition = localPosition;

                    enemy.Initialize(_config.EnemyHealth, _config.EnemyScoreReward, scoreService, OnEnemyDied);
                    enemy.Damaged += OnEnemyDamaged;

                    _enemies.Add(enemy);
                }
            }
        }

        private float CalculateHorizontalSpacing()
        {
            if (_config.FormationColumns <= 1)
            {
                return 0f;
            }

            Camera mainCamera = Camera.main;

            if (mainCamera == null || !mainCamera.orthographic)
            {
                return _config.FormationSpacing.x;
            }

            float screenWidth = mainCamera.orthographicSize * 2f * mainCamera.aspect;
            float enemyWidth = GetLargestEnemyWidth();
            float availableWidth = Mathf.Max(
                0f,
                screenWidth - enemyWidth - _config.FormationScreenPadding * 2f);
            float fittedSpacing = availableWidth / (_config.FormationColumns - 1);

            return Mathf.Min(_config.FormationSpacing.x, fittedSpacing);
        }

        private void CenterFormationOnCamera()
        {
            Camera mainCamera = Camera.main;

            if (mainCamera == null)
            {
                return;
            }

            Vector3 position = transform.position;
            position.x = mainCamera.transform.position.x;
            transform.position = position;
        }

        private float GetLargestEnemyWidth()
        {
            float largestWidth = 0f;

            foreach (EnemyController enemyPrefab in _enemyPrefabs)
            {
                if (enemyPrefab == null)
                {
                    continue;
                }

                BoxCollider2D boxCollider = enemyPrefab.GetComponent<BoxCollider2D>();

                if (boxCollider != null)
                {
                    float width = boxCollider.size.x * Mathf.Abs(enemyPrefab.transform.localScale.x);
                    largestWidth = Mathf.Max(largestWidth, width);
                    continue;
                }

                SpriteRenderer spriteRenderer = enemyPrefab.GetComponent<SpriteRenderer>();

                if (spriteRenderer != null && spriteRenderer.sprite != null)
                {
                    float width = spriteRenderer.sprite.bounds.size.x *
                                  Mathf.Abs(enemyPrefab.transform.localScale.x);
                    largestWidth = Mathf.Max(largestWidth, width);
                }
            }

            return largestWidth;
        }

        private void Descend()
        {
            transform.position += Vector3.down * _config.FormationDescentStep;
            Descended?.Invoke();

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
            enemy.Damaged -= OnEnemyDamaged;
            _enemies.Remove(enemy);
            EnemyDestroyed?.Invoke();

            if (_enemies.Count > 0)
            {
                return;
            }

            _isActive = false;
            AllEnemiesDestroyed?.Invoke();
        }

        private void OnEnemyDamaged()
        {
            EnemyDamaged?.Invoke();
        }
    }
}
