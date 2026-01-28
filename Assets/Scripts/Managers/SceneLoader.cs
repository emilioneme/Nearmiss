using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Collections;
using DG.Tweening;

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


        [SerializeField] GameObject LoadingCanvas;
        [SerializeField] Image LoadingBar;

        [SerializeField] RectTransform rt;

        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneRoutine(sceneName));
        }

        IEnumerator LoadSceneRoutine(string sceneName)
        {
            var scene = SceneManager.LoadSceneAsync(sceneName);

            float canvasWidth = rt.rect.width;

            LoadingCanvas.SetActive(true);
            scene.allowSceneActivation = false;

            rt.anchoredPosition = new Vector2(canvasWidth, 0);
            rt.DOAnchorPos(Vector2.zero, .25f).SetEase(Ease.OutCubic);

            while (scene.progress < 0.9f)
            {
                LoadingBar.fillAmount = Mathf.Clamp01(scene.progress + .1f);
                yield return null;
            }

            rt.anchoredPosition = Vector2.zero;
            rt.DOAnchorPos(new Vector2(-canvasWidth, 0), .25f).SetEase(Ease.OutCubic)
               .OnComplete(() => 
               {
                   scene.allowSceneActivation = true;
                   LoadingCanvas.SetActive(true);
               });
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