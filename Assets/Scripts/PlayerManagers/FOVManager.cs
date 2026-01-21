using Unity.Cinemachine;
using UnityEngine;

public class FOVManager : MonoBehaviour
{
    [Header("FOV")]
    [SerializeField] float baseFov = 60f;
    [SerializeField] float degreesPerSpeed = 1;
    [SerializeField] float maxFovOffset = 20f;
    [SerializeField] float fovSmoothSpeed = 8f;
    float currentFov;


    [SerializeField] CinemachineCamera cam;

    //public void UpdateFOV()
    //{
    //  float speedFOV = minFov + Mathf.InverseLerp(minVelocityFOV, maxVelocityFOV, UserData.Instance.droneVelocity.magnitude) * maxFovAdditive;
    //  cam.Lens.FieldOfView = speedFOV;
    // }

    public void UpdateFOV()
    {
        float targetOffset = Mathf.Clamp(UserData.Instance.deltaVelocity * degreesPerSpeed, -maxFovOffset, maxFovOffset);
        float targetFov = baseFov + targetOffset;
        float fovT = 1f - Mathf.Exp(-fovSmoothSpeed * Time.deltaTime);
        currentFov = Mathf.Lerp(currentFov == 0 ? baseFov : currentFov, targetFov, fovT);
        cam.Lens.FieldOfView = currentFov;
    }
}
