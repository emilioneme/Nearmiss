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
    
    RaycastHit hitPoint;

    [SerializeField]
    UnityEvent<float, int, Vector3, RaycastHit> NearmissEvent; //distance normalized, total distance, number of hits  playerPosition, hit 

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
        float minDistance = rayDistance;
        int numberOfHits = 0;
        Vector3 origin = transform.position;
        ShootRaySpehere(origin, ref minDistance, ref numberOfHits);
        float normalizedDistance = Mathf.Abs(minDistance / rayDistance) - 1;
        if (numberOfHits > 0)
            NearmissEvent.Invoke(normalizedDistance, numberOfHits, origin, hitPoint); //1 = as close as it can get, 0, is not close at all
    }

    #region RayShooting
    void ShootRaySpehere(Vector3 rayOrigin, ref float minDistance, ref int numberOfHits) 
    {
        for (int i = 0; i < numberOfRays; i++)
        {
            Vector3 dir = FibonacciSphereDirection(i, numberOfRays);
            var ray = Physics.Raycast(rayOrigin, dir, out RaycastHit hit, rayDistance, layerMask, QueryTriggerInteraction.Ignore);
            if (ray) 
            {
                numberOfHits++;
                if (hit.distance < minDistance)
                {
                    minDistance = hit.distance;
                    hitPoint = hit;
                    Debug.DrawLine(rayOrigin, rayOrigin + dir, Color.red);
                }
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

