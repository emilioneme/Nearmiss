using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;

    [Header("Interpreted Output (wire in Inspector)")]
    [SerializeField] private UnityEvent<Vector2> OnLook;     // normalized / unified
    [SerializeField] private UnityEvent OnRotateLeft;
    [SerializeField] private UnityEvent OnRotateRight;
    [SerializeField] private UnityEvent<Vector2> OnDash;     // direction intent (-1..1)

    [Header("Look Tuning")]
    [Tooltip("Stick is already -1..1. Mouse is usually delta-per-frame.")]
    [Header("Mouse")]
    [SerializeField] private float mouseLookScale = 1f;
    [Header("Touch")]
    [SerializeField] private float touchLookScale = 1f;
    [Header("Stick")]
    [SerializeField] private float stickDeadzone = 1f;


    [Header("Dash Tuning")]
    [SerializeField] private float dashDeadzone = 0.25f;

    [Header("Sckemes")]
    [SerializeField] private string computerScheme = "Keyboard and Mouse";
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

    private InputAction lookAction;
    private InputAction rotateLeftAction;
    private InputAction rotateRightAction;
    private InputAction dashAction;

    #region Set up
    void EnableActions()
    {
        lookAction.Enable();
        rotateLeftAction.Enable();
        rotateRightAction.Enable();
        dashAction.Enable();
    }
    void DisabelActions()
    {
        lookAction.Disable();
        rotateLeftAction.Disable();
        rotateRightAction.Disable();
        dashAction.Disable();
    }

    private void Awake()
    {
        lookAction = playerInput.actions["Look"];
        rotateLeftAction = playerInput.actions["RotateLeft"];
        rotateRightAction = playerInput.actions["RotateRight"];
        dashAction = playerInput.actions["Dash"];
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
        Vector2 rawLook = lookAction.ReadValue<Vector2>();
        Vector2 interpretedLook = InterpretLook(rawLook);
        interpretedLook *= UserData.Instance.lookSensitivity;

        if (interpretedLook.sqrMagnitude > 0.000001f)
            OnLook.Invoke(interpretedLook);

        if (dashAction.IsPressed())
            OnDashPerformed(dashAction.ReadValue<Vector2>());

        if (rotateLeftAction.inProgress)
            OnRotateLeft.Invoke();

        if (rotateRightAction.inProgress)
            OnRotateRight.Invoke();

    }

    #region Control Scheme CHange
    private void OnControlsChanged(PlayerInput input)
    {
        currentScheme = SchemeFromString(input.currentControlScheme);
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
    private Vector2 InterpretLook(Vector2 raw)
    {
        if (currentScheme == ControlScheme.Gamepad)
        {
            raw = ApplyRadialDeadzone(raw, stickDeadzone);// Stick: already -1..1, apply deadzone.
            return raw;
        }

        if (currentScheme == ControlScheme.Mobile)
        {
            Vector2 scaled = raw * touchLookScale; // Mouse/touch: often comes as delta-per-frame. Scale to feel like stick.
            return scaled;
        }

        if (currentScheme == ControlScheme.Computer)
        {
            Vector2 mouseScaled = raw * mouseLookScale;
            return mouseScaled;
        }

        return Vector2.zero;
    }

    private void OnDashPerformed(Vector2 v)
    {
        if (v.sqrMagnitude < dashDeadzone * dashDeadzone)
            return;
        OnDash.Invoke(v.normalized);
    }

    #region tools
    private static Vector2 ApplyRadialDeadzone(Vector2 v, float deadzone)
    {
        float mag = v.magnitude;
        if (mag <= deadzone)
            return Vector2.zero;

        // Rescale so it starts at 0 after deadzone (nice feel)
        float scaled = (mag - deadzone) / (1f - deadzone);
        return v.normalized * Mathf.Clamp01(scaled);
    }
    #endregion
}
