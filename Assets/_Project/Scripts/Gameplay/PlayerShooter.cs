using System;
using SpaceInvaders.Gameplay.Configuration;
using SpaceInvaders.Gameplay.Input;
using SpaceInvaders.Gameplay.Projectiles;
using UnityEngine;

namespace SpaceInvaders.Gameplay.Player
{
    public sealed class PlayerShooter : MonoBehaviour
    {
        [SerializeField] private Transform _firePoint;

        public event Action ShotFired;

        private GameConfig _config;
        private IPlayerInput _input;
        private PlayerProjectilePool _projectilePool;
        private float _cooldown;
        private bool _isInitialized;

        private void Update()
        {
            if (!_isInitialized)
            {
                return;
            }

            _cooldown -= Time.deltaTime;

            if (_input.FireHeld && _cooldown <= 0f)
            {
                Shoot();
            }
        }

        public void Initialize(GameConfig config, IPlayerInput input, PlayerProjectilePool projectilePool)
        {
            _config = config;
            _input = input;
            _projectilePool = projectilePool;
            _isInitialized = true;
        }

        private void Shoot()
        {
            _projectilePool.Spawn(_firePoint.position, _config.PlayerProjectileSpeed);
            ShotFired?.Invoke();

            _cooldown = _config.PlayerFireCooldown;
        }
    }
}
