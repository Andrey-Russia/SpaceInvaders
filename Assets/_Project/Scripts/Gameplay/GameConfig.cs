using UnityEngine;

namespace SpaceInvaders.Gameplay.Configuration
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Space Invaders/Game Config")]
    public sealed class GameConfig : ScriptableObject
    {
        public const int InitialPlayerHealth = 3;

        [Header("Player")]
        [SerializeField, Min(0.1f)]
        private float _playerMoveSpeed = 8f;

        [SerializeField, Min(0.01f)]
        private float _playerFireCooldown = 0.25f;

        [SerializeField, Min(0f)]
        private float _playerScreenPadding = 0.25f;

        [Header("Player projectile")]
        [SerializeField, Min(0.1f)]
        private float _playerProjectileSpeed = 12f;

        [Header("Enemy")]
        [SerializeField, Min(1)]
        private int _enemyHealth = 1;

        [SerializeField, Min(0)]
        private int _enemyScoreReward = 100;

        [Header("Enemy formation")]
        [SerializeField, Min(1)]
        private int _formationRows = 4;

        [SerializeField, Min(1)]
        private int _formationColumns = 8;

        [SerializeField]
        private Vector2 _formationSpacing = new(1.3f, 0.9f);

        [SerializeField]
        private Vector2 _formationStartPosition = new(-4.5f, 3.5f);

        [SerializeField, Min(0f)]
        private float _formationScreenPadding = 0.4f;

        [SerializeField, Min(0.1f)]
        private float _formationDescentInterval = 3f;

        [SerializeField, Min(0.01f)]
        private float _formationDescentStep = 0.35f;

        [SerializeField]
        private float _defeatLineY = -3.5f;

        [Header("Enemy shooting")]
        [SerializeField, Min(0.1f)]
        private float _enemyShotMinInterval = 1.5f;

        [SerializeField, Min(0.1f)]
        private float _enemyShotMaxInterval = 3.5f;

        [SerializeField, Min(0.1f)]
        private float _enemyProjectileSpeed = 7f;

        public float PlayerMoveSpeed => _playerMoveSpeed;
        public float PlayerFireCooldown => _playerFireCooldown;
        public float PlayerScreenPadding => _playerScreenPadding;

        public float PlayerProjectileSpeed => _playerProjectileSpeed;

        public int EnemyHealth => _enemyHealth;
        public int EnemyScoreReward => _enemyScoreReward;

        public int FormationRows => _formationRows;
        public int FormationColumns => _formationColumns;
        public Vector2 FormationSpacing => _formationSpacing;
        public Vector2 FormationStartPosition => _formationStartPosition;
        public float FormationScreenPadding => _formationScreenPadding;
        public float FormationDescentInterval => _formationDescentInterval;
        public float FormationDescentStep => _formationDescentStep;
        public float DefeatLineY => _defeatLineY;

        public float EnemyShotMinInterval => _enemyShotMinInterval;
        public float EnemyShotMaxInterval => _enemyShotMaxInterval;
        public float EnemyProjectileSpeed => _enemyProjectileSpeed;

        private void OnValidate()
        {
            _formationSpacing.x = Mathf.Max(0.1f, _formationSpacing.x);
            _formationSpacing.y = Mathf.Max(0.1f, _formationSpacing.y);
            _formationScreenPadding = Mathf.Max(0f, _formationScreenPadding);

            _enemyShotMaxInterval = Mathf.Max(_enemyShotMinInterval, _enemyShotMaxInterval);
        }
    }
}
