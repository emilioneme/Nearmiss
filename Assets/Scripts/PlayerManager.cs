using TMPro;
using UnityEngine;
using UnityEditor.Events;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(DroneMovement))]
[RequireComponent(typeof(PlayerUIManager))]

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    public float totalPoints;

    [SerializeField]
    public float maxDistancePoints = 10;
    [SerializeField]
    public float speedPointsMultiplier = .5f;
    [SerializeField]
    public float dashPointsMultiplier = 1.3f;

    [SerializeField]
    Transform spawnTransform;

    [SerializeField]
    UnityEvent HighScoreChange;

    [SerializeField]
    UnityEvent PointsChange;

    #region Managers
    [HideInInspector]
    public PlayerInput playerInput;
    [HideInInspector]
    public DroneMovement droneMovement;
    [HideInInspector]
    public PlayerUIManager playerUIManager;
    #endregion

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        droneMovement = GetComponent<DroneMovement>();
        playerUIManager = GetComponent<PlayerUIManager>();
    }

    private void Update()
    {
        if(droneMovement.movementEnabled)
        {
            droneMovement.ForwardVelocity();
            droneMovement.GravityVelocity();
        }

        if (playerInput.RotateLeftPressed)
            droneMovement.RotateLeft();
        if (playerInput.RotateRightPressed)
            droneMovement.RotateRight();

        if (playerInput.LookInput.y != 0) 
            droneMovement.LookUpDown(playerInput.LookInput.y);
        if (playerInput.LookInput.x != 0)
            droneMovement.LookLeftRight(playerInput.LookInput.x);

        if (playerInput.DashLeftPressed)
            droneMovement.Dash(-1, Vector3.forward);
        if (playerInput.DashRightPressed)
            droneMovement.Dash(1, Vector3.forward);

        if (playerInput.DashBackwardPressed)
            droneMovement.Dash(-1, Vector3.right);
        if (playerInput.DashForwardPressed)
            droneMovement.Dash(1, Vector3.right);

    }

    #region NearmissHandler
    public void PlayerNearmissed(float normalizedDistance) //This is a float from 0 to 1
    {
        totalPoints += DistancePoints(normalizedDistance) * SpeedPointsMultiplier();
        PointsChange.Invoke();
    }

    float DistancePoints(float normalizedDistance) 
    {
        return (normalizedDistance * maxDistancePoints);
    }

    float SpeedPointsMultiplier() 
    {
        return droneMovement.GetTotalSpeed() * speedPointsMultiplier;
    }

    #endregion

    #region CrashHandler
    public void PlayerCrashed(ControllerColliderHit hit)
    {
        playerUIManager.TriggerCrashUI(hit);
        StartCoroutine(CouritineSpawn());
    }

    IEnumerator CouritineSpawn() 
    {
        droneMovement.enabled = false;
        transform.position = spawnTransform.position;
        droneMovement.enabled = true;

        droneMovement.flyingEnabled = false;
        droneMovement.gravityEnabled = false;
        droneMovement.dashingEnabled = false;
        ResetPoints();
        yield return new WaitForSeconds(2f);
        droneMovement.flyingEnabled = true;
        droneMovement.gravityEnabled = true;
        droneMovement.dashingEnabled = true;
        //SceneManager.LoadScene(0);
    }

    void ResetPoints() 
    {
        if(GameManager.Instance.highScore < totalPoints) 
        {
            GameManager.Instance.highScore = totalPoints;
            HighScoreChange.Invoke();
        }
        totalPoints = 0;
        PointsChange.Invoke();
    }
    #endregion

}
