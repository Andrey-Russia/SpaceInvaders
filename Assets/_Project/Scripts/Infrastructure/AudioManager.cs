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

        [Header("Music")]
        [SerializeField] private AudioClip menuMusic;
        [SerializeField] private AudioClip gameplayMusic;

        [Header("SFX")]
        [SerializeField] private AudioClip uiClick;
        [SerializeField] private AudioClip playerShot;
        [SerializeField] private AudioClip enemyShot;
        [SerializeField] private AudioClip projectileHit;
        [SerializeField] private AudioClip enemyDestroyed;
        [SerializeField] private AudioClip playerDamaged;
        [SerializeField] private AudioClip enemyStep;
        [SerializeField] private AudioClip victory;
        [SerializeField] private AudioClip defeat;

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
            if (clip == null)
            {
                return;
            }

            if (musicSource.clip == clip)
            {
                if (!musicSource.isPlaying)
                {
                    musicSource.Play();
                }

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

        public void PlayMenuMusic() => PlayMusic(menuMusic);

        public void PlayGameplayMusic() => PlayMusic(gameplayMusic);

        public void PlayUiClick() => PlaySfx(uiClick);

        public void PlayPlayerShot() => PlaySfx(playerShot);

        public void PlayEnemyShot() => PlaySfx(enemyShot);

        public void PlayProjectileHit() => PlaySfx(projectileHit);

        public void PlayEnemyDestroyed() => PlaySfx(enemyDestroyed);

        public void PlayPlayerDamaged() => PlaySfx(playerDamaged);

        public void PlayEnemyStep() => PlaySfx(enemyStep);

        public void PlayVictory() => PlaySfx(victory);

        public void PlayDefeat() => PlaySfx(defeat);

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
