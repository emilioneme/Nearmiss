using TMPro;
using UnityEngine;
using UnityEditor.Events;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(DroneMovement))]
[RequireComponent(typeof(PlayerUIManager))]
public class PlayerManager : MonoBehaviour
{

    [Header("Spawn")]
    [SerializeField]
    Transform spawnTransform;

    [SerializeField]
    UnityEvent PlayerCrash;

    #region Managers
    [HideInInspector]
    public PlayerInput playerInput;
    [HideInInspector]
    public DroneMovement droneMovement;
    [HideInInspector]
    public PlayerUIManager playerUIManager;
    [HideInInspector]
    public NearmissHandler nearmissHandler;
    [HideInInspector]
    public CollisionHandler collisionHandler;
    [HideInInspector]
    public PlayerModelHandler playerModelHandler;
    [HideInInspector]
    public PointManager pointManager;
    #endregion

    private void Awake()
    {
        
        playerInput = GetComponent<PlayerInput>();
        droneMovement = GetComponent<DroneMovement>();
        playerUIManager = GetComponent<PlayerUIManager>();
        nearmissHandler = GetComponent<NearmissHandler>();
        collisionHandler = GetComponent<CollisionHandler>();
        pointManager = GetComponent<PointManager>();
        

        playerModelHandler = GetComponentInChildren<PlayerModelHandler>();
    }

    private void Start()
    {
        StartCoroutine(SpawnCorutine());
    }

    private void Update()
    {
        HandleMovement();
    }

    #region Movmeent
    void HandleMovement() 
    {
        droneMovement.MoveDrone();

        //Look
        if (playerInput.LookInput.y != 0)
            droneMovement.LookUpDown(playerInput.LookInput.y);
        if (playerInput.LookInput.x != 0)
            droneMovement.LookLeftRight(playerInput.LookInput.x);

        if (droneMovement.enableFlying) 
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

            //Rotation
            if (playerInput.rotateLeftAction.IsPressed())
                droneMovement.RotateLeft();
            if (playerInput.rotateRightAction.IsPressed())
                droneMovement.RotateRight();
        }
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

        SpawnPlayer();
        PlayerCrash.Invoke();

        yield return new WaitForSeconds(2f);
        ToggleFreeze(false);
    }

    void SpawnPlayer() 
    {
        droneMovement.enabled = false;
        transform.position = spawnTransform.position;
        droneMovement.enabled = true;
    }
    void ToggleFreeze(bool freeze = true) 
    {
        droneMovement.enableFlying = !freeze;
        droneMovement.applyGravity = !freeze;
        nearmissHandler.enabled = !freeze;
        collisionHandler.enabled = !freeze;

    }
    #endregion

}
