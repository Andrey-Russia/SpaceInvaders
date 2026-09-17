using UnityEngine;

namespace SpaceInvaders.Infrastructure.Audio
{
    public sealed class AudioManager : MonoBehaviour
    {
        private const string MusicVolumeKey = "MusicVolume";
        private const string SfxVolumeKey = "SfxVolume";

        public static AudioManager Instance { get; private set; }

        [Header("Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        public float MusicVolume => musicSource.volume;
        public float SfxVolume => sfxSource.volume;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadSettings();
        }

        public void PlayMusic(AudioClip clip)
        {
            if (clip == null || musicSource.clip == clip)
            {
                return;
            }

            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }

        public void StopMusic()
        {
            musicSource.Stop();
        }

        public void PlaySfx(AudioClip clip)
        {
            if (clip != null)
            {
                sfxSource.PlayOneShot(clip);
            }
        }

        public void SetMusicVolume(float volume)
        {
            musicSource.volume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat(MusicVolumeKey, musicSource.volume);
            PlayerPrefs.Save();
        }

        public void SetSfxVolume(float volume)
        {
            sfxSource.volume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat(SfxVolumeKey, sfxSource.volume);
            PlayerPrefs.Save();
        }

        private void LoadSettings()
        {
            musicSource.volume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
            sfxSource.volume = PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
        }
    }
}