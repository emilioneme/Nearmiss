using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.FilePathAttribute;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Rigidbody))]
public class DroneMovement : MonoBehaviour
{

    [HideInInspector]
    public CharacterController cc;
    [HideInInspector]
    public Rigidbody rb;


    [Header("Flight Forward")]
    [SerializeField][Range(1, 50)]
    public float flyingSpeed = 20f;

    [Header("NoseDive")]
    [SerializeField][Range(1, 50)]
    public float noseDiveSpeedMultiplier = 10f;

    [Header("Gravity")]
    [SerializeField][Range(1, 50)]
    float gravityForceMultiplier = 1.9f;


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

    [Header("Settings Toggles")]
    [SerializeField]
    public bool flying;
    public bool rotating, looking, lookRotation, gravity, dashing = true;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
    }


    #region FlyForward
    public void Fly() 
    {
        if (!flying)
            return;
        cc.Move(transform.forward * (CurrentForwardSpeed() * Time.deltaTime));
    }

    public float CurrentForwardSpeed() 
    {
        float baseSpeed = flyingSpeed;
        float noseDiveSpeed = NoseDiveSpeed() * noseDiveSpeedMultiplier;
        return baseSpeed + noseDiveSpeed;
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
        if(!gravity)
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
        if (!rotating)
            return;
        Rotate(1);
    }

    public void RotateRight()
    {
        if (!rotating)
            return;
        Rotate(-1);
    }
    #endregion

    #region Look
    public void LookUpDown(float y)
    {
        if (!looking)
            return;
        float yClamped = Mathf.Clamp(y, -yRotationInputThershhold, yRotationInputThershhold);
        float loookAmount = -yClamped * lookSpeed * lookSpeedMultiplier * Time.deltaTime;
        transform.rotation = transform.rotation * Quaternion.Euler(loookAmount, 0, 0);
    }
    public void LookLeftRight(float x)
    {
        if (!looking)
            return;
        float xClamped = Mathf.Clamp(x, -xRotationInputThershhold, xRotationInputThershhold);
        float loookAmount = xClamped * lookSpeed * lookSpeedMultiplier * Time.deltaTime;
        transform.rotation = transform.rotation * Quaternion.Euler(0, loookAmount, 0);
        if(x > 0 && lookRotation) 
            Rotate(-1);
        else if (x < 0 && lookRotation)
            Rotate(1);
    }
    #endregion

    #region Dash
    public void DashLeft() 
    {
        if(dashing) 
            return;
    }

    public void DashRight()
    {
        if (dashing)
            return;
    }

    #endregion
    public float GetVelocity() 
    {
        return cc.velocity.sqrMagnitude;
    }

    private void OnEnable()
    {
        cc.enabled = true;
    }

    private void OnDisable()
    {
        cc.enabled = false;
    }

    public void DisableAllMovement() 
    {
        rotating = false;
        rotating = false;
        looking = false;
        lookRotation = false;
        gravity = false;
        dashing = false;
    }

    public void EnableAllMovement()
    {
        rotating = true;
        rotating = true;
        looking = true;
        lookRotation = true;
        gravity = true;
        dashing = true;
    }
}
