using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class DroneMovement : MonoBehaviour
{
    CharacterController cc;
    [Header("Flight Forward")]
    [SerializeField]
    public float flyingSpeedMultiplier = 100f;
    

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
    public void FlyForward() 
    {
        float currentSpeed =  flyingSpeedMultiplier * Time.deltaTime;
        cc.Move(transform.forward * currentSpeed);
        //Debug.Log(cc.velocity.sqrMagnitude);
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
