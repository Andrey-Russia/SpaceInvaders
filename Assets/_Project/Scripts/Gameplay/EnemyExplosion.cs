using UnityEngine;

namespace SpaceInvaders.Gameplay.Effects
{
    public sealed class EnemyExplosion : MonoBehaviour
    {
        [SerializeField] private float _duration = 0.3f;

        private SpriteRenderer _spriteRenderer;
        private float _initialAlpha;
        private float _elapsed;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _initialAlpha = _spriteRenderer.color.a;
        }

        private void Update()
        {
            _elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(_elapsed / _duration);
            Color color = _spriteRenderer.color;
            color.a = _initialAlpha * (1f - progress);
            _spriteRenderer.color = color;

            if (_elapsed >= _duration)
            {
                Destroy(gameObject);
            }
        }
    }
}