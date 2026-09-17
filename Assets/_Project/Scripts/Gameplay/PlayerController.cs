using SpaceInvaders.Gameplay.Configuration;
using SpaceInvaders.Gameplay.Input;
using UnityEngine;

namespace SpaceInvaders.Gameplay.Player
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class PlayerController : MonoBehaviour
    {
        private GameConfig _config;
        private IPlayerInput _input;
        private Camera _camera;
        private BoxCollider2D _collider;
        private bool _isInitialized;

        private void Awake()
        {
            _camera = Camera.main;
            _collider = GetComponent<BoxCollider2D>();
        }

        private void Update()
        {
            if (!_isInitialized)
            {
                return;
            }

            Move();
        }

        public void Initialize(GameConfig config, IPlayerInput input)
        {
            _config = config;
            _input = input;
            _isInitialized = true;
        }

        private void Move()
        {
            float direction = _input.Movement;

            Vector3 position = transform.position;
            position.x += direction * _config.PlayerMoveSpeed * Time.deltaTime;

            position.x = ClampToScreen(position.x);
            transform.position = position;
        }

        private float ClampToScreen(float positionX)
        {
            float cameraHalfWidth = _camera.orthographicSize * _camera.aspect;

            float playerHalfWidth = _collider.bounds.extents.x;

            float leftBoundary = _camera.transform.position.x - cameraHalfWidth + playerHalfWidth + _config.PlayerScreenPadding;

            float rightBoundary = _camera.transform.position.x + cameraHalfWidth - playerHalfWidth - _config.PlayerScreenPadding;

            return Mathf.Clamp(positionX, leftBoundary, rightBoundary);
        }
    }
}