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
    [Header("Points")]
    [SerializeField]
    public float totalPoints;
    [SerializeField]
    public float expectedPoints;

    #region  Multipliers
    [Header("Point Multipliers")]
    [SerializeField]
    public float maxDistancePoints = 10;
    [SerializeField]
    public float speedPointsMultiplier = .5f;
    [SerializeField]
    public float dashPointsMultiplier = 1.3f;
    #endregion

    #region Expected Points
    float lastNearmiss = 0;
    [SerializeField]
    float minTimeBeforeNextCombo = 0.75f;
    [SerializeField]
    float timeToSecureCombo = 1.5f;
    Coroutine comboTimer;
    #endregion

    [SerializeField]
    Transform spawnTransform;

    #region Events
    [SerializeField]
    UnityEvent HighScoreChange;
    [SerializeField]
    UnityEvent PointsChange;
    [SerializeField]
    UnityEvent ExpectedPointsChange;
    [SerializeField]
    UnityEvent ExpectedPointsFailed;
    #endregion

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
    #endregion

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        droneMovement = GetComponent<DroneMovement>();
        playerUIManager = GetComponent<PlayerUIManager>();
        nearmissHandler = GetComponent<NearmissHandler>();
        collisionHandler = GetComponent<CollisionHandler>();
        playerModelHandler = GetComponentInChildren<PlayerModelHandler>();
    }

    private void Start()
    {
        StartCoroutine(SpawnCorutine());
    }

    private void Update()
    {
        //Moving
        if(droneMovement.movementEnabled)
        {
            droneMovement.ForwardVelocity();
            droneMovement.GravityVelocity();
        }

        //Look
        if (playerInput.LookInput.y != 0) 
            droneMovement.LookUpDown(playerInput.LookInput.y);
        if (playerInput.LookInput.x != 0)
            droneMovement.LookLeftRight(playerInput.LookInput.x);

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

    #region NearmissHandler
    public void PlayerNearmissed(float normalizedDistance, float distance, Vector3 playerPos, RaycastHit hit) //This is a float from 0 to 1
    {
        lastNearmiss = Time.time;
        expectedPoints += TotalPointsCalculation(normalizedDistance);
        ExpectedPointsChange.Invoke();
        if(comboTimer != null) 
        {
            StopCoroutine(comboTimer);
            comboTimer = null;
        } 
        comboTimer = StartCoroutine(ComboTimer());
    }

    #region Combo
    IEnumerator ComboTimer() 
    {
        while(ComboCooldown() < 1) 
            yield return null;
        PointsSecured();
        comboTimer = null;
    }

    public float ComboCooldown() 
    {
        float c = (Time.time - lastNearmiss) / timeToSecureCombo;
        if (c < 1)
            return c;
        return 1;
    }

    void PointsSecured() 
    {
        totalPoints += expectedPoints;
        expectedPoints = 0;
        PointsChange.Invoke();
    }

    #endregion

    #region Point Fomrulas
    float TotalPointsCalculation(float normalizedDistance) 
    {
        return DistancePoints(normalizedDistance) * SpeedPointsMultiplier();
    }

    float DistancePoints(float normalizedDistance) 
    {
        return (normalizedDistance * maxDistancePoints);
    }
    float SpeedPointsMultiplier() 
    {
        return droneMovement.GetTotalVelocity().magnitude * speedPointsMultiplier;
    }
    #endregion


    #endregion

    #region CrashHandler
    public void PlayerCrashed(ControllerColliderHit hit)
    {
        playerUIManager.TriggerCrashUI(hit);
        StartCoroutine(SpawnCorutine());
    }

    IEnumerator SpawnCorutine() 
    {
        SpawnPlayer();
        ToggleFreeze(true);
        ResetPoints();
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
        droneMovement.flyingEnabled = !freeze;
        droneMovement.gravityEnabled = !freeze;
        droneMovement.dashingEnabled = !freeze;

        nearmissHandler.on = !freeze;
        collisionHandler.on = !freeze;

    }
    void ResetPoints() 
    {
        if(GameManager.Instance.highScore < totalPoints) 
        {
            GameManager.Instance.highScore = totalPoints;
            HighScoreChange.Invoke();
        }
        totalPoints = 0;
        expectedPoints = 0;
        PointsChange.Invoke();
    }
    #endregion

}
