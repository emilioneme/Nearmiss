using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerInput : MonoBehaviour
{
    [Header("Look")]
    [SerializeField]
    private InputAction lookAction;
    [HideInInspector]
    public Vector2 LookInput;

    [Header("Rotate")]
    [SerializeField]
    public InputAction rotateLeftAction;
    [SerializeField]
    public InputAction rotateRightAction;

    [Header("Dash to Side")]
    [SerializeField]
    public InputAction dashAction;
    [HideInInspector]
    public Vector2 DashInput;

    [Header("Dash Forward and Back")]
    [SerializeField]
    public InputAction dashForwardAction;
    [SerializeField]
    public InputAction dashBackwardAction;

    [Header("Pause")]
    public InputAction pauseAction;


    private void OnEnable()
    {
        lookAction.Enable();

        rotateLeftAction.Enable();
        rotateRightAction.Enable();

        dashAction.Enable();

        dashForwardAction.Enable();
        dashBackwardAction.Enable();

        pauseAction.Enable();
    }

    private void OnDisable()
    {
        lookAction.Disable();

        rotateLeftAction.Disable();
        rotateRightAction.Disable();

        dashForwardAction.Disable();
        dashBackwardAction.Disable();

        pauseAction.Disable();
    }


    private void Update()
    {
        Vector2 rawLook = lookAction.ReadValue<Vector2>();
        Vector3 look;
        if (lookAction.activeControl?.device is Mouse)
            look = rawLook * UserData.Instance.mouseSensitivity;
        else
            look = rawLook * UserData.Instance.stickSensitivity;

        LookInput = look;

        DashInput = dashAction.ReadValue<Vector2>();

    }

}
