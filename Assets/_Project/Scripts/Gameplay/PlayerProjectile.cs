using System;
using UnityEngine;

namespace SpaceInvaders.Gameplay.Projectiles
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public sealed class PlayerProjectile : MonoBehaviour
    {
        private Action<PlayerProjectile> _releaseAction;
        private Camera _camera;
        private float _speed;
        private bool _isLaunched;

        public void Initialize(Action<PlayerProjectile> releaseAction)
        {
            _releaseAction = releaseAction;
            _camera = Camera.main;

            gameObject.SetActive(false);
        }

        public void Launch(Vector3 position, float speed)
        {
            transform.position = position;
            _speed = speed;
            _isLaunched = true;

            gameObject.SetActive(true);
        }

        private void Update()
        {
            if (!_isLaunched)
            {
                return;
            }

            transform.position += Vector3.up * (_speed * Time.deltaTime);

            Vector3 viewportPosition = _camera.WorldToViewportPoint(transform.position);

            if (viewportPosition.y > 1.05f)
            {
                Release();
            }
        }

        public void Release()
        {
            if (!_isLaunched)
            {
                return;
            }

            _isLaunched = false;
            _releaseAction?.Invoke(this);
        }
    }
}