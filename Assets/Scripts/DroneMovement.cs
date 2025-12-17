using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEditor.FilePathAttribute;

[RequireComponent(typeof(CharacterController))]
public class DroneMovement : MonoBehaviour
{
    public Vector3 velocity = Vector3.zero;

    [HideInInspector]
    public CharacterController cc;

    [Header("Flight Forward")]
    [SerializeField][Range(1, 50)]
    public float flyingSpeed = 20f;
    [SerializeField][Range(1, 50)]
    public float totalFlyingSpeedMultiplier = 1f;

    [Header("NoseDive")]
    [SerializeField][Range(1, 50)]
    public float noseDiveSpeedMultiplier = 10f;

    [Header("Gravity")]
    [SerializeField][Range(1, 50)]
    public float gravityForceMultiplier = 1.9f;


    [Header("Turning Rotation")]
    [SerializeField]
    public float rotationSpeedMultiplier = 1.0f;
    float rotationSpeed = 1;

    [SerializeField]
    float yRotationInputThershhold = 3;
    [SerializeField]
    float xRotationInputThershhold = 3;

    [Header("Look")]
    [SerializeField]
    public float lookSpeedMultiplier = 1.0f;
    float lookSpeed = 1;

    [Header("Dash")]
    [SerializeField]
    float dashCooldwon = 3;
    [SerializeField]
    public float dashSpeed = 10;
    [SerializeField]
    public float dashDuration = .75f;
    float lastTimeDashed = 0;
    [HideInInspector]
    public bool isDashing;

    [SerializeField]
    UnityEvent<int, Vector3, float> DashStarted;


    [Header("Settings Toggles")]
    [SerializeField]
    public bool movementEnabled = true;
    public bool flyingEnabled, rotatingEnabled, lookingEnabled, lookRotationEnabled, gravityEnabled, dashingEnabled = true;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }


    #region FlyForward
    public void Fly() 
    {
        if (!flyingEnabled)
            return;
        cc.Move(transform.forward * (CurrentForwardSpeed() * Time.deltaTime));
    }

    public float CurrentForwardSpeed() 
    {
        float baseSpeed = flyingSpeed;
        float noseDiveSpeed = NoseDiveSpeed() * noseDiveSpeedMultiplier;
        return (baseSpeed + noseDiveSpeed) * totalFlyingSpeedMultiplier;
    }

    public float NoseDiveSpeed() 
    {
        float dot = Vector3.Dot(transform.forward, Vector3.down);
        return Mathf.InverseLerp(-1f, 1f, dot);
    }
    #endregion

    #region Gravity
    public void ApplyGravity()
    {
        if(!gravityEnabled)
            return;
        cc.Move(Vector3.down * (CurrentDownSpeed() * Time.deltaTime));
    }

    public float CurrentDownSpeed()
    {
        float downSpeed = GravityForceSpeed() * gravityForceMultiplier;
        return downSpeed;
    }

    public float GravityForceSpeed() 
    {
        float dot = Vector3.Dot(transform.up,Vector3.up);
        float flatAmount = Mathf.Abs(dot);
        float gravityFactor = 1f - flatAmount;  // 0 when flat, 1 when vertical
        return gravityFactor;
    }
    #endregion

    #region Rotate
    public void Rotate(int direction)
    {
        float rotationAmount = rotationSpeed * rotationSpeedMultiplier * Time.deltaTime;
        transform.rotation = transform.rotation * Quaternion.Euler(0, 0, rotationAmount * direction);
    }

    public void RotateLeft()
    {
        if (!rotatingEnabled)
            return;
        Rotate(1);
    }

    public void RotateRight()
    {
        if (!rotatingEnabled)
            return;
        Rotate(-1);
    }
    #endregion

    #region Look
    public void LookUpDown(float y)
    {
        if (!lookingEnabled)
            return;
        float yClamped = Mathf.Clamp(y, -yRotationInputThershhold, yRotationInputThershhold);
        float loookAmount = -yClamped * lookSpeed * lookSpeedMultiplier * Time.deltaTime;
        transform.rotation = transform.rotation * Quaternion.Euler(loookAmount, 0, 0);
    }
    public void LookLeftRight(float x)
    {
        if (!lookingEnabled)
            return;
        float xClamped = Mathf.Clamp(x, -xRotationInputThershhold, xRotationInputThershhold);
        float loookAmount = xClamped * lookSpeed * lookSpeedMultiplier * Time.deltaTime;
        transform.rotation = transform.rotation * Quaternion.Euler(0, loookAmount, 0);
        if(x > 0 && lookRotationEnabled) 
            Rotate(-1);
        else if (x < 0 && lookRotationEnabled)
            Rotate(1);
    }
    #endregion

    #region Dash
    bool CanDash()
    {
        if (dashCooldownFloat() < 1)
            return false;
        return true;
    }

    float dashCooldownFloat()
    {
        float cooldown = (Time.time - lastTimeDashed) / dashCooldwon;
        return cooldown < 1 ? cooldown : 1;
    }

    public void Dash(int direction, Vector3 animateAxis) 
    {
        if(!CanDash()) 
            return;
        lastTimeDashed = Time.time;
        StartCoroutine(DashCoroutine(direction, animateAxis));
        DashStarted.Invoke(direction, animateAxis, dashDuration);
    }

    IEnumerator DashCoroutine(int direction, Vector3 animateAxis)
    {
        isDashing = true;
        float timer = 0f;
        while (timer < dashDuration)
        {
            if (animateAxis == Vector3.forward) //side dash
                cc.Move(transform.right * direction * dashSpeed * Time.deltaTime);
            if (animateAxis == Vector3.right) //forwardsa
                cc.Move(transform.forward * direction * dashSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
        isDashing = false;
    }

    #endregion

    public float GetTotalSpeed() 
    {
        float currentDashSpeed = isDashing ? dashSpeed : 0;
        return CurrentDownSpeed() + CurrentForwardSpeed() + currentDashSpeed;
    }
    public float GetVelocity() 
    {
        return cc.velocity.magnitude;
    }

    private void OnEnable()
    {
        cc.enabled = true;
    }

    private void OnDisable()
    {
        cc.enabled = false;
    }
  
}
