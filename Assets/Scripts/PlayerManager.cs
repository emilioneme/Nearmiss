using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(DroneMovement))]

public class PlayerManager : MonoBehaviour
{
    PlayerInput playerInput;
    DroneMovement droneMovement;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        droneMovement = GetComponent<DroneMovement>();
    }

    // Update is called once per frame
    private void Update()
    {
        droneMovement.FlyForward(); 

        if (playerInput.RotateLeftPressed)
            droneMovement.RotateLeft();
        if (playerInput.RotateRightPressed)
            droneMovement.RotateRight();
        if (playerInput.AcceleratePressed)
            droneMovement.Accelerate();
        if (playerInput.DeacceleratePressed)
            droneMovement.Deaccelerate();
        if (playerInput.LoookInput.y != 0)
            droneMovement.LookUpDown(playerInput.LoookInput.y);
        if (playerInput.LoookInput.x != 0)
            droneMovement.LookLeftRight(playerInput.LoookInput.x);
    }
}
