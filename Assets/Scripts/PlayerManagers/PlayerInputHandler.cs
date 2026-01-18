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
    [Tooltip("Stick is already -1..1. Mouse is usually delta-per-frame, so scale it here.")]
    [SerializeField] private float mouseLookScale = 0.02f;
    [SerializeField] private float touchLookScale = 0.02f;
    [SerializeField] private float stickDeadzone = 0.15f;
    [SerializeField] private float lookClamp = 1f;

    [Header("Dash Tuning")]
    [SerializeField] private float dashDeadzone = 0.25f;
    private bool dashArmed = true;

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

        rotateLeftAction.performed += _ => OnRotateLeft.Invoke();
        rotateRightAction.performed += _ => OnRotateRight.Invoke();

        dashAction.performed += OnDashPerformed; // interpret dash before unity event invoke
        dashAction.canceled -= OnDashCanceled; //

        playerInput.onControlsChanged += OnControlsChanged;
        OnControlsChanged(playerInput); // initialize current scheme
    }

    private void OnDisable()
    {
        playerInput.onControlsChanged -= OnControlsChanged;
        DisabelActions();
    }

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

    private void Update()
    {
        Vector2 rawLook = lookAction.ReadValue<Vector2>();
        Vector2 interpretedLook = InterpretLook(rawLook);
        interpretedLook *= UserData.Instance.lookSensitivity;

        if (interpretedLook.sqrMagnitude > 0.000001f)
            OnLook.Invoke(interpretedLook);
    }

    private Vector2 InterpretLook(Vector2 raw)
    {
        if (currentScheme == ControlScheme.Gamepad)
        {
            raw = ApplyRadialDeadzone(raw, stickDeadzone);// Stick: already -1..1, apply deadzone.
            return Vector2.ClampMagnitude(raw, lookClamp);
        }

        if (currentScheme == ControlScheme.Mobile)
        {
            Vector2 scaled = raw * touchLookScale; // Mouse/touch: often comes as delta-per-frame. Scale to feel like stick.
            return Vector2.ClampMagnitude(scaled, lookClamp);
        }

        if (currentScheme == ControlScheme.Computer)
        {
            Vector2 mouseScaled = raw * mouseLookScale;
            return Vector2.ClampMagnitude(mouseScaled, lookClamp);
        }

        return Vector2.zero;
    }

    private void OnDashPerformed(InputAction.CallbackContext ctx)
    {
        if (!dashArmed)
            return;

        Vector2 v = ctx.ReadValue<Vector2>();

        if (v.sqrMagnitude < dashDeadzone * dashDeadzone)
            return;

        dashArmed = false;

        OnDash.Invoke(v.normalized);
    }

    private void OnDashCanceled(InputAction.CallbackContext ctx)
    {
        dashArmed = true;
    }

    private static Vector2 ApplyRadialDeadzone(Vector2 v, float deadzone)
    {
        float mag = v.magnitude;
        if (mag <= deadzone) return Vector2.zero;

        // Rescale so it starts at 0 after deadzone (nice feel)
        float scaled = (mag - deadzone) / (1f - deadzone);
        return v.normalized * Mathf.Clamp01(scaled);
    }
}
