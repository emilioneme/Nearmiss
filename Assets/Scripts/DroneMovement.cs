using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.FilePathAttribute;

[RequireComponent(typeof(CharacterController))]
public class DroneMovement : MonoBehaviour
{
    CharacterController cc;
    [Header("Flight Forward")]
    [SerializeField]
    public float flyingSpeed = 20f;

    [Header("NoseDive")]
    [SerializeField]
    public float xRotationMultiplier = 20f;

    [SerializeField][Range(1, 50)]
    public float noseDiveSpeedMultiplier = 10f;

    [Header("Gravity")]
    [SerializeField]
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
        float noseDiveSpeed = NoseDiveSpeed() * xRotationMultiplier;
        return baseSpeed + noseDiveSpeed;
    }

    public float NoseDiveSpeed() 
    {
        float dist = Mathf.Abs(transform.rotation.eulerAngles.x + 90);
        float normalizedDist = Mathf.InverseLerp(0, 360, dist > 360 ? dist - 360 : dist);
        return normalizedDist; 
    }
    #endregion


    #region Gravity
    public void ApplyGravity()
    {
        cc.Move(Vector3.down * (CurrentDownSpeed() * Time.deltaTime));
    }

    public float CurrentDownSpeed()
    {
        float downSpeed = gravityForceSpeed() * gravityForceMultiplier;
        return downSpeed;
    }

    public float gravityForceSpeed() 
    {
        float rotation = transform.rotation.eulerAngles.z;
        rotation  = rotation > 180? rotation - 180 : rotation;

        float distFromFlat = Mathf.Abs(rotation - 90);
        float nomralizedDist = Mathf.InverseLerp(0, 90, distFromFlat);
        return 1 - nomralizedDist;
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
}
