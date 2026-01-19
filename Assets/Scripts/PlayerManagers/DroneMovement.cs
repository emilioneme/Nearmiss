using System.Collections;
using Unity.VisualScripting;
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
    UnityEvent<Vector2, float> DashStarted; //direction, duration

    [Header("Physics")]
    public bool applyGravity = true;
    public bool enableFlying = true;
    public bool allowDash = true;


    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        MoveDrone();
    }

    #region Movement
    public void MoveDrone() 
    {
        if (!cc)
            return;
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

    #region Dash
    public Coroutine DashRutine;
    public void Dash(Vector2 direction)  
    {
        if(!allowDash)
            return;
        if(eneme.Tools.CooldownSince(lastTimeDashed, dashCooldwon) < 1) 
            return;

        lastTimeDashed = Time.time;
        dashDirection = transform.rotation * direction.normalized; //set it wihtin class scope

        if(DashRutine == null)
            DashRutine = StartCoroutine(DashCoroutine()); //takes direction from the classes scope

        DashStarted.Invoke(direction, dashDuration);
    }

    IEnumerator DashCoroutine()
    {
        float timer = 0f;
        while (timer < dashDuration)
        {
            float t = timer / dashDuration;
            timer += Time.deltaTime;

            dashCurrentSpeed = dashSpeed * dashSpeedOverTime.Evaluate(t);

            if(cc)
                cc.Move(GetDashVelocity() * Time.deltaTime);
            yield return null;
        }
        DashRutine = null;
    }

    public void ResetDash()
    {
        DestroyCourutineSafely(ref DashRutine);
    }

    public Vector3 GetDashVelocity() // so we can add it to total velocity
    {
        if (DashRutine == null)
            return Vector3.zero;
        return dashDirection * dashCurrentSpeed;
    }

    #endregion

    #region Total Velocity
    Vector3 GetTotalVelocity()
    {
        Vector3 vel = CurrentDownVelocity() + GetForwardVelocity() + GetDashVelocity();
        float speed = vel.magnitude;
        float avgT = 1f - Mathf.Exp(-UserData.Instance.averageAdaptSpeed * Time.deltaTime); 
        UserData.Instance.avgVelocity = Mathf.Lerp(UserData.Instance.avgVelocity, speed, avgT);
        UserData.Instance.deltaVelocity = speed - UserData.Instance.avgVelocity;
        UserData.Instance.droneVelocity = vel;
        return vel;
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
            DestroyCourutineSafely(ref pointsCoroutine);

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
        DestroyCourutineSafely(ref DashRutine);
    }

    private void OnDisable()
    {
        DestroyCourutineSafely(ref DashRutine);
        cc.enabled = false;
    }
    #endregion

    void DestroyCourutineSafely(ref Coroutine Routine)
    {
        if (Routine != null)
            StopCoroutine(Routine);
        Routine = null;
    }


}
