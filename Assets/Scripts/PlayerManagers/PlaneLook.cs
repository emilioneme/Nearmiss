using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Windows;

public class PlaneLook : MonoBehaviour
{

    [Header("Look")]
    [SerializeField]
    public float lookSpeedMultiplier = 1.0f;
    float lookSpeed = 1;

    [Header("Turning Rotation")]
    [SerializeField]
    public float rotationSpeedMultiplier = 1.0f;
    float rotationSpeed = 1;
    [SerializeField]
    public float manuverSpeedMultiplier = 1.0f;
    float manuverSpeed = 1;
    [SerializeField]
    float yRotationInputThershhold = 3;
    [SerializeField]
    float xRotationInputThershhold = 3;

    public bool allowLook = true;
    public bool allowLookRotate = true;
    public bool allowRotate = true;

    #region Look
    public void Look(Vector2 input) 
    {
        LookUpDown(input.y);
        LookLeftRight(input.x);
    }

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
        Quaternion prevRotation = transform.rotation;
        Quaternion toRotation = transform.rotation * Quaternion.Euler(0, loookAmount, 0);
        transform.rotation = transform.rotation * Quaternion.Euler(0, loookAmount, 0);
        Manuver(xClamped);
    }

    public void Manuver(float magnitude)
    {
        if (!allowLookRotate)
            return;
        float rotationAmount = manuverSpeed * manuverSpeedMultiplier * -magnitude * Time.deltaTime;
        transform.rotation = transform.rotation * Quaternion.Euler(0, 0, rotationAmount);
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
    
}
