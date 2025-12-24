using eneme;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance;
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
    public float highScore = 0;

    [Range(0.1f, 2.0f)]
    public float mouseSensitivity = 1;
    [Range(0.1f, 2.0f)]
    public float stickSensitivity = 180;
    [Range(1f, 4f)]
    public float stickExponent = 2.0f; // curve: higher = faster near edge

    [Range(0f, 1f)]
    public float masterVolume = .5f;

    [Range(0f, 1f)]
    public float musicVolume = .5f;


}
