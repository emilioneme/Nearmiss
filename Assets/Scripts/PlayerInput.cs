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
    private InputAction deaccelerateAction;
    [SerializeField]
    private InputAction accelerateAction;

    public Vector2 LoookInput { get; private set; }
    public bool RotateLeftPressed { get; private set; }
    public bool RotateRightPressed { get; private set; }
    public bool DeacceleratePressed { get; private set; }
    public bool AcceleratePressed { get; private set; }

    private void OnEnable()
    {
        lookAction.Enable();
        rotateLeftAction.Enable();
        rotateRightAction.Enable();
        deaccelerateAction.Enable();
        accelerateAction.Enable();
    }

    private void OnDisable()
    {
        lookAction.Disable();
        rotateLeftAction.Disable();
        rotateRightAction.Disable();
        deaccelerateAction.Disable();
        accelerateAction.Disable();
    }


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        LoookInput = lookAction.ReadValue<Vector2>();
        DeacceleratePressed = deaccelerateAction.IsPressed();
        AcceleratePressed = accelerateAction.IsPressed();
        RotateLeftPressed = rotateLeftAction.IsPressed();
        RotateRightPressed = rotateRightAction.IsPressed();
    }
}
