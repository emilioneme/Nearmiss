using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{

    #region Singleton
    public static SettingsManager Instance;
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

    [Header("Canvas")]
    [SerializeField]
    public GameObject settingsCanvas;

    [Header("Volume")]
    [SerializeField]
    Slider volumeSlider;
    [SerializeField]
    Slider musicSlider;

    [Header("Sense")]
    [SerializeField]
    Slider mouseSensSlider;
    [SerializeField]
    Slider stickSensSlider;

    [Header("Respawn")]
    [SerializeField]
    Toggle automaticRespawn;

    [SerializeField]
    public UnityEvent SettingsClosed;

    

    public void OpenSettings()
    {
        settingsCanvas.SetActive(true);
    }

    public void CloseSettings()
    {
        SettingsClosed.Invoke();
    }

    private void Start()
    {
        UpdateSettings();
    }


    void UpdateSettings() 
    {
        volumeSlider.value = UserData.Instance.masterVolume;
        musicSlider.value = UserData.Instance.musicVolume;
        mouseSensSlider.value = UserData.Instance.mouseSensitivity;
        stickSensSlider.value = UserData.Instance.stickSensitivity;
        automaticRespawn.isOn = UserData.Instance.automaticRespawn;
    }

    #region Set Setting
    public void SetVolume() 
    {
        UserData.Instance.masterVolume = volumeSlider.value;
    }

    public void SetMusic()
    {
        UserData.Instance.musicVolume = musicSlider.value;
    }
    public void SetMouseSens()
    {
        UserData.Instance.mouseSensitivity = mouseSensSlider.value;
    }
    public void SetStickSense()
    {
        UserData.Instance.stickSensitivity = stickSensSlider.value;
    }

    public void SetRespawn() 
    {
        UserData.Instance.automaticRespawn = automaticRespawn.isOn;
    }

    #endregion

    

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
        Debug.Log("Scene loaded: " + scene.name);
        settingsCanvas.SetActive(false);
    }

}
