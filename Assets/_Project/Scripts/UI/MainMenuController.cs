using SpaceInvaders.Core;
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