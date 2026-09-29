using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace SpaceInvaders.UI
{
    public sealed class MainMenuTransitionController : MonoBehaviour
    {
        [Header("Menu")]
        [SerializeField] private CanvasGroup _menuCanvasGroup;
        [SerializeField] private RectTransform _logo;
        [SerializeField] private CanvasGroup _logoCanvasGroup;

        [Header("Buttons")]
        [SerializeField] private RectTransform[] _leftButtons;
        [SerializeField] private RectTransform[] _rightButtons;

        [Header("Ship")]
        [SerializeField] private RectTransform _ship;
        [SerializeField] private GameObject _thruster;

        [Header("Animation")]
        [SerializeField, Min(0.1f)] private float _buttonDuration = 0.45f;
        [SerializeField, Min(0f)] private float _buttonStagger = 0.06f;
        [SerializeField, Min(100f)] private float _buttonExitDistance = 1400f;

        [SerializeField, Min(0.1f)] private float _logoDuration = 0.35f;
        [SerializeField] private float _logoOffset = 120f;

        [SerializeField, Min(0f)] private float _shipAnticipationDistance = 35f;
        [SerializeField, Min(0.1f)] private float _shipAnticipationDuration = 0.18f;
        [SerializeField, Min(100f)] private float _shipFlightDistance = 1600f;
        [SerializeField, Min(0.1f)] private float _shipFlightDuration = 0.85f;

        [Header("Events")]
        [SerializeField] private UnityEvent _transitionCompleted;

        private Sequence _sequence;
        private bool _isPlaying;

        private void Awake()
        {
            if (_thruster != null)
            {
                _thruster.SetActive(false);
            }
        }

        public void PlayTransition()
        {
            if (_isPlaying)
            {
                return;
            }

            _isPlaying = true;

            if (_menuCanvasGroup != null)
            {
                _menuCanvasGroup.interactable = false;
                _menuCanvasGroup.blocksRaycasts = false;
            }

            _sequence = DOTween.Sequence()
                .SetUpdate(true)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);

            AnimateButtons(_leftButtons, -1f);
            AnimateButtons(_rightButtons, 1f);
            AnimateLogo();
            AnimateShip();

            _sequence.OnComplete(() => _transitionCompleted?.Invoke());
        }

        private void AnimateButtons(
            RectTransform[] buttons,
            float direction)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                RectTransform button = buttons[i];

                if (button == null)
                {
                    continue;
                }

                float targetX =
                    button.anchoredPosition.x +
                    direction * _buttonExitDistance;

                _sequence.Insert(
                    i * _buttonStagger,
                    button.DOAnchorPosX(targetX, _buttonDuration)
                        .SetEase(Ease.InBack));

                _sequence.Insert(
                    i * _buttonStagger,
                    button.DOScale(0.85f, _buttonDuration));
            }
        }

        private void AnimateLogo()
        {
            if (_logo != null)
            {
                float targetY =
                    _logo.anchoredPosition.y + _logoOffset;

                _sequence.Insert(
                    0f,
                    _logo.DOAnchorPosY(targetY, _logoDuration)
                        .SetEase(Ease.InQuad));
            }

            if (_logoCanvasGroup != null)
            {
                _sequence.Insert(
                    0f,
                    _logoCanvasGroup.DOFade(0f, _logoDuration));
            }
        }

        private void AnimateShip()
        {
            if (_ship == null)
            {
                return;
            }

            float startY = _ship.anchoredPosition.y;
            float launchTime = _shipAnticipationDuration;

            _sequence.InsertCallback(0f, () =>
            {
                if (_thruster != null)
                {
                    _thruster.SetActive(true);
                }
            });

            _sequence.Insert(
                0f,
                _ship.DOAnchorPosY(
                        startY - _shipAnticipationDistance,
                        _shipAnticipationDuration)
                    .SetEase(Ease.OutQuad));

            _sequence.Insert(
                launchTime,
                _ship.DOAnchorPosY(
                        startY + _shipFlightDistance,
                        _shipFlightDuration)
                    .SetEase(Ease.InCubic));

            _sequence.Insert(
                launchTime,
                _ship.DOScale(1.15f, _shipFlightDuration)
                    .SetEase(Ease.InQuad));
        }

        private void OnDestroy()
        {
            _sequence?.Kill();
        }
    }
}