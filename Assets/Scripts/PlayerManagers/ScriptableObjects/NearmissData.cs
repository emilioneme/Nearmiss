using UnityEngine;

[CreateAssetMenu(fileName = "NearmissData", menuName = "Scriptable Objects/NearmissData")]
public class NearmissData : ScriptableObject
{
    [SerializeField] public float rayDistance = 4;
    [SerializeField] public int startingNumberOfRays = 20;
    [SerializeField] public int shotsPerSecond = 20;
}
