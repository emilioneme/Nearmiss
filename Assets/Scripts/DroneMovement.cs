using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class DroneMovement : MonoBehaviour
{
    CharacterController cc;
    [Header("Flight Forward")]
    [SerializeField]
    public float flyingSpeed = 20f;

    [Header("NoseDive")]
    [SerializeField]
    public float noseDiveSpeedMultiplier = 20f;


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

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }


    public void Fly() 
    {
        cc.Move(transform.forward * (CurrentSpeed() * Time.deltaTime));
    }

    public float CurrentSpeed() 
    {
        float baseSpeed = flyingSpeed;
        float noseDiveSpeed = NoseDiveSpeed() * noseDiveSpeedMultiplier;
        return baseSpeed + noseDiveSpeed;
    }

    public float NoseDiveSpeed() 
    {
        float noseDiveAngle = -.5f;
        float distanceFromAngle = Mathf.Abs(noseDiveAngle - transform.rotation.x);
        return distanceFromAngle;
    }


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
        if(x > 0) 
        {
            Rotate(-1);
        }
        else if (x < 0)
        {
            Rotate(1);
        }
    }
    #endregion
}
