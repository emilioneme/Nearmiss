using UnityEngine;

public class PlayerModelContainer : MonoBehaviour
{
    [Header("Model")]
    public GameObject Model;
    public GameObject CrashModelPrefab;
    [Header("Trails")]
    public TrailRenderer[] TrailRenderers;
}
