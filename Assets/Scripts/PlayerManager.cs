using TMPro;
using UnityEngine;
using UnityEditor.Events;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(DroneMovement))]
[RequireComponent(typeof(PlaneLook))]
[RequireComponent(typeof(NearmissHandler))]
[RequireComponent(typeof(CollisionHandler))]
[RequireComponent(typeof(PointManager))]
[RequireComponent(typeof(PlayerUIManager))]

public class PlayerManager : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField]
    Transform spawnTransform;

    [Header("Events")]
    [SerializeField]
    UnityEvent PlayerCrash;
    [SerializeField]
    UnityEvent PlayerSpawned;

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
        playerUIManager = GetComponent<PlayerUIManager>();
        nearmissHandler = GetComponent<NearmissHandler>();
        collisionHandler = GetComponent<CollisionHandler>();
        pointManager = GetComponent<PointManager>();
        planeLook = GetComponent<PlaneLook>();


        playerModelHandler = GetComponentInChildren<PlayerModelHandler>();
    }

    private void Start()
    {
        StartCoroutine(SpawnCorutine());
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
        playerUIManager.TriggerCrashUI(hit);
        StartCoroutine(SpawnCorutine());
    }

    IEnumerator SpawnCorutine() 
    {

        ToggleFreeze(true);
        PlayerCrash.Invoke();
        SpawnPlayer();

        yield return new WaitForSeconds(2f);
        ToggleFreeze(false);
    }

    void SpawnPlayer() 
    {
        PlayerSpawned.Invoke();
        bool enabled = droneMovement.enabled;
        droneMovement.enabled = false;
        transform.position = spawnTransform.position;
        droneMovement.enabled = enabled;
    }

    void ToggleFreeze(bool freeze = true) 
    {
        droneMovement.enabled = !freeze;
        nearmissHandler.enabled = !freeze;
        collisionHandler.enabled = !freeze;
    }
    #endregion

}
