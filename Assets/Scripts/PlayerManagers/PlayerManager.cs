using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using Unity.Cinemachine;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(DroneMovement))]
[RequireComponent(typeof(PlaneLook))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NearmissHandler))]
[RequireComponent(typeof(CollisionHandler))]
[RequireComponent(typeof(PointManager))]
public class PlayerManager : MonoBehaviour
{
    [SerializeField] DroneData droneData;

    [Header("Spawn")]
    [SerializeField]
    float freezeDuration;

    [Header("Cameras")]
    [SerializeField]
    Camera PlayerCamera;
    [SerializeField]
    CinemachineBrain Brain;

    [SerializeField]
    float crashScreenDurstion = 2f;

    [Header("Events")]
    [SerializeField]
    UnityEvent PlayerDied; //freeze duration

    [SerializeField]
    UnityEvent CrashScreen;
    [SerializeField]
    UnityEvent CrashCamera;

    [SerializeField]
    UnityEvent<float> PlayerSpawnInitiated; //freeze duration
    [SerializeField]
    UnityEvent PlayerSpawned; //freeze duration

    [Header("GameObjects")]
    [SerializeField]
    GameObject PlayerModel;

    #region Managers
    [HideInInspector]
    public PlayerInputHandler playerInputHandler;
    [HideInInspector]
    public DroneMovement droneMovement;
    [HideInInspector]
    public PlaneLook planeLook;
    [HideInInspector]
    public PlayerUIManager playerUIManager;
    [HideInInspector]
    public NearmissHandler nearmissHandler;
    [HideInInspector]
    public CollisionHandler collisionHandler;
    [HideInInspector]
    public PointManager pointManager;
    [HideInInspector]
    public CharacterController cc;

    [HideInInspector] //playerHandler
    public PlayerModelHandler playerModelHandler;
    #endregion

    private void Awake()
    {
        playerInputHandler = GetComponent<PlayerInputHandler>();
        droneMovement = GetComponent<DroneMovement>();
        nearmissHandler = GetComponent<NearmissHandler>();
        collisionHandler = GetComponent<CollisionHandler>();
        pointManager = GetComponent<PointManager>();
        planeLook = GetComponent<PlaneLook>();
        cc = GetComponent<CharacterController>();

        playerModelHandler = GetComponentInChildren<PlayerModelHandler>();

        DisablePlayer();

        if (droneData != null) 
            SetStartData(droneData);
    }

    public void SetStartData(DroneData droneData) 
    {
        if(droneData.MovementData)
            droneMovement.SetStartData(droneData.MovementData);
        if (droneData.NearmissData)
            nearmissHandler.SetStartData(droneData.NearmissData);
        if (droneData.PointData)
            pointManager.SetStartData(droneData.PointData);
        if (droneData.PlayerModel)
            playerModelHandler.SetPlayerModelVisual(droneData.PlayerModel);

        playerModelHandler.InitiatePlayerModel();
        SpawnPlayer(freezeDuration);
    }

    #region CrashHandler
    public void PlayerCrashed(ControllerColliderHit hit)
    {
        PlayerDeathHandler();
    }
    #endregion

    void PlayerDeathHandler() 
    {
        PlayerDied.Invoke();
        DisablePlayer();
        if (UserData.Instance.automaticRespawn)
            InitiatePlayerSpawning();
        else
            ActivateCrashCamera();
    }

    #region Spawning
    public void InitiatePlayerSpawning() 
    {
        if (UserData.Instance.freezeBeforeSpawn)
            SpawnPlayer(freezeDuration);
        else
            SpawnPlayer(0);
    }

    void SpawnPlayer(float freezeDuration) 
    {
        StartCoroutine(FreezeSpawn(freezeDuration));
    }

    IEnumerator FreezeSpawn(float freezeDuration) 
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        PlayerSpawnInitiated.Invoke(freezeDuration);
        TeleportPlayer(GetRandomSpawn());
        planeLook.enabled = true;
        playerInputHandler.enabled = true;
        PlayerModel.SetActive(true);
        yield return new WaitForSeconds(freezeDuration);
        EnablePlayer();
        PlayerSpawned.Invoke();
    }
    #endregion

    #region Crash Cam and Screen
    public void ActivateCrashCamera()
    {
        CrashCamera.Invoke();
        StartCoroutine(CrashScreenCoroutine());
    }

    IEnumerator CrashScreenCoroutine()
    {
        yield return new WaitForSeconds(crashScreenDurstion);
        CrashScreen.Invoke();
    }
    #endregion

    #region Port
    void TeleportPlayer(Vector3 pos)
    {
        bool ccState = cc.enabled;
        cc.enabled = false;
        transform.position = pos;
        cc.enabled = ccState;
    }

    Vector3 GetRandomSpawn()
    {
        Transform[] spawnPoints = SpawnPointsManager.Instance.spawnPoints;
        int randomSpawnIndex = Random.Range(0, spawnPoints.Length);
        return spawnPoints[randomSpawnIndex].position;
    }
    #endregion

    void DisablePlayer() 
    {
        droneMovement.enabled = false;
        nearmissHandler.enabled = false;
        collisionHandler.enabled = false;
        pointManager.enabled = false;
        planeLook.enabled = false;
        playerInputHandler.enabled = false;

        playerModelHandler.enabled = false;
        PlayerModel.SetActive(false);

        UserData.Instance.isDead = true;
    }
    void EnablePlayer()
    {
        droneMovement.enabled = true;
        nearmissHandler.enabled = true;
        collisionHandler.enabled = true;
        pointManager.enabled = true;
        planeLook.enabled = true;
        playerInputHandler.enabled = true;

        playerModelHandler.enabled = true;
        PlayerModel.SetActive(true);

        UserData.Instance.isDead = false;
    }

}
