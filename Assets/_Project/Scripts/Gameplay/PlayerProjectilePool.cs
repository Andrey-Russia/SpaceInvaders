using System.Collections.Generic;
using UnityEngine;

namespace SpaceInvaders.Gameplay.Projectiles
{
    public sealed class PlayerProjectilePool : MonoBehaviour
    {
        [SerializeField] private PlayerProjectile _projectilePrefab;

        [SerializeField, Min(1)]
        private int _initialSize = 12;

        private readonly Queue<PlayerProjectile> _projectiles = new();

        private void Awake()
        {
            for (int i = 0; i < _initialSize; i++)
            {
                PlayerProjectile projectile = CreateProjectile();
                _projectiles.Enqueue(projectile);
            }
        }

        public void Spawn(Vector3 position, float speed)
        {
            PlayerProjectile projectile = _projectiles.Count > 0 ? _projectiles.Dequeue() : CreateProjectile();

            projectile.Launch(position, speed);
        }

        private PlayerProjectile CreateProjectile()
        {
            PlayerProjectile projectile = Instantiate(_projectilePrefab, transform);

            projectile.Initialize(Release);
            return projectile;
        }

        private void Release(PlayerProjectile projectile)
        {
            projectile.gameObject.SetActive(false);
            _projectiles.Enqueue(projectile);
        }
    }
}