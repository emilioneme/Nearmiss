using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class DroneMovement : MonoBehaviour
{
    CharacterController cc;
    [Header("Flight Forward")]
    [SerializeField]
    public float flyingSpeedMultiplier = 1.0f;
    float flyingSpeed = 1.5f;
    float flyingSpeedMax = 2;
    float flyingSpeedMin = 1;

    [Header("Acceleration")]
    [SerializeField]
    float deaccelerationSpeedMultiplier = 1f;
    [SerializeField]
    float accelerationSpeedMultiplier = 1f;

    float deaccelerationSpeed = .1f;
    float accelerationSpeed = .1f;

    [Header("Rotation")]
    [SerializeField]
    public float rotationSpeedMultiplier = 1.0f;
    float rotationSpeed = 1;

    [Header("Look")]
    [SerializeField]
    public float lookSpeedMultiplier = 1.0f;
    float lookSpeed = 1;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    public void FlyForward() 
    {
        float currentSpeed = flyingSpeed * flyingSpeedMultiplier * Time.deltaTime;
        cc.Move(transform.forward * currentSpeed);
        //rb.MovePosition(rb.position + transform.forward * currentSpeed);
    }
    public void Deaccelerate() 
    {
        float deaccelerateAmount = deaccelerationSpeed * deaccelerationSpeedMultiplier * Time.deltaTime;
        flyingSpeed = Mathf.Clamp(
            flyingSpeed - deaccelerateAmount,
            flyingSpeedMin,
            flyingSpeedMax);
    }
    public void Accelerate()
    {
        float accelerateAmount = accelerationSpeed * accelerationSpeedMultiplier * Time.deltaTime;
        flyingSpeed = Mathf.Clamp(flyingSpeed + accelerateAmount,
            flyingSpeedMin, flyingSpeedMax);
    }
    public void RotateLeft()
    {
        float rotationAmount = rotationSpeed * rotationSpeedMultiplier * Time.deltaTime;
        transform.rotation = transform.rotation * Quaternion.Euler(0, 0, rotationAmount);
    }
    public void RotateRight()
    {
        float rotationAmount = rotationSpeed * rotationSpeedMultiplier * Time.deltaTime;
        transform.rotation = transform.rotation * Quaternion.Euler(0, 0, -rotationAmount);
    }
    public void LookUpDown(float y)
    {
        float loookAmount = -y * lookSpeed * lookSpeedMultiplier * Time.deltaTime;
        transform.rotation = transform.rotation * Quaternion.Euler(loookAmount, 0, 0);
    }
    public void LookLeftRight(float x)
    {
        float loookAmount = x * lookSpeed * lookSpeedMultiplier * Time.deltaTime;
        transform.rotation = transform.rotation * Quaternion.Euler(0, loookAmount, 0);
    }

}
