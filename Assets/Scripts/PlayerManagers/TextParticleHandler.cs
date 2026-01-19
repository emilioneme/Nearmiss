using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextParticleHandler : MonoBehaviour
{
    [SerializeField]
    public GameObject TextParticlePrefab;

    [Header("TextParticleEffct")]
    [SerializeField]
    float particleForwardOffsett = 1;
    [SerializeField]
    float textParticleDistance = 1.2f;
    [SerializeField]
    float plusPointsMultiplier = 10;
    [SerializeField]
    [Range(0f, 1f)]
    float textParticleCooldown = .2f;
    [SerializeField]
    [Range(0f, 1f)]
    float timeBeforeGavityOff = 1f;
    [SerializeField]
    int maxTextParticles = 5;

    Queue<GameObject> TextParticleQueue = new Queue<GameObject>();
    [SerializeField]
    List<GameObject> TextParticleList = new List<GameObject>();
    float lastTextParticle = 0;

    [Header("Cam")]
    [SerializeField]
    Camera PlayerCamera;
    [SerializeField]
    Transform Pivot;

    private void Awake()
    {
        foreach (GameObject go in TextParticleList) 
        {
            TextParticleQueue.Enqueue(go);
        }
    }

    public void NeamissEffetcSpawner(float normalDistance, int numberOfHits, Vector3 origin, RaycastHit hit)
    {
        if (Time.time - lastTextParticle > textParticleCooldown)
            SpawnTextParticle(normalDistance, transform.position + eneme.Tools.projectedDirection(textParticleDistance, Pivot, origin, hit));
    }

    #region TexctParticles
    public void SpawnTextParticle(float normalDistance, Vector3 position)
    {
        lastTextParticle = Time.time;

        Vector3 forwardOffset = transform.forward * particleForwardOffsett;
        Vector3 pos = position + forwardOffset;

        float points = (1 + Mathf.Abs(normalDistance - 1)) * UserData.Instance.droneVelocity.magnitude * plusPointsMultiplier;
        string text = "+" + eneme.Tools.ProcessFloat(points, 1);

        GameObject TextParticleGO;

        if (TextParticleQueue.Count >= maxTextParticles)
        {
            TextParticleGO = TextParticleQueue.Dequeue();
            TextParticleGO.SetActive(false);
        }
        else
        {
            TextParticleGO = Instantiate(
            TextParticlePrefab,
            Pivot.transform
            );
        }

        // Reset + reuse
        TextParticleEffect ParticleEffect =
            TextParticleGO.GetComponent<TextParticleEffect>();
        ParticleEffect.cam = PlayerCamera;
        ParticleEffect.rb.useGravity = false;
        ParticleEffect.SetText(text); // whatever you already use
        ParticleEffect.rb.linearVelocity = Vector3.zero;
        ParticleEffect.rb.angularVelocity = Vector3.zero;

        TextParticleGO.transform.position = pos;
        TextParticleGO.transform.rotation = Quaternion.identity;
        TextParticleGO.SetActive(true);


        // stop old life coroutine for this particle
        if (ParticleEffect.lifeRoutine != null)
        {
            StopCoroutine(ParticleEffect.lifeRoutine);
            ParticleEffect.lifeRoutine = null;
        }

        // start new life coroutine
        ParticleEffect.lifeRoutine = StartCoroutine(
            SpawnTextParticleCoroutine(TextParticleGO, ParticleEffect)
        );

        TextParticleQueue.Enqueue(TextParticleGO);
    }


    IEnumerator SpawnTextParticleCoroutine(GameObject TextParticleGO, TextParticleEffect ParticleEffect)
    {
        yield return new WaitForSeconds(timeBeforeGavityOff);
        ParticleEffect.rb.useGravity = true;
        yield return new WaitForSeconds(.5f);
        ParticleEffect.rb.useGravity = false;
        TextParticleGO.SetActive(false);
    }
    #endregion

    public void DisableAllParticles() 
    {
        foreach(GameObject t in TextParticleQueue) 
        {
            t.SetActive(false);
        }
    }
}
