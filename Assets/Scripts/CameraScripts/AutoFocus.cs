using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AutoFocus : MonoBehaviour
{
    public Transform focusTarget;
    public Volume volume;

    private DepthOfField dof;

    void Start()
    {
        if (volume.profile.TryGet(out DepthOfField tmp))
        {
            dof = tmp;
        }
    }

    void Update()
    {
        if (dof == null || focusTarget == null)
            return;

        float distance =
            Vector3.Distance(transform.position, focusTarget.position);

        dof.focusDistance.value = distance;
    }
}