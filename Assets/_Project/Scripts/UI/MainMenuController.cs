using SpaceInvaders.Core;
using SpaceInvaders.Infrastructure.Audio;
using UnityEngine;

namespace SpaceInvaders.UI
{
    public sealed class MainMenuController : MonoBehaviour
    {
        [SerializeField] private SettingsPanelController _settingsPanel;

        private void Awake()
        {
            _settingsPanel.Close();
        }

        private void Start()
        {
            AudioManager.Instance?.PlayMenuMusic();
        }

        public void StartGame()
        {
            GameBootstrapper.Instance.LoadGame();
        }

        public void OpenSettings()
        {
            _settingsPanel.gameObject.SetActive(true);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
