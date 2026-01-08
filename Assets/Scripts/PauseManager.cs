using eneme;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Events;

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

    [Header("Event")]
    public UnityEvent Paused;
    public UnityEvent UnPaused;


    private void Start()
    {
        if(UserData.Instance.isPaused)
            Pause();
        else
            UnPause();
    }

    private void Update()
    {
        if (playerInput.pauseAction.WasReleasedThisFrame() && UserData.Instance.canPause) 
            TogglePause();
    }   

    #region Pausing
    public void TogglePause() 
    {
        if (UserData.Instance.isPaused)
            UnPause();
        else
            Pause();
    }
    public void UnPause()
    {
        UserData.Instance.isPaused = false;
        pauseCanvas.SetActive(false);
        UnPaused.Invoke();

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
        UserData.Instance.isPaused = true;
        pauseCanvas.SetActive(true);
        Paused.Invoke();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = .1f;
    }
    #endregion
    public void GoToScene(string sceneName)
    {
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

}
