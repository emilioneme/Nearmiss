using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NearmissHandler : MonoBehaviour
{
    [Header("Rays")]
    [SerializeField]
    public float rayDistance = 10;
    [SerializeField]
    int numberOfRays = 10;

    [Header("Ray Timing")]
    [SerializeField]
    int shotsPerSecond = 10;
    float nextShotTime;

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

    void OnEnable()
    {
        nextShotTime = Time.time;
    }


    private void Update()
    {
        float interval = 1f / shotsPerSecond;

        while (Time.time >= nextShotTime)
        {
            ShootAllRays();

            nextShotTime += interval;
            if (Time.time - nextShotTime > 1f) // 1f for 1 second 
                nextShotTime = Time.time + interval;
        }
    }

    void ShootAllRays() 
    {
        minDistance = rayDistance;
        hitAtleastOnce = false;
        Vector3 origin = transform.position;
        ShootRaySpehere(origin);
        if (hitAtleastOnce)
            NearmissEvent.Invoke(minDistance / rayDistance, minDistance, origin, hitPoint); //1 = as close as it can get, 0, is not close at all
    }

    #region RayShooting
    void ShootRaySpehere(Vector3 rayOrigin) 
    {
        for (int i = 0; i < numberOfRays; i++)
        {
            Vector3 dir = FibonacciSphereDirection(i, numberOfRays);
            var ray = Physics.Raycast(rayOrigin, dir, out RaycastHit hit, rayDistance, layerMask, QueryTriggerInteraction.Ignore);
            if (ray)
            {
                hitAtleastOnce = true;
                if (hit.distance < minDistance)
                {
                    minDistance = hit.distance;
                    hitPoint = hit;
                    Debug.DrawLine(rayOrigin, rayOrigin + dir, Color.red);
                }
            }
            else
            {
                //idk
            }
        }
    }

    static Vector3 FibonacciSphereDirection(int i, int n)
    {
        float offset = 2f / n;
        float y = ((i * offset) - 1f) + (offset / 2f);
        float r = Mathf.Sqrt(1f - y * y);

        float goldenAngle = Mathf.PI * (3f - Mathf.Sqrt(5f));
        float phi = i * goldenAngle;

        float x = Mathf.Cos(phi) * r;
        float z = Mathf.Sin(phi) * r;

        return new Vector3(x, y, z);
    }
    #endregion

    private void OnDrawGizmos()
    {
        if (drawGizmos) 
        {
            Vector3 origin = transform.position;
            for (int i = 0; i < numberOfRays; i++)
            {
                Vector3 dir = FibonacciSphereDirection(i, numberOfRays);
                if (Physics.Raycast(origin, dir, out RaycastHit hit, rayDistance, layerMask, QueryTriggerInteraction.Ignore))
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(origin, hit.point);
                }
                else
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawRay(origin, dir * rayDistance);
                }
            }
        }
    }
}

