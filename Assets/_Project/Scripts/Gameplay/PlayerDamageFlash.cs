using System.Collections;
using UnityEngine;

namespace SpaceInvaders.Gameplay.Player
{
    public sealed class PlayerDamageFlash : MonoBehaviour
    {
        [SerializeField] private float _flashDuration = 0.12f;

        private PlayerHealth _health;
        private SpriteRenderer _spriteRenderer;
        private Color _normalColor;
        private Coroutine _flashCoroutine;

        private void Awake()
        {
            _health = GetComponent<PlayerHealth>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _normalColor = _spriteRenderer.color;
        }

        private void OnEnable()
        {
            _health.Damaged += Flash;
        }

        private void OnDisable()
        {
            _health.Damaged -= Flash;

            if (_flashCoroutine != null)
            {
                StopCoroutine(_flashCoroutine);
            }

            _spriteRenderer.color = _normalColor;
        }

        private void Flash()
        {
            if (_flashCoroutine != null)
            {
                StopCoroutine(_flashCoroutine);
            }

            _flashCoroutine = StartCoroutine(FlashRoutine());
        }

        private IEnumerator FlashRoutine()
        {
            _spriteRenderer.color = Color.red;
            yield return new WaitForSecondsRealtime(_flashDuration);
            _spriteRenderer.color = _normalColor;
            _flashCoroutine = null;
        }
    }
}