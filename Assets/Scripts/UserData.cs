using UnityEngine;

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

    [Header("Sense")]
    [Range(0.1f, 2.0f)]
    public float mouseSensitivity = 1;
    [Range(0.1f, 2.0f)]
    public float stickSensitivity = 180;
    [Range(1f, 4f)]
    public float stickExponent = 2.0f; // curve: higher = faster near edge

    [Header("Volume")]
    [Range(0f, 1f)]
    public float masterVolume = .5f;

    [Range(0f, 1f)]
    public float musicVolume = .5f;

    [Header("Respawn")]
    public bool automaticRespawn = false;
}
