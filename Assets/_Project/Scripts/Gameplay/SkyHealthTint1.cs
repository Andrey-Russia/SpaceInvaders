using SpaceInvaders.Gameplay.Player;
using UnityEngine;

namespace SpaceInvaders.Gameplay.Visuals
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class SkyHealthTint : MonoBehaviour
    {
        private static readonly int DangerAmountId =
            Shader.PropertyToID("_DangerAmount");

        [SerializeField] private PlayerHealth _playerHealth;

        private SpriteRenderer _renderer;
        private MaterialPropertyBlock _propertyBlock;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _propertyBlock = new MaterialPropertyBlock();
        }

        private void OnEnable()
        {
            if (_playerHealth == null)
            {
                Debug.LogError("PlayerHealth is not assigned.", this);
                enabled = false;
                return;
            }

            _playerHealth.HealthChanged += UpdateDanger;
            UpdateDanger(_playerHealth.CurrentHealth);
        }

        private void OnDisable()
        {
            if (_playerHealth != null)
            {
                _playerHealth.HealthChanged -= UpdateDanger;
            }
        }

        private void UpdateDanger(int health)
        {
            float danger = _playerHealth.MaxHealth > 0
                ? 1f - Mathf.Clamp01(
                    (float)health / _playerHealth.MaxHealth)
                : 0f;

            _renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetFloat(DangerAmountId, danger);
            _renderer.SetPropertyBlock(_propertyBlock);
        }
    }
}