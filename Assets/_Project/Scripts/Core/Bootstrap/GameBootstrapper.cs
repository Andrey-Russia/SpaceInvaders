using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceInvaders.Core
{
    public sealed class GameBootstrapper : MonoBehaviour
    {
        public static GameBootstrapper Instance { get; private set; }

        private bool _isLoading;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (SceneManager.GetActiveScene().name == SceneNames.Bootstrap)
            {
                LoadMainMenu();
            }
        }

        public void LoadMainMenu()
        {
            StartCoroutine(LoadScene(SceneNames.MainMenu));
        }

        public void LoadGame()
        {
            StartCoroutine(LoadScene(SceneNames.Game));
        }

        public void RestartGame()
        {
            LoadGame();
        }

        private IEnumerator LoadScene(string sceneName)
        {
            if (_isLoading)
            {
                yield break;
            }

            _isLoading = true;
            Time.timeScale = 1f;

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

            while (!operation.isDone)
            {
                yield return null;
            }

            _isLoading = false;
        }
    }
}