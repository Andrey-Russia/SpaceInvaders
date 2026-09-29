using System.Collections.Generic;
using UnityEngine;

namespace SpaceInvaders.Gameplay.Projectiles
{
    public sealed class EnemyProjectilePool : MonoBehaviour
    {
        [SerializeField] private EnemyProjectile _projectilePrefab;

        [SerializeField, Min(1)]
        private int _initialSize = 12;

        private readonly Queue<EnemyProjectile> _projectiles = new();

        private void Awake()
        {
            for (int i = 0; i < _initialSize; i++)
            {
                EnemyProjectile projectile = CreateProjectile();
                _projectiles.Enqueue(projectile);
            }
        }

        public void Spawn(Vector3 position, float speed)
        {
            EnemyProjectile projectile = _projectiles.Count > 0 ? _projectiles.Dequeue() : CreateProjectile();

            projectile.Launch(position, speed);
        }

        private EnemyProjectile CreateProjectile()
        {
            EnemyProjectile projectile = Instantiate(_projectilePrefab, transform);

            projectile.Initialize(Release);
            return projectile;
        }

        private void Release(EnemyProjectile projectile)
        {
            projectile.gameObject.SetActive(false);
            _projectiles.Enqueue(projectile);
        }
    }
}