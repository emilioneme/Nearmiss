using eneme;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Windows;

public class PlaneLook : MonoBehaviour
{
    [SerializeField] float maxYawDegPerSec = 360f;
    [SerializeField] float maxPitchDegPerSec = 240f;

    [Header("Look")]
    [SerializeField]
    public float lookSpeedMultiplier = 1.0f;
    float lookSpeed = 1;
    Vector2 currentLook;

    [Header("Manuvers")]
    public float manuverRotationSpeedMultiplier = 1.0f;
    [SerializeField] public float manuverSpeedMultiplier = 1.0f;
    float manuverSpeed = 1;

    [Header("Turning Rotation")]
    [SerializeField] public float rotationSpeedMultiplier = 1.0f;
    float rotationSpeed = 1;
   
    [Header("Other")]
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
        if (!allowLook) return;
        float pitchDegPerSec = (-y) * lookSpeed * lookSpeedMultiplier;
        pitchDegPerSec = Mathf.Clamp(pitchDegPerSec, -maxPitchDegPerSec, maxPitchDegPerSec);

        float step = pitchDegPerSec * Time.deltaTime;
        transform.rotation *= Quaternion.Euler(step, 0f, 0f);
    }

    public void LookLeftRight(float x)
    {
        if (!allowLook) return;
        float yawDegPerSec = x * lookSpeed * lookSpeedMultiplier;
        yawDegPerSec = Mathf.Clamp(yawDegPerSec, -maxYawDegPerSec, maxYawDegPerSec);
        float step = yawDegPerSec * Time.deltaTime;
        Quaternion prevRotation = transform.rotation;
        transform.rotation *= Quaternion.Euler(0f, step, 0f);

        float yawDelta = Quaternion.Angle(prevRotation, transform.rotation); // degrees this frame (positive)
        float signedDelta = yawDelta * Mathf.Sign(yawDegPerSec);              // keep direction

        Manuver(signedDelta * manuverRotationSpeedMultiplier);
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
        float rotationAmount = manuverSpeed * manuverSpeedMultiplier * -direction * Time.deltaTime;
        transform.rotation = transform.rotation * Quaternion.Euler(0, 0, rotationAmount);
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
