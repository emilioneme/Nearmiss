using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class UserData : MonoBehaviour
{
    #region Singleton
    public static UserData Instance;
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
    public float personalHighScore = 0;
    [HideInInspector] public string personalHighScoreKey = "HighScore";
    public float currentScore = 0;

    public DroneData startDroneData;

    [Header("Sense")]
    public float lookSensitivity = 1;
    [HideInInspector] public string lookSensitivityKey = "LookSensitivity";

    [Header("Volume")]
    [Range(0f, 1f)]
    public float masterVolume = .5f;
    [HideInInspector] public string masterVolumeKey = "MasterVolume";

    [Range(0f, 1f)]
    public float musicVolume = .5f;
    [HideInInspector] public string musicVolumeKey = "MusicVolume";

    [Header("Respawn")]
    public bool automaticRespawn = false;
    [HideInInspector] public string automaticRespawnKey = "AutomaticRespawn";
    public bool freezeBeforeSpawn;
    [HideInInspector] public string freezeBeforeSpawnKey = "FreezeBeforeSpawn";

    public bool isDead = false;


    public UnityEvent<float> MusicVolumeChange;

    #region  Multipliers
    [Header("Point Calculation")]
    [SerializeField]
    [Range(0f, 1f)]
    static public float maxDistancePoints = 10;
    [SerializeField]
    [Range(0f, .1f)]
    static public float speedPointsMultiplier = .5f;
    [SerializeField]
    static public float maxComboMultiplier = 10;
    #endregion

    private void Start()
    {
        LoadData();
    }

    void LoadData()
    {
        ////personalHigh
        if (PlayerPrefs.HasKey(personalHighScoreKey))
        {
            personalHighScore = PlayerPrefs.GetFloat(personalHighScoreKey);
        }
        else
        {
            PlayerPrefs.SetFloat(personalHighScoreKey, personalHighScore);
        }

        ////look sensitivity
        if (PlayerPrefs.HasKey(lookSensitivityKey))
        {
            lookSensitivity = PlayerPrefs.GetFloat(lookSensitivityKey);
        }
        else
        {
            PlayerPrefs.SetFloat(lookSensitivityKey, lookSensitivity);
        }

        ////master
        if (PlayerPrefs.HasKey(masterVolumeKey))
        {
            masterVolume = PlayerPrefs.GetFloat(masterVolumeKey);
        }
        else
        {
            PlayerPrefs.SetFloat(masterVolumeKey, masterVolume);
        }

        ////music  
        if (PlayerPrefs.HasKey(musicVolumeKey))
        {
            musicVolume = PlayerPrefs.GetFloat(musicVolumeKey);
        }
        else
        {
            PlayerPrefs.SetFloat(musicVolumeKey, musicVolume);
        }

        ////respawn
        if (PlayerPrefs.HasKey(automaticRespawnKey))
        {
            automaticRespawn = PlayerPrefs.GetInt(automaticRespawnKey) == 1;
        }
        else
        {
            PlayerPrefs.SetInt(automaticRespawnKey, automaticRespawn ? 1 : 0);
        }

        //freze spawn
        if (PlayerPrefs.HasKey(freezeBeforeSpawnKey))
        {
            freezeBeforeSpawn = PlayerPrefs.GetInt(freezeBeforeSpawnKey) == 1;
        }
        else
        {
            PlayerPrefs.SetInt(freezeBeforeSpawnKey, freezeBeforeSpawn ? 1 : 0);
        }

        AudioListener.volume = masterVolume;
        PlayerPrefs.Save();
    }

    public void SaveData()
    {
        PlayerPrefs.SetFloat("PersonalHighScore", personalHighScore);
        PlayerPrefs.SetFloat("LookSensitivity", lookSensitivity);
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);

        PlayerPrefs.SetInt("AutomaticRespawn", automaticRespawn ? 1 : 0);
        PlayerPrefs.SetInt("FreezeBeforeSpawn", freezeBeforeSpawn ? 1 : 0);

        PlayerPrefs.Save();
    }

    public void ChangeMusicVolume(float vol) 
    {
        musicVolume = vol;
        MusicVolumeChange.Invoke(musicVolume);
        SaveData();
    }
    public void ChangeMasterVolume(float vol)
    {
        masterVolume = vol;
        AudioListener.volume = vol;
        SaveData();
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
        currentScore = 0;
        if(PauseManager.Instance) PauseManager.Instance.canPause = true;
        if (PauseManager.Instance) PauseManager.Instance.isPaused = false;
        isDead = false;
        AudioListener.volume = masterVolume;
    }
}
