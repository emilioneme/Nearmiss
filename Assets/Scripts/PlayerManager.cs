using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(DroneMovement))]

public class PlayerManager : MonoBehaviour
{
    PlayerInput playerInput;
    DroneMovement droneMovement;

    [SerializeField]
    bool flying = true;
    [SerializeField]
    bool rotating = true;
    [SerializeField]
    bool looking = true;
    [SerializeField]
    bool lookRotation = true;
    [SerializeField]
    bool gravity = true;

    [Header("UI")]
    [SerializeField] TMP_Text SpeedText;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        droneMovement = GetComponent<DroneMovement>();
    }

    // Update is called once per frame
    private void Update()
    {
        if(flying)
            droneMovement.Fly();

        if (gravity)
            droneMovement.ApplyGravity();

        if (rotating) 
        {
            if (playerInput.RotateLeftPressed)
                droneMovement.Rotate(1);
            if (playerInput.RotateRightPressed)
                droneMovement.Rotate(-1);
        }

        if(looking) 
        {
            if (playerInput.LoookInput.y != 0)
                droneMovement.LookUpDown(playerInput.LoookInput.y);
            if (playerInput.LoookInput.x != 0)
                droneMovement.LookLeftRight(playerInput.LoookInput.x);
        }
        
        droneMovement.lookRotation = lookRotation;


        SpeedText.text = ((int)droneMovement.CurrentForwardSpeed()).ToString();
    }
}
