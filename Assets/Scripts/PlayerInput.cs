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



    public Vector2 LookInput { get; private set; }
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
        Vector2 raw = lookAction.ReadValue<Vector2>();
        var device = lookAction.activeControl?.device;
        if (device is Mouse)
        {
            // Mouse is already a "delta this frame"
            Vector2 scaled = raw * GameManager.Instance.mouseSensitivity;

            // optional clamp if you really want it
            scaled.x = Mathf.Clamp(scaled.x, -5f, 5f);
            scaled.y = Mathf.Clamp(scaled.y, -5f, 5f);

            LookInput = scaled;
        }
        else // assume stick (gamepad / joystick)
        {

            // curve so small movements are fine, big movements ramp up
            Vector2 v = new Vector2(
                Mathf.Sign(raw.x) * Mathf.Pow(Mathf.Abs(raw.x), GameManager.Instance.stickExponent),
                Mathf.Sign(raw.y) * Mathf.Pow(Mathf.Abs(raw.y), GameManager.Instance.stickExponent)
            );

            // convert to "delta this frame" using deg/sec * dt
            Vector2 scaled = v * GameManager.Instance.stickSensitivity * Time.deltaTime;

            LookInput = scaled;
        }

        RotateLeftPressed = rotateLeftAction.IsPressed();
        RotateRightPressed = rotateRightAction.IsPressed();

        DashLeftPressed = dashLeftAction.IsPressed();
        DashRightPressed = dashRightAction.IsPressed();

        DashForwardPressed = dashForwardAction.IsPressed();
        DashBackwardPressed = dashBackwardAction.IsPressed();
    }
}
