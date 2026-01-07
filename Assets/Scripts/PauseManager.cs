using eneme;
using UnityEngine;

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

    public bool isPaused = false;

    [Header("Input")]
    [SerializeField]
    PlayerInput playerInput;

    [Header("Canvas")]
    [SerializeField]
    GameObject pauseCanvas;
    [SerializeField]
    GameObject playerUICanvas;

    [Header("GlobalCamera")]
    [SerializeField]
    Camera GlobalCamera;

    [Header("Menu")]
    [SerializeField]
    string MenuSceneName = "Menu Scene";

    private void Start()
    {
        if(isPaused)
            Pause();
        else
            UnPause();

        SettingsManager.Instance.SettingsClosed.AddListener(CloseSettings);
    }

    private void Update()
    {
        if (playerInput.pauseAction.WasReleasedThisFrame()) 
        {
            TogglePause();
        }
    }   

    #region Pausing
    public void TogglePause() 
    {
        if (isPaused)
            UnPause();
        else
            Pause();
    }
    public void UnPause()
    {
        isPaused = false;
        GlobalCamera.enabled = false;
        pauseCanvas.SetActive(false);

        playerUICanvas.SetActive(true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        SettingsManager.Instance.CloseSettings();

        Time.timeScale = 1;

    }
    public void Pause()
    {
        isPaused = true;
        GlobalCamera.enabled = true;
        pauseCanvas.SetActive(true);

        playerUICanvas.SetActive(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = .1f;
    }
    #endregion

    #region Settings
    public void OpenSettings()
    {
        SettingsManager.Instance.settingsCanvas.SetActive(true);
    }

    public void CloseSettings()
    {
        SettingsManager.Instance.settingsCanvas.SetActive(false);
    }
    #endregion

    public void GoToMenu() 
    {
        SceneLoader.Instance.LoadScene(MenuSceneName);
    }
}
