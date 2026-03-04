using DG.Tweening;
using eneme;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    #region Singleton
    public static PauseManager Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    #endregion

    [Header("Input")]
    [SerializeField]
    PlayerInput playerInput;

    [Header("Canvas")]
    [SerializeField]
    GameObject pauseCanvas;

    [Header("GlobalCamera")]
    [SerializeField]
    Camera GlobalCamera;

    [Header("Scores")]
    [SerializeField]
    TMP_Text highScoreText;
    [SerializeField]
    TMP_Text personalScoreText;

    [SerializeField]
    GameObject highScoreGO;
    [SerializeField]
    GameObject personalScoreGO;

    Vector3 startHighScoreScale;
    Vector3 startScoreScale;


    public UnityEvent OnTogglePause;

    private void Start()
    {
        UnPause();
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasReleasedThisFrame && UserData.Instance.canPause) 
            TogglePause();
    }   

    #region Pausing
    public void TogglePause() 
    {
        if (UserData.Instance.isPaused)
            UnPause();
        else
            Pause();
        OnTogglePause.Invoke();
    }
    public void UnPause()
    {
        UserData.Instance.isPaused = false;
        pauseCanvas.SetActive(false);

        if(!UserData.Instance.isDead) 
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        SettingsManager.Instance.CloseSettings();
        Time.timeScale = 1;
    }

    public void OpenedCrashScreen() 
    {
        UserData.Instance.isPaused = false;
        SettingsManager.Instance.CloseSettings();
        pauseCanvas.SetActive(false);
        Time.timeScale = 1;
    }

    public void Pause()
    {
        UpdateScoresText();
        UserData.Instance.isPaused = true;
        pauseCanvas.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
    }

    void UpdateScoresText() 
    {
        highScoreText.text = Tools.ProcessFloat(UserData.Instance.personalHighScore, 2);
        personalScoreText.text = Tools.ProcessFloat(UserData.Instance.currentScore, 2);
    }

    #endregion
    public void GoToScene(string sceneName)
    {
        UnPause();
        SceneLoader.Instance.LoadScene(sceneName);
    }

    public void OpenSettings()
    {
        SettingsManager.Instance.OpenSettings();
    }

    public void CloseSettings()
    {
        SettingsManager.Instance.CloseSettings();
    }

    public void HighScoreBounce()
    {
        DOBounceTween(ref highScoreGO, startHighScoreScale, .5f, .25f);
    }

    public void PersonalScoreBounce()
    {
        DOBounceTween(ref personalScoreGO, startScoreScale, .5f, .25f);
    }

    public void DOBounceTween(ref GameObject GO, Vector3 fromScale, float toScale, float duration, Ease easeType = Ease.InOutSine)
    {
        GO.transform.DOKill();
        GO.transform.localScale = fromScale;
        GO.transform
            .DOScale(toScale, duration)
            .SetEase(easeType)
            .SetLoops(2, LoopType.Yoyo);
    }

}
