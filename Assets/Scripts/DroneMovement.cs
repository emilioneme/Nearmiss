using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.FilePathAttribute;

[RequireComponent(typeof(CharacterController))]
public class DroneMovement : MonoBehaviour
{
    CharacterController cc;
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
    [HideInInspector]
    public bool lookRotation;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    #region FlyForward
    public void Fly() 
    {
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
    //int diection -1 for left, 1 for right
    public void Rotate(int direction)
    {
        float rotationAmount = rotationSpeed * rotationSpeedMultiplier * Time.deltaTime;
        transform.rotation = transform.rotation * Quaternion.Euler(0, 0, rotationAmount * direction);
    }
    #endregion

    #region Look
    public void LookUpDown(float y)
    {
        float yClamped = Mathf.Clamp(y, -yRotationInputThershhold, yRotationInputThershhold);
        float loookAmount = -yClamped * lookSpeed * lookSpeedMultiplier * Time.deltaTime;
        transform.rotation = transform.rotation * Quaternion.Euler(loookAmount, 0, 0);
    }
    public void LookLeftRight(float x)
    {
        float xClamped = Mathf.Clamp(x, -xRotationInputThershhold, xRotationInputThershhold);
        float loookAmount = xClamped * lookSpeed * lookSpeedMultiplier * Time.deltaTime;
        transform.rotation = transform.rotation * Quaternion.Euler(0, loookAmount, 0);
        if(x > 0 && lookRotation) 
            Rotate(-1);
        else if (x < 0 && lookRotation)
            Rotate(1);
    }
    #endregion

    public float GetVelocity() 
    {
        return cc.velocity.sqrMagnitude;
    }
}
