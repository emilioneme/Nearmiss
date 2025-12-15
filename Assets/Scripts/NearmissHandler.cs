using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;

public class NearmissHandler : MonoBehaviour
{
    [Header("Rays")]
    [SerializeField]
    float rayDistance = 10;
    [SerializeField]
    int numberOfRays = 10;


    [SerializeField]
    int cylinderSteps = 4;
    [SerializeField]
    float cylinderStartOffset = -.5f;
    [SerializeField]
    float cylinderEndOffset = .5f;

    [Header("LayerMask")]
    [SerializeField]
    LayerMask layerMask;

    [Header("Gizmo")]
    [SerializeField]
    bool drawGizmos = false;


    [SerializeField]
    UnityEvent<float> Nearmissed;

    [SerializeField]
    UnityEvent NearmissSuccesfull;

    private void Update()
    {
        float minDistance = rayDistance;
        bool hitAtleastOnce = false;

        for (int c = 0; c <= cylinderSteps - 1; c++)
        {
            float height = (float)c / cylinderSteps;
            float heightStep = Mathf.Lerp(cylinderStartOffset, cylinderEndOffset, height);
            Vector3 rayOrigin = transform.position + transform.forward * heightStep;

            for (int a = 0; a < numberOfRays; a++)
            {
                float stepSize = 360 / numberOfRays;
                float eulerAngle = a * stepSize;
                Vector3 direction = transform.rotation * (Quaternion.AngleAxis(eulerAngle, Vector3.forward) * Vector3.right);
                Ray ray = new Ray(rayOrigin, direction.normalized);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit, rayDistance, layerMask))
                {
                    hitAtleastOnce = true;
                    if(hit.distance < minDistance) 
                    {
                        minDistance = hit.distance;
                    }
                }
            }
        }

        if(hitAtleastOnce) 
        {
            Nearmissed.Invoke(minDistance);
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos)
            return;

        Gizmos.color = Color.cyan;

        for (int c = 0; c <= cylinderSteps - 1; c++)
        {
            float height = (float)c / cylinderSteps;
            float heightStep = Mathf.Lerp(cylinderStartOffset, cylinderEndOffset, height);
            Vector3 rayOrigin = transform.position + transform.forward * heightStep;

            for (int a = 0; a < numberOfRays; a++)
            {
                float stepSize = 360 / numberOfRays;
                float eulerAngle = a * stepSize;
                Vector3 direction = transform.rotation * (Quaternion.AngleAxis(eulerAngle, Vector3.forward) * Vector3.right);
                Ray ray = new Ray(rayOrigin, direction.normalized);
                Vector3 endPoint = rayOrigin + direction.normalized * rayDistance;
                Gizmos.DrawLine(rayOrigin, endPoint);
            }
        }
    }


}   
