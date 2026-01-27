using NUnit.Framework;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

public class NearmissHandler : MonoBehaviour
{
    [SerializeField] NearmissData nearmissData;

    [Header("Rays")]
    [SerializeField]
    public float rayDistance = 10;
    [SerializeField]
    int startingNumberOfRays = 10;
    Vector3[] rayDirections;

    [Header("Ray Timing")]
    [SerializeField]
    int shotsPerSecond = 10;
    float nextShotTime;
    float interval = 0;

    [Header("LayerMask")]
    [SerializeField]
    LayerMask layerMask;

    [Header("Gizmo")]
    [SerializeField]
    bool drawGizmos = false;
    
    RaycastHit hitPoint;

    [SerializeField]
    UnityEvent<float, int, Vector3, RaycastHit> NearmissEvent; //distance normalized, total distance, number of hits  playerPosition, hit 

    private void Awake()
    {
        if (nearmissData == null) return;
        SetStartData(nearmissData);
    }

    public void SetStartData(NearmissData nearmissData) 
    {
        rayDistance = nearmissData.rayDistance;
        startingNumberOfRays = nearmissData.startingNumberOfRays;
        shotsPerSecond = nearmissData.shotsPerSecond;
    }

    private void Start()
    {
        interval = 1f / shotsPerSecond;
        SetNumberOfRays(startingNumberOfRays);
    }

    void OnEnable()
    {
        nextShotTime = Time.time;
    }

    public void SetShotsPerSecond(int amount) 
    {
        shotsPerSecond = amount;
        interval = 1 / shotsPerSecond;
    }

    public void SetNumberOfRays(int amount) 
    {
        CalculateRayDirections(amount);
    }

    void CalculateRayDirections(int numberOfRays)
    {
        rayDirections = new Vector3[numberOfRays];

        for (int i = 0; i < numberOfRays; i++)
        {
            Vector3 dir = FibonacciSphereDirection(i, numberOfRays);
            rayDirections[i] = dir;
        }
    }

    private void Update()
    {
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
        float normalizedDistance = 0;

        Vector3 origin = transform.position;

        ShootRaySpehere(origin, ref minDistance, ref numberOfHits, ref normalizedDistance);

        if (numberOfHits > 0) //if hit
            NearmissEvent.Invoke(normalizedDistance, numberOfHits, origin, hitPoint); //1 = as close as it can get, 0, is not close at all
    }

    #region RayShooting
    void ShootRaySpehere(Vector3 rayOrigin, ref float minDistance, ref int numberOfHits, ref float normalizedDistance) 
    {
        for (int i = 0; i < rayDirections.Length; i++)
        {
            Vector3 dir = rayDirections[i];
            var ray = Physics.Raycast(rayOrigin, transform.rotation * dir, out RaycastHit hit, rayDistance, layerMask, QueryTriggerInteraction.Ignore);
            if (ray) 
            {
                numberOfHits++;
                if (hit.distance < minDistance)
                {
                    hitPoint = hit;
                    minDistance = hit.distance;
                    normalizedDistance = Mathf.Abs(minDistance / rayDistance) - 1;
                    //NearmissEvent.Invoke(normalizedDistance, numberOfHits, rayOrigin, hitPoint); // this is very frame expensive
                }

                Debug.DrawLine(rayOrigin, hit.point, Color.red, .02f);
            }
            else if(drawGizmos)
            {
                Vector3 end = rayOrigin + (transform.rotation * dir) * rayDistance;
                Debug.DrawLine(rayOrigin, end, Color.blue, .02f);
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
    
}

