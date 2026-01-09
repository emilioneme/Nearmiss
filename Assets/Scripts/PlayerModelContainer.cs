using UnityEngine;

public class PlayerModelContainer : MonoBehaviour
{
    [Header("Model")]
    public GameObject Model;
    public GameObject CrashModelPrefab;
    [Header("Trails")]
    public TrailRenderer[] TrailRenderers;
    [Header("Nearmis")]
    public GameObject NearmissEffect;
    [Header("PointsTextParticle")]
    public GameObject TextParticleEffect;
    [Header("PointsTextIndicastor")]
    public GameObject TextIndicatorEffect;
}
