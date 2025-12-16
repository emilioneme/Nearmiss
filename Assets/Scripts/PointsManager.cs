using UnityEngine;
using UnityEngine.Events;

public class PointsManager : MonoBehaviour
{
    [SerializeField]
    public float totalPoints = 0;

    [SerializeField]
    public float distancePointsMultiplier = 10;

    [SerializeField]
    UnityEvent PassedPoints;

    public void DistancePoints(float value) 
    {
        totalPoints += value * distancePointsMultiplier;
        PassedPoints.Invoke();
    }


}
