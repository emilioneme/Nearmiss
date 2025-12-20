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
    public bool isDashing = false;

    Vector3 dashDirection;
    float dashCurrentSpeed;

    [SerializeField]
    AnimationCurve dashSpeedOverTime;

    [SerializeField]
    UnityEvent<Vector3, Vector3, float> DashStarted;


    [Header("Settings Toggles")]
    [SerializeField]
    public bool movementEnabled = true;
    public bool flyingEnabled, rotatingEnabled, lookingEnabled, lookRotationEnabled, gravityEnabled, dashingEnabled = true;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        ForwardVelocity();
        GravityVelocity();

#if UNITY_EDITOR
        if(!lookingEnabled)
            lookRotationEnabled = false;
#endif
    }

    #region FlyForward
    public void ForwardVelocity() 
    {
        if (!flyingEnabled)
            return;
        cc.Move(CurrentForwardVelocity() * Time.deltaTime);
    }

    public Vector3 CurrentForwardVelocity() 
    {
        return transform.forward * CurrentForwardSpeed();
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
    public void GravityVelocity()
    {
        if(!gravityEnabled)
            return;
        cc.Move(CurrentDownVelocity() * Time.deltaTime);
    }

    public Vector3 CurrentDownVelocity()
    {
        return Vector3.down * CurrentDownSpeed();
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

    public void Manuver(float magnitude, int direction)
    {
        float rotationAmount = rotationSpeed * rotationSpeedMultiplier * magnitude * Time.deltaTime;
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
        if (x > 0 && lookRotationEnabled)
            Manuver(x, -1);//Rotate(-1);
        else if (x < 0 && lookRotationEnabled)
            Manuver(x, -1);//Rotate(-1);
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

    public void Dash(Vector3 direction, Vector3 animateAxis) 
    {
        if(!CanDash()) 
            return;
        lastTimeDashed = Time.time;

        dashDirection = transform.rotation * direction; //set it wihtin class scope
        StartCoroutine(DashCoroutine()); //takes direction from the classes scope
        DashStarted.Invoke(direction, animateAxis, dashDuration);
    }

    IEnumerator DashCoroutine()
    {
        isDashing = true;
        float timer = 0f;
        while (timer < dashDuration)
        {
            //float t = Mathf.Abs((timer / dashDuration) - 1);
            float t = timer / dashDuration;
            dashCurrentSpeed = dashSpeed * dashSpeedOverTime.Evaluate(t);
            cc.Move(CurrentDashVelocity() * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
        isDashing = false;
    }

    Vector3 CurrentDashVelocity() // so we can add it to total velocity
    {
        if (!isDashing)
            return Vector3.zero;
        return dashDirection * dashCurrentSpeed;
    }

    #endregion

    #region Total Velocity
    public Vector3 GetTotalVelocity()
    {
        return CurrentDownVelocity() + CurrentForwardVelocity() + CurrentDashVelocity();
    }

    public float GetVelocity() //does not work
    {
        return cc.velocity.magnitude;
    }
    #endregion

    #region Other
    private void OnEnable()
    {
        cc.enabled = true;
    }

    private void OnDisable()
    {
        cc.enabled = false;
    }
    #endregion

}
