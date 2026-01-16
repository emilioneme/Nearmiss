using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using Unity.Cinemachine;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(DroneMovement))]
[RequireComponent(typeof(PlaneLook))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NearmissHandler))]
[RequireComponent(typeof(CollisionHandler))]
[RequireComponent(typeof(PointManager))]
public class PlayerManager : MonoBehaviour
{
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
    public PlayerInput playerInput;
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
        playerInput = GetComponent<PlayerInput>();
        droneMovement = GetComponent<DroneMovement>();
        nearmissHandler = GetComponent<NearmissHandler>();
        collisionHandler = GetComponent<CollisionHandler>();
        pointManager = GetComponent<PointManager>();
        planeLook = GetComponent<PlaneLook>();
        cc = GetComponent<CharacterController>();

        playerModelHandler = GetComponentInChildren<PlayerModelHandler>();
    }

    private void Start()
    {
        playerModelHandler.InitiatePlayerModel();
        DisablePlayer();
        SpawnPlayer(freezeDuration);
    }

    private void Update()
    {
        if(planeLook.enabled)
            HandleLookInput();
        if(droneMovement.enabled)
            HandleMovementInput();
    }

    #region Input
    void HandleLookInput() 
    {
        if (playerInput.LookInput.y != 0)
            planeLook.LookUpDown(playerInput.LookInput.y);
        if (playerInput.LookInput.x != 0)
            planeLook.LookLeftRight(playerInput.LookInput.x);

        //Rotation
        if (playerInput.rotateLeftAction.IsPressed())
            planeLook.RotateLeft();
        if (playerInput.rotateRightAction.IsPressed())
            planeLook.RotateRight();
    }

    void HandleMovementInput() 
    {
        Vector2 dashAction = playerInput.DashInput;
        if (dashAction.sqrMagnitude > 0.01f)
        {
            Vector3 dir = new Vector3(dashAction.x, dashAction.y, 0).normalized;
            Vector3 animAxis = Vector3.Cross(Vector3.forward, dir).normalized;
            droneMovement.Dash(dir, animAxis);
        }

        //*
        if (playerInput.dashBackwardAction.IsPressed())
            droneMovement.Dash(Vector3.back, Vector3.left);
        if (playerInput.dashForwardAction.IsPressed())
            droneMovement.Dash(Vector3.forward, Vector3.right);
        //*/
        
    }
    #endregion

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
        PlayerSpawnInitiated.Invoke(freezeDuration);
        TeleportPlayer(GetRandomSpawn());
        planeLook.enabled = true;
        playerInput.enabled = true;
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
        playerInput.enabled = false;

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
        playerInput.enabled = true;

        playerModelHandler.enabled = true;
        PlayerModel.SetActive(true);

        UserData.Instance.isDead = false;
    }

}
