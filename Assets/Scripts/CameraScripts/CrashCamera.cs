using Unity.Cinemachine;
using UnityEngine;

public class CrashCamera : MonoBehaviour
{
    [SerializeField]
    CinemachineCamera cam;

    public void SetTarget(GameObject target)
    {
        cam.Follow = target.transform;
        cam.LookAt = target.transform;
    }
}
