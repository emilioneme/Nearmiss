using TMPro;
using UnityEngine;
using UnityEditor.Events;
using UnityEngine.Events;
using System.Collections;

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
        droneMovement.Fly();

        droneMovement.ApplyGravity();

        if (playerInput.RotateLeftPressed)
            droneMovement.RotateLeft();
        if (playerInput.RotateRightPressed)
            droneMovement.RotateRight();

        if (playerInput.LoookInput.y != 0) 
            droneMovement.LookUpDown(playerInput.LoookInput.y);
        if (playerInput.LoookInput.x != 0)
            droneMovement.LookLeftRight(playerInput.LoookInput.x);

        if (playerInput.DashLeftPressed)
            droneMovement.Rotate(1);
        if (playerInput.DashRightPressed)
            droneMovement.Rotate(-1);

    }

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

    #region CrashHandler
    public void PlayerCrashed(ControllerColliderHit hit)
    {
        playerUIManager.TriggerCrashUI(hit);
        StartCoroutine(CouritineSpawn());
    }

    IEnumerator CouritineSpawn() 
    {
        droneMovement.enabled = false;
        transform.position = new Vector3(0, 10, 0); // spawn pints
        droneMovement.enabled = true;
        droneMovement.flying = false;
        droneMovement.gravity = false;
        ResetPoints();
        yield return new WaitForSeconds(2f);
        droneMovement.flying = true;
        droneMovement.gravity = true;
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
