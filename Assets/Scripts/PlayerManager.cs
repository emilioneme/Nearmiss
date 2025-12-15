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
        yield return new WaitForSeconds(2f);
        droneMovement.flying = true;
        droneMovement.gravity = true;
    }
    
}
