using System;
using SpaceInvaders.Gameplay.Combat;
using SpaceInvaders.Gameplay.Scoring;
using UnityEngine;

namespace SpaceInvaders.Gameplay.Enemies
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class EnemyController : MonoBehaviour, IDamageable
    {
        private ScoreService _scoreService;
        private Action<EnemyController> _deathCallback;

        private int _health;
        private int _scoreReward;
        private bool _isDead;

        public void Initialize(int health, int scoreReward, ScoreService scoreService, Action<EnemyController> deathCallback)
        {
            _health = health;
            _scoreReward = scoreReward;
            _scoreService = scoreService;
            _deathCallback = deathCallback;
            _isDead = false;
        }

        public void TakeDamage(int damage)
        {
            if (_isDead || damage <= 0)
            {
                return;
            }

            _health -= damage;

            if (_health <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            _isDead = true;

            _scoreService.AddScore(_scoreReward);
            _deathCallback?.Invoke(this);

            Destroy(gameObject);
        }
    }
}