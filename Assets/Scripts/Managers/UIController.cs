using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class UIController : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;

    [SerializeField] private UnityEvent<Vector2> OnInspect;     // normalized / unified

    [SerializeField] private UnityEvent OnToggleInspectPressed;
    [SerializeField] private UnityEvent OnToggleInspectReleased;     

    private InputAction inspectAction;
    private InputAction toggleInspectAction;

    [Header("Sckemes")]
    [SerializeField] private string computerScheme = "Computer";
    [SerializeField] private string gamepadScheme = "Gamepad";
    [SerializeField] private string mobileScheme = "Mobile";

    public enum ControlScheme
    {
        Unknown,
        Computer,
        Gamepad,
        Mobile
    }

    public ControlScheme currentScheme = ControlScheme.Unknown;

    #region Set up
    void EnableActions()
    {
        inspectAction.Enable();
    }
    void DisabelActions()
    {
        inspectAction.Disable();
    }

    private void Awake()
    {
        inspectAction = playerInput.actions["Inspect"];
        toggleInspectAction = playerInput.actions["ToggleInspect"];
    }

    private void OnEnable()
    {
        EnableActions();

        playerInput.onControlsChanged += OnControlsChanged;
        OnControlsChanged(playerInput); // initialize current scheme
    }

    private void OnDisable()
    {
        playerInput.onControlsChanged -= OnControlsChanged;
        DisabelActions();
    }
    #endregion

    private void Update()
    {
        Vector2 raw = inspectAction.ReadValue<Vector2>();
        raw *= UserData.Instance.lookSensitivity;

        if (raw.sqrMagnitude > 0.01f)
            OnInspect.Invoke(raw);

        if(currentScheme == ControlScheme.Computer)
        {
            if(toggleInspectAction.WasPerformedThisFrame())
                OnToggleInspectPressed.Invoke();
            if (toggleInspectAction.WasCompletedThisFrame())
                OnToggleInspectReleased.Invoke();
        }
       
    }

    #region Control Scheme CHange
    private void OnControlsChanged(PlayerInput input)
    {
        currentScheme = SchemeFromString(input.currentControlScheme);

        if (currentScheme != ControlScheme.Computer)
            OnToggleInspectPressed.Invoke();
        else
            OnToggleInspectReleased.Invoke();

    }

    private ControlScheme SchemeFromString(string scheme)
    {
        if (scheme == computerScheme) return ControlScheme.Computer;
        if (scheme == gamepadScheme) return ControlScheme.Gamepad;
        if (scheme == mobileScheme) return ControlScheme.Mobile;

        Debug.LogWarning("Uknown Scheme: " + scheme);
        return ControlScheme.Unknown;
    }
    #endregion
}
