using SpaceInvaders.Infrastructure.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceInvaders.UI
{
    [RequireComponent(typeof(Button))]
    public sealed class UIButtonAudio : MonoBehaviour
    {
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(PlayClick);
        }

        private void OnDestroy()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(PlayClick);
            }
        }

        private static void PlayClick()
        {
            AudioManager.Instance?.PlayUiClick();
        }
    }
}
