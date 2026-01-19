using System.Collections.Generic;
using UnityEngine;

public class WallParticleHandler : MonoBehaviour
{
    [Header("Wall Particles")]
    [SerializeField]
    float wallParticleCooldown = 1;
    [SerializeField]
    float wallEffectForwardMultiplier = 1;
    [SerializeField]
    int maxWallParticles = 5;

    [SerializeField]
    public GameObject WallParticlePrefab;

    [SerializeField]
    Transform Pivot;
    [SerializeField]
    List<GameObject> WallParticleList = new List<GameObject>();
    Queue<GameObject> WallParticleQueue = new Queue<GameObject>();
    private float lastWallParticle;


    private void Awake()
    {
        foreach (GameObject obj in WallParticleList) 
        {
            WallParticleQueue.Enqueue(obj);
        }
    }
    public void NeamissEffetcSpawner(float normalDistance, int numberOfHits, Vector3 origin, RaycastHit hit)
    {
        if (Time.time - lastWallParticle > wallParticleCooldown)
            SpawnWallParticle(hit);
    }
    void SpawnWallParticle(RaycastHit hit)
    {
        Vector3 pos = hit.point + (Pivot.transform.forward * wallEffectForwardMultiplier);

        GameObject WallParticleGO;
        if (WallParticleQueue.Count >= maxWallParticles)
        {
            WallParticleGO = WallParticleQueue.Dequeue();
            WallParticleGO.SetActive(false);
        }
        else
        {
            WallParticleGO = Instantiate
            (
                WallParticlePrefab,
                pos,
                Quaternion.identity
            );
        }

        WallParticleGO.SetActive(true);
        WallParticleGO.transform.position = pos;
        WallParticleGO.transform.rotation = Quaternion.identity;

        WallParticleQueue.Enqueue(WallParticleGO);
    }

    public void DisableAllParticles()
    {
        foreach (GameObject t in WallParticleQueue)
        {
            t.SetActive(false);
        }
    }
}
