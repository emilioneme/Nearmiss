using eneme;
using Unity.Cinemachine;
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
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    public GameObject playerPrefab;
    GameObject player;
    PlayerManager playerManager;

    //[Header("Cameras")]
    //[SerializeField] int globalCameraPriority = -10;
    //[SerializeField] CinemachineCamera globalCam;

    private void Start()
    {
        InitiatePlayer();
    }

    void InitiatePlayer() 
    {
        player = Instantiate(playerPrefab);
        playerManager = player.GetComponent<PlayerManager>();
        playerManager.SetStartData(UserData.Instance.startDroneData);
        //globalCam.Priority = globalCameraPriority;
    }

}
