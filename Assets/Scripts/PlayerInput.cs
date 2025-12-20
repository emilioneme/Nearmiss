using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    private InputAction lookAction;

    [HideInInspector]
    public Vector2 LookInput;

    [SerializeField]
    public InputAction rotateLeftAction;
    [SerializeField]
    public InputAction rotateRightAction;

    [SerializeField]
    public InputAction dashLeftAction;
    [SerializeField]
    public InputAction dashRightAction;

    [SerializeField]
    public InputAction dashUpAction;
    [SerializeField]
    public InputAction dashDownAction;

    [SerializeField]
    public InputAction dashForwardAction;
    [SerializeField]
    public InputAction dashBackwardAction;


    private void OnEnable()
    {
        lookAction.Enable();

        rotateLeftAction.Enable();
        rotateRightAction.Enable();

        dashLeftAction.Enable();
        dashRightAction.Enable();
        dashUpAction.Enable();
        dashDownAction.Enable();

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
        dashUpAction.Disable();
        dashDownAction.Disable();

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
        Vector3 look;
        if (lookAction.activeControl?.device is Mouse)
            look = raw * GameManager.Instance.mouseSensitivity;
        else
            look = raw * GameManager.Instance.stickSensitivity;

        LookInput = look;
    }

}
