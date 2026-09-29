using System;
using SpaceInvaders.Gameplay.Configuration;
using SpaceInvaders.Gameplay.Projectiles;
using UnityEngine;

namespace SpaceInvaders.Gameplay.Enemies
{
    public sealed class EnemyShooterController : MonoBehaviour
    {
        public event Action ShotFired;

        private GameConfig _config;
        private EnemyFormationController _formation;
        private EnemyProjectilePool _projectilePool;

        private float _shotTimer;
        private bool _isInitialized;

        public void Initialize(GameConfig config, EnemyFormationController formation,EnemyProjectilePool projectilePool)
        {
            _config = config;
            _formation = formation;
            _projectilePool = projectilePool;
            _isInitialized = true;

            ScheduleNextShot();
        }

        private void Update()
        {
            if (!_isInitialized ||
                _formation.Enemies.Count == 0)
            {
                return;
            }

            _shotTimer -= Time.deltaTime;

            if (_shotTimer <= 0f)
            {
                Shoot();
                ScheduleNextShot();
            }
        }

        private void Shoot()
        {
            int randomIndex = UnityEngine.Random.Range(0, _formation.Enemies.Count);

            EnemyController enemy = _formation.Enemies[randomIndex];

            if (enemy == null)
            {
                return;
            }

            _projectilePool.Spawn(enemy.FirePosition, _config.EnemyProjectileSpeed);
            ShotFired?.Invoke();
        }

        private void ScheduleNextShot()
        {
            _shotTimer = UnityEngine.Random.Range(_config.EnemyShotMinInterval, _config.EnemyShotMaxInterval);
        }
    }
}
