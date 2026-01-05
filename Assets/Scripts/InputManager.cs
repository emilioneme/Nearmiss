using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    #region Singleton
    public static InputManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion


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
    public InputAction dashLeftAction;
    [SerializeField]
    public InputAction dashRightAction;

    [SerializeField]
    public InputAction dashUpAction;
    [SerializeField]
    public InputAction dashDownAction;

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

        dashLeftAction.Enable();
        dashRightAction.Enable();
        dashUpAction.Enable();
        dashDownAction.Enable();

        dashForwardAction.Enable();
        dashBackwardAction.Enable();

        pauseAction.Enable();
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

        pauseAction.Disable();
    }


    private void Update()
    {
        Vector2 raw = lookAction.ReadValue<Vector2>();
        Vector3 look;
        if (lookAction.activeControl?.device is Mouse)
            look = raw * UserData.Instance.mouseSensitivity;
        else
            look = raw * UserData.Instance.stickSensitivity;

        LookInput = look;
    }

}
