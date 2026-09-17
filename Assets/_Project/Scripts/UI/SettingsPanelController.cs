using SpaceInvaders.Infrastructure.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceInvaders.UI
{
    public sealed class SettingsPanelController : MonoBehaviour
    {
        [Header("Volume")]
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _sfxSlider;

        private void OnEnable()
        {
            AudioManager audioManager = AudioManager.Instance;

            if (audioManager == null)
            {
                Debug.LogError("AudioManager is not initialized.");
                return;
            }

            _musicSlider.SetValueWithoutNotify(audioManager.MusicVolume);
            _sfxSlider.SetValueWithoutNotify(audioManager.SfxVolume);

            _musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            _sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
        }

        private void OnDisable()
        {
            _musicSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
            _sfxSlider.onValueChanged.RemoveListener(OnSfxVolumeChanged);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        private void OnMusicVolumeChanged(float volume)
        {
            AudioManager.Instance.SetMusicVolume(volume);
        }

        private void OnSfxVolumeChanged(float volume)
        {
            AudioManager.Instance.SetSfxVolume(volume);
        }
    }
}