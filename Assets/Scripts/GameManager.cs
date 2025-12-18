using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
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


}
