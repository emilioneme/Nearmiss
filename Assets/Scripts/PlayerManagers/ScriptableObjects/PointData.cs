using UnityEngine;

[CreateAssetMenu(fileName = "PointData", menuName = "Scriptable Objects/PointData")]
public class PointData : ScriptableObject
{
    [Header("Points")]
    public float startTotalPoints = 0;
    public float startRunningPoints = 0;
    public float startExpectedPoints = 0;
    public float startComboMultiplier = 1;

    [Header("Point Calculation")]
    public float runningPointsMultiplier = .1f;
    public float speedPointMultiplier = .1f;
    public float maxDistancePoints = 1;

    [Header("Combo Calculation")]
    public float maxSwerveCombo = 2;
    public float maxSkimCombo = 2;

    [Header("Time For")] //time to secure point is minTimeBeforeCombo + comboWindowDuratio
    public float scureTimeDecrease = .25f;
    public float minSecureTime = 1;
    public float maxSecureTime = 2;
}
