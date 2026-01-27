using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "MovementData", menuName = "Scriptable Objects/MovementData")]
public class MovementData : ScriptableObject
{
    [HideInInspector]
    public CharacterController cc;

    [Header("Flight Forward")]
    [Range(1, 50)] public float flyingSpeed = 20f;
    [Range(1, 50)] public float totalFlyingSpeedMultiplier = 1.2f;

    [Header("NoseDive")]
    [Range(1, 50)] public float maxNosedieveSpeed = 10f;

    [Header("PointsSpeed")]
    public float maxPointsSpeed = 20;
    public float maxPointsForSpeed = 100000;
    public float pointIncreaseSpeed = 1;

    [Header("Sprint")]
    public float maxSprintSpeed = 1;
    public float sprintIncrease = 3;
    public float sprintDecrease = 5;

    [Header("Thrill")] //Thrill boost -> basically meant for when u get points
    public float thrillDuration = 1.5f;
    public float maxThrillSpeed = 1.1f;
    public float thrillIncrease = 10;
    public float thrillDecrease = 5;
    public float maxPointsForThrill = 500;

    [Header("Dash")]
    public float dashCooldwon = 1;
    public float dashSpeed = 15;
    public float dashDuration = .5f;

    [Header("Physics")]
    public bool allowDash = true;
    public bool allowThrill = false;
    public bool allowSprint = true;
    public bool allowPointsSpeed = false;

}
