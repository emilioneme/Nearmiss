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
    [SerializeField]
    public float runningPoints;
    [SerializeField]
    public float comboMultiplier = 1;

    #region  Multipliers
    [Header("Point Multipliers")]
    [SerializeField]
    public float maxDistancePoints = 10;
    [SerializeField]
    public float speedPointsMultiplier = .5f;
    [SerializeField][Range(0f, 5f)]
    public float comboMultiplierIncrease = 3f;
    #endregion

    #region Expected Points
    float lastNearmiss = 0;
    [Header("Time For")]
    [SerializeField][Range(0, 10f)]
    float timeToSecurePoints = 1.5f;
    [SerializeField][Range(0, 0.9f)]
    float minTimeForComboMult = .5f; //PErcenttage of 
    #endregion

    [Header("Spawn")]
    [SerializeField]
    Transform spawnTransform;

    #region Events
    [SerializeField]
    UnityEvent PointsChange;
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
        HandleMovement();
    }

    #region Input
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

    #region NearmissHandler
    Coroutine secureTimer;
    public void PlayerNearmissed(float normalizedDistance, float distance, Vector3 playerPos, RaycastHit hit) //This is a float from 0 to 1
    {
        if (ComboMultCooldwon() >= 1)
            IncreaseComboMultiplier();

        lastNearmiss = Time.time;

        runningPoints += RunnignPointsCalculation(normalizedDistance);
        expectedPoints += runningPoints * comboMultiplier;

        if (secureTimer != null) 
        {
            StopCoroutine(secureTimer);
            secureTimer = null;
        }
        secureTimer = StartCoroutine(SecureTimer());
    }

    #region SecurePoints
    public float SecurePointsCooldown() 
    {
        return eneme.Tools.CooldownSince(lastNearmiss, timeToSecurePoints);
    }
    IEnumerator SecureTimer() 
    {
        while (SecurePointsCooldown() < 1) 
        {
            yield return null;
        }
        PointsSecured();
        secureTimer = null;
    }

    void PointsSecured() 
    {
        totalPoints = expectedPoints;
        runningPoints = 0;
        comboMultiplier = 1;
        if (secureTimer != null)
        {
            StopCoroutine(secureTimer);
            secureTimer = null;
        }
        PointsChange.Invoke();
    }
    #endregion

    #region Combo Multiuplier
    public float ComboMultCooldwon() 
    {
        return eneme.Tools.CooldownSince(lastNearmiss, minTimeForComboMult * timeToSecurePoints);
    }
    void IncreaseComboMultiplier()
    {
        comboMultiplier = comboMultiplier + comboMultiplierIncrease;
    }
    #endregion

    #region Point Fomrulas
    float RunnignPointsCalculation(float normalizedDistance) 
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
        droneMovement.enableFlying = !freeze;
        droneMovement.applyGravity = !freeze;
        nearmissHandler.on = !freeze;
        collisionHandler.on = !freeze;

    }
    void ResetPoints() 
    {
        if(GameManager.Instance.highScore < totalPoints) 
        {
            GameManager.Instance.highScore = totalPoints;
            //HighScoreChange.Invoke();
        }
        totalPoints = 0;
        expectedPoints = 0;
        runningPoints = 0;
        comboMultiplier = 1;

        PointsChange.Invoke();
    }
    #endregion

}
