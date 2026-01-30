using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "DroneData", menuName = "Scriptable Objects/DroneData")]
public class DroneData : ScriptableObject
{
    public MovementData MovementData;
    public PointData PointData;
    public NearmissData NearmissData;

    public GameObject Mesh;
    public GameObject Container;

    public string DroneName; 

}
