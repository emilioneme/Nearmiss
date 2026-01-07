using TMPro;
using UnityEngine;
using UnityEditor.Events;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using Unity.Cinemachine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(DroneMovement))]
[RequireComponent(typeof(PlaneLook))]
[RequireComponent(typeof(NearmissHandler))]
[RequireComponent(typeof(CollisionHandler))]
[RequireComponent(typeof(PointManager))]
public class PlayerManager : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField]
    float freezeDuration;
    [SerializeField]
    Transform[] spawnPoints;

    [Header("Cameras")]
    [SerializeField]
    Camera PlayerCamera;
    [SerializeField]
    CinemachineBrain Brain;

    [Header("Events")]
    [SerializeField]
    UnityEvent PlayerSpawned;
    [SerializeField]
    UnityEvent<float> FreezSpawning; //freeze duration

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

        playerModelHandler = GetComponentInChildren<PlayerModelHandler>();
    }

    private void Start()
    {
        SpawnPlayer();
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
        bool isDashing = droneMovement.enabled? droneMovement.isDashing : false;
        if (playerInput.LookInput.y != 0 && !isDashing)
            planeLook.LookUpDown(playerInput.LookInput.y);
        if (playerInput.LookInput.x != 0 && !isDashing)
            planeLook.LookLeftRight(playerInput.LookInput.x);

        //Rotation
        if (playerInput.rotateLeftAction.IsPressed())
            planeLook.RotateLeft();
        if (playerInput.rotateRightAction.IsPressed())
            planeLook.RotateRight();
    }

    void HandleMovementInput() 
    {
        //Dashing
        if (playerInput.dashRightAction.IsPressed())
            droneMovement.Dash(Vector3.right, Vector3.back);
        if (playerInput.dashLeftAction.IsPressed())
            droneMovement.Dash(Vector3.left, Vector3.forward);
        if (playerInput.dashUpAction.IsPressed())
            droneMovement.Dash(Vector3.up, Vector3.down);
        if (playerInput.dashDownAction.IsPressed())
            droneMovement.Dash(Vector3.down, Vector3.up);


        if (playerInput.dashBackwardAction.IsPressed())
            droneMovement.Dash(Vector3.back, Vector3.left);
        if (playerInput.dashForwardAction.IsPressed())
            droneMovement.Dash(Vector3.forward, Vector3.right);
    }
    #endregion

    #region CrashHandler
    public void PlayerCrashed(ControllerColliderHit hit)
    {
        DisablePlayer();
        if (UserData.Instance.automaticRespawn)
            InitiatePlayerSpawning();
        //else 
            //InstanitateDeathScreen();
    }

    void InitiatePlayerSpawning() 
    {
        int randomSpawnIndex = Random.Range(0, spawnPoints.Length);
        transform.position = spawnPoints[randomSpawnIndex].position;

        if (UserData.Instance.freezeBeforeSpawn)
            StartCoroutine(FreezeSpawn(freezeDuration));
        else
            SpawnPlayer();
    }

    void SpawnPlayer() 
    {
        PlayerSpawned.Invoke();
        EnablePlayer();
    }

    IEnumerator FreezeSpawn(float freezeTime) 
    {
        FreezSpawning.Invoke(freezeTime);

        planeLook.enabled = true;
        playerInput.enabled = true;
        PlayerModel.SetActive(true);

        yield return new WaitForSeconds(freezeTime);
        SpawnPlayer();
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
    }

}
