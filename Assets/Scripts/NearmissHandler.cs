using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;

public class NearmissHandler : MonoBehaviour
{
    [Header("Rays")]
    [SerializeField]
    public bool on = true;
    [SerializeField]
    public float rayDistance = 10;
    [SerializeField]
    int numberOfRays = 10;

    [Header("LayerMask")]
    [SerializeField]
    LayerMask layerMask;

    [Header("Gizmo")]
    [SerializeField]
    bool drawGizmos = false;

    float minDistance;
    bool hitAtleastOnce;
    RaycastHit hitPoint;


    [SerializeField]
    UnityEvent<float, float, Vector3, RaycastHit> NearmissEvent; //distance normalized, distance, playerPosition, hit 

    private void FixedUpdate()
    {
        if (on)
            ShootAllRays();
    }

    void ShootAllRays() 
    {
        minDistance = rayDistance;
        hitAtleastOnce = false;
        ShootRayCircle(transform.position, Vector3.forward, Vector3.right);
        ShootRayCircle(transform.position, Vector3.right, Vector3.forward);
        ShootRayCircle(transform.position, Vector3.up, Vector3.right);
        if (hitAtleastOnce)
            NearmissEvent.Invoke(minDistance / rayDistance, minDistance, transform.position, hitPoint); //1 = as close as it can get, 0, is not close at all
    }

    #region RayShooting
    void ShootRayCircle(Vector3 rayOrigin, Vector3 onAxis, Vector3 toAxis) 
    {
        for (int a = 0; a <= numberOfRays; a++)
        {
            float stepSize = 360 / numberOfRays;
            float eulerAngle = a * stepSize;
            Vector3 direction = transform.rotation * (Quaternion.AngleAxis(eulerAngle, onAxis) * toAxis);
            Ray ray = new Ray(rayOrigin, direction.normalized);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, rayDistance, layerMask))
            {
                hitAtleastOnce = true;
                if (hit.distance < minDistance)
                {
                    Debug.DrawLine(rayOrigin, hit.point, Color.black);
                    minDistance = hit.distance;
                    hitPoint = hit;
                }
            }
        }
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        if (!drawGizmos)
            return;
        
        Gizmos.color = Color.cyan;
        ShootRayCircleGizmo(transform.position, Vector3.forward, Vector3.right);
        Gizmos.color = Color.blue;
        ShootRayCircleGizmo(transform.position, Vector3.right, Vector3.forward);
        Gizmos.color = Color.red;
        ShootRayCircleGizmo(transform.position, Vector3.up, Vector3.right);
    }

    void ShootRayCircleGizmo(Vector3 rayOrigin, Vector3 onAxis, Vector3 toAxis)
    {
        for (int a = 0; a <= numberOfRays; a++)
        {
            float stepSize = 360 / numberOfRays;
            float eulerAngle = a * stepSize;
            Vector3 direction = transform.rotation * (Quaternion.AngleAxis(eulerAngle, onAxis) * toAxis);
            Vector3 endPoint = rayOrigin + direction.normalized * rayDistance;
            Gizmos.DrawLine(rayOrigin, endPoint);
        }
    }
    #endregion
}
