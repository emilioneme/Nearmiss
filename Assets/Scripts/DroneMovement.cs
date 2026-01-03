using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController))]
public class DroneMovement : MonoBehaviour
{
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

    [Header("PointsSpeed")]
    [SerializeField]
    float pointsSpeedMultiplier = 1;
    public float maxPointsForSpeed = 60;
    public AnimationCurve pointsSpeedCurve;

    float totalPointsNormalized = 0;

    [SerializeField]
    float pointIncreaseSpeed = 1;


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

    [Header("Physics")]
    public bool applyGravity = true;
    public bool enableFlying = true;

    [Header("Other")]
    public bool allowLook = true;
    public bool allowRotate = true;
    public bool allowLookRotate = true;
    public bool allowDash = true;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    #region Movement
    public void MoveDrone() 
    {
        cc.Move(GetTotalVelocity() * Time.deltaTime);
    }
    #endregion

    #region FlyForward
    public Vector3 GetForwardVelocity()
    {
        return enableFlying? transform.forward * CurrentForwardSpeed(): Vector3.zero;
    }

    float CurrentForwardSpeed() 
    {
        return ((flyingSpeed + NoseDiveSpeed()) * PointsSpeed()) * totalFlyingSpeedMultiplier;
    }

    float NoseDiveSpeed() 
    {
        float dot = Vector3.Dot(transform.forward, Vector3.down);
        return Mathf.InverseLerp(-1f, 1f, dot) * noseDiveSpeedMultiplier;
    }

    float PointsSpeed() 
    {
        return 1 + pointsSpeedCurve.Evaluate(totalPointsNormalized) * pointsSpeedMultiplier;
    }
    #endregion

    #region Gravity
    Vector3 CurrentDownVelocity()
    {
        return applyGravity? Vector3.down * CurrentDownSpeed() : Vector3.zero;
    }

    float CurrentDownSpeed()
    {
        float downSpeed = GravityForceSpeed() * gravityForceMultiplier;
        return downSpeed;
    }

    float GravityForceSpeed() 
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
        if(!allowLookRotate)
            return;
        float rotationAmount = rotationSpeed * rotationSpeedMultiplier * magnitude * Time.deltaTime;
        transform.rotation = transform.rotation * Quaternion.Euler(0, 0, rotationAmount * direction);
    }

    public void RotateLeft()
    {
        if (!allowRotate)
            return;
        Rotate(1);
    }

    public void RotateRight()
    {
        if (!allowRotate)
            return;
        Rotate(-1);
    }
    #endregion

    #region Look
    public void LookUpDown(float y)
    {
        if (!allowLook)
            return;
        float yClamped = Mathf.Clamp(y, -yRotationInputThershhold, yRotationInputThershhold);
        float loookAmount = -yClamped * lookSpeed * lookSpeedMultiplier * Time.deltaTime;
        transform.rotation = transform.rotation * Quaternion.Euler(loookAmount, 0, 0);
    }
    public void LookLeftRight(float x)
    {
        if (!allowLook)
            return;
        float xClamped = Mathf.Clamp(x, -xRotationInputThershhold, xRotationInputThershhold);
        float loookAmount = xClamped * lookSpeed * lookSpeedMultiplier * Time.deltaTime;
        transform.rotation = transform.rotation * Quaternion.Euler(0, loookAmount, 0);
        if (x > 0)
            Manuver(x, -1);//Rotate(-1);
        else if (x < 0)
            Manuver(x, -1);//Rotate(-1);
    }
    #endregion

    #region Dash
    public void Dash(Vector3 direction, Vector3 animateAxis)  
    {
        if(!allowDash)
            return;
        if(eneme.Tools.CooldownSince(lastTimeDashed, dashCooldwon) < 1) 
            return;
        lastTimeDashed = Time.time;
        dashDirection = transform.rotation * direction; //set it wihtin class scope
        StartCoroutine(DashCoroutine()); //takes direction from the classes scope
        DashStarted.Invoke(direction, animateAxis, dashDuration);
    }

    IEnumerator DashCoroutine()
    {
        isDashing = true;
        allowLook = false;
        float timer = 0f;
        while (timer < dashDuration)
        {
            //float t = Mathf.Abs((timer / dashDuration) - 1);
            float t = timer / dashDuration;
            dashCurrentSpeed = dashSpeed * dashSpeedOverTime.Evaluate(t);
            cc.Move(GetDashVelocity() * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
        isDashing = false;
        allowLook = true;
    }

    public Vector3 GetDashVelocity() // so we can add it to total velocity
    {
        if (!isDashing)
            return Vector3.zero;
        return dashDirection * dashCurrentSpeed;
    }

    #endregion

    #region Total Velocity
    public Vector3 GetTotalVelocity()
    {
        return CurrentDownVelocity() + GetForwardVelocity() + GetDashVelocity();
    }
    #endregion

    #region Points Speed

    public void ResetedPoints(float totalPoints) 
    {
        totalPointsNormalized = totalPoints / maxPointsForSpeed;
    }

    Coroutine pointsCoroutine;
    public void PointsChanged(float totalPoints)
    {
        float targetPoints = Mathf.Clamp01(totalPoints / maxPointsForSpeed);

        if (pointsCoroutine != null)
            StopCoroutine(pointsCoroutine);

        pointsCoroutine = StartCoroutine(LerpSpeedChange(targetPoints));
    }
    IEnumerator LerpSpeedChange(float targetPoints)
    {
        while (!Mathf.Approximately(totalPointsNormalized, targetPoints))
        {
            totalPointsNormalized = Mathf.MoveTowards(
                totalPointsNormalized,
                targetPoints,
                pointIncreaseSpeed * Time.deltaTime
            );

            yield return null;
        }

        totalPointsNormalized = targetPoints;
        pointsCoroutine = null;
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
