using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    private InputAction lookAction;

    [SerializeField]
    private InputAction rotateLeftAction;
    [SerializeField]
    private InputAction rotateRightAction;

    [SerializeField]
    private InputAction dashLeftAction;
    [SerializeField]
    private InputAction dashRightAction;

    [SerializeField]
    private InputAction dashForwardAction;
    [SerializeField]
    private InputAction dashBackwardAction;



    public Vector2 LoookInput { get; private set; }
    public bool RotateLeftPressed { get; private set; }
    public bool RotateRightPressed { get; private set; }

    public bool DashLeftPressed { get; private set; }
    public bool DashRightPressed { get; private set; }

    public bool DashForwardPressed { get; private set; }
    public bool DashBackwardPressed { get; private set; }


    private void OnEnable()
    {
        lookAction.Enable();

        rotateLeftAction.Enable();
        rotateRightAction.Enable();

        dashLeftAction.Enable();
        dashRightAction.Enable();

        dashForwardAction.Enable();
        dashBackwardAction.Enable();
    }

    private void OnDisable()
    {
        lookAction.Disable();

        rotateLeftAction.Disable();
        rotateRightAction.Disable();

        dashLeftAction.Disable();
        dashRightAction.Disable();

        dashForwardAction.Disable();
        dashBackwardAction.Disable();
    }


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        LoookInput = lookAction.ReadValue<Vector2>();

        RotateLeftPressed = rotateLeftAction.IsPressed();
        RotateRightPressed = rotateRightAction.IsPressed();

        DashLeftPressed = dashLeftAction.IsPressed();
        DashRightPressed = dashRightAction.IsPressed();

        DashForwardPressed = dashForwardAction.IsPressed();
        DashBackwardPressed = dashBackwardAction.IsPressed();
    }
}
