using Unity.Cinemachine;
using UnityEngine;

public class PlayerCameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineBrain brain;
    [SerializeField] private CinemachineCamera mainCam;
    [SerializeField] private CinemachineCamera crashCam;

    [SerializeField] private int mainPriority = 10;
    [SerializeField] private int crashPriority = 20;

    [Header("Blend Settings")]
    [SerializeField] private float crashBlendTime = 0.15f;
    [SerializeField] private float respawnBlendTime = 0.5f;

    void Awake()
    {
        mainCam.Priority = mainPriority;
        crashCam.Priority = 0;
    }

    public void ActivateCrashCamera()
    {
        brain.DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Styles.EaseIn,
            crashBlendTime
        );

        crashCam.Priority = crashPriority;
    }

    public void ActivateMainCamera()
    {
        brain.DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Styles.EaseIn,
            respawnBlendTime
        );

        crashCam.Priority = 0;
    }
}
