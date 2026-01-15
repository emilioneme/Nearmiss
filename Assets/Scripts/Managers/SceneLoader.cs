using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Collections;

namespace eneme
{
    public class SceneLoader : MonoBehaviour
    {
        #region Singleton
        public static SceneLoader Instance;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion

        [SerializeField]
        GameObject LoadingCanvas;
        [SerializeField]
        Image LoadingBar;

        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneRoutine(sceneName));
        }

        IEnumerator LoadSceneRoutine(string sceneName)
        {
            var scene = SceneManager.LoadSceneAsync(sceneName);

            scene.allowSceneActivation = false;
            LoadingCanvas.SetActive(true);

            while (scene.progress < 0.9f)
            {
                LoadingBar.fillAmount = Mathf.Clamp01(scene.progress + .1f);
                yield return null; // <- the key
            }

            scene.allowSceneActivation = true;
        }

        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            LoadingCanvas.SetActive(false);
            LoadingBar.fillAmount = .1f;
        }
    }
}