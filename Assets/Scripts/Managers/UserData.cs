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

    [Header("User")]
    public string UserName = "Anonymus";
    int UserID;
    [SerializeField]
    public float personalHighScore = 0;

    public DroneData startDroneData;

    [Header("Sense")]
    public float lookSensitivity = 1;

    [Header("Volume")]
    [Range(0f, 1f)]
    public float masterVolume = .5f;

    [Range(0f, 1f)]
    public float musicVolume = .5f;

    [Header("Respawn")]
    public bool automaticRespawn = false;
    public bool freezeBeforeSpawn;

    public bool canPause = false;
    public bool isPaused = false;
    public bool isDead = false;

    [Header("In Game Stats")]
    [SerializeReference]
    public Vector3 droneVelocity = Vector3.zero;
    [SerializeReference]
    public float deltaVelocity = 0;
    [SerializeReference]
    public float avgVelocity = 0;
    [SerializeField] 
    public float averageAdaptSpeed = 1;

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

    public void ChangeMusicVolume(float vol) 
    {
        musicVolume = vol;
        MusicVolumeChange.Invoke(musicVolume);
    }
    public void ChangeMasterVolume(float vol)
    {
        masterVolume = vol;
        AudioListener.volume = vol;
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
        canPause = true;
        isPaused = false;
        isDead = false;
    }
}
