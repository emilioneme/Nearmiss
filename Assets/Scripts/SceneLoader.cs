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

        public async void LoadScene(string sceneName)
        {
            var scene = SceneManager.LoadSceneAsync(sceneName);

            scene.allowSceneActivation = false;
            LoadingCanvas.SetActive(true);

            do 
            {
                await Task.Delay(1000);
                LoadingBar.fillAmount = scene.progress;
            }while (scene.progress < 0.9f);

            scene.allowSceneActivation = true;
            StartCoroutine(deactivateCanvas(.1f));
        }

        IEnumerator deactivateCanvas(float time) 
        {
            yield return new WaitForSeconds(time);
            SettingsManager.Instance.CloseSettings();
            LoadingCanvas.SetActive(false);
        }

    }
}