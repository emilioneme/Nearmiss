using eneme;
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

    private void Start()
    {
        if(UserData.Instance.isPaused)
            Pause();
        else
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
        UserData.Instance.isPaused = true;
        pauseCanvas.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
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
