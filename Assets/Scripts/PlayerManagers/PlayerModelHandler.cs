using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PlayerModelHandler : MonoBehaviour
{
    [SerializeField]
    public GameObject PlayerModelPrefab;
    [SerializeField]
    public GameObject TextIndicatorPrefab;
    [SerializeField]
    public GameObject TextParticlePrefab;
    [HideInInspector]
    public GameObject PlayerModelGO;
    [HideInInspector]
    public PlayerModelContainer PlayerModelContainer;

    [Header("Wall Particles")]
    [SerializeField]
    float wallEffectForwardMultiplier = 1;
    [SerializeField]
    int maxWallParticles = 5;

    [Header("Trails")]
    [SerializeField]
    float trailFadeTimeMultiplier = 5;
    [SerializeField]
    float maxDroneSpeedForTrail = 300;

    [Header("TextIndicatorEffct")]
    [SerializeField]
    float textIndicatorDistance = 1;

    [Header("SecureTextIndicatorEffct")]
    [SerializeField]
    float securePointsEffectDuration = 1;
    [SerializeField]
    Vector3 secureLerpToPosition;
    [SerializeField]
    AnimationCurve secureLerpCurve;

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

    [Header("Cam")]
    [SerializeField]
    Camera PlayerCamera;
    [SerializeField]
    Transform Pivot;

    [Header("Event")]
    public UnityEvent<GameObject> SpawnedCrashObject;

    [Header("Other")]
    Queue<GameObject> WallParticleGOs = new Queue<GameObject>();
    Queue<GameObject> TextParticleGOs = new Queue<GameObject>();
    GameObject TextIndicatorGO = null;
    GameObject TextSecuredGO = null;
    TextIndicatorEffect TextIndicatorEffectGO;
    Coroutine RunRoutine;

    float lastTextParticle = 0;

    #region Instantiate
    private void Awake()
    {
        InitiatePlayerModel();
    }
    public void SetPlayerModelVisual(GameObject newPlayerModelPrefab)
    {
        if(PlayerModelGO != null)
            Destroy(PlayerModelGO);

        PlayerModelPrefab = newPlayerModelPrefab;
        InitiatePlayerModel();
    }
    void InitiatePlayerModel() 
    {
        if (PlayerModelGO == null)
            PlayerModelGO = Instantiate(PlayerModelPrefab, transform);

        PlayerModelContainer = PlayerModelGO.GetComponent<PlayerModelContainer>();
        

        TextParticleGOs = new Queue<GameObject>();
        WallParticleGOs = new Queue<GameObject>();
    }

    private void FixedUpdate()
    {
        if (PlayerModelContainer) 
        {
            foreach (TrailRenderer trail in PlayerModelContainer.TrailRenderers)
            {
                float velocityNomralized = UserData.Instance.droneVelocity.magnitude / maxDroneSpeedForTrail;
                trail.time = velocityNomralized * trailFadeTimeMultiplier;
            }
        }
    }
    #endregion

    #region  Run Start
    public void RunStarted(float timeToSecure)
    {
        if (!TextIndicatorGO)
            return;
        DestroyCourutineSafely(ref RunRoutine);
        RunRoutine = StartCoroutine(RunCooldownCoroutine(timeToSecure));
    }

    public void RunContinued(float timeToSecure)
    {
        DestroyCourutineSafely(ref RunRoutine);
        RunRoutine = StartCoroutine(RunCooldownCoroutine(timeToSecure));
    }

    IEnumerator RunCooldownCoroutine(float timeToSecure)
    {
        yield return new WaitForSeconds(timeToSecure);

        DestroyGO(ref TextSecuredGO);
        TextSecuredGO = TextIndicatorGO;
        TextSecuredGO.transform.SetParent(PlayerCamera.transform, true);
        TextIndicatorGO = null;
        float timer = 0f;
        Vector3 startLocal = TextSecuredGO.transform.localPosition;
        Vector3 targetWorld = PlayerCamera.ViewportToWorldPoint(secureLerpToPosition);
        Vector3 targetLocal = PlayerCamera.transform.InverseTransformPoint(targetWorld);

        while (timer < securePointsEffectDuration)
        {
            timer += Time.deltaTime;
            float normalized = timer / securePointsEffectDuration;
            float t = secureLerpCurve.Evaluate(normalized);

            targetWorld = PlayerCamera.ViewportToWorldPoint(secureLerpToPosition);
            targetLocal = PlayerCamera.transform.InverseTransformPoint(targetWorld);

            TextSecuredGO.transform.localPosition = Vector3.Lerp(startLocal, targetLocal, t);
            yield return null;
        }

        DestroyGO(ref TextSecuredGO);
    }

    public void UpdateRunPoints(float points)
    {
        if(TextIndicatorEffectGO != null)
            TextIndicatorEffectGO.SetText(eneme.Tools.ProcessFloat(points, 2));
    }
    #endregion

    #region TexctParticles
    public void SpawnTextParticle(float normalDistance, Vector3 position)
    {
        lastTextParticle = Time.time;

        Vector3 forwardOffset = transform.forward * particleForwardOffsett;
        Vector3 pos = position + forwardOffset;

        float points = (1 + Mathf.Abs(normalDistance - 1)) * UserData.Instance.droneVelocity.magnitude * plusPointsMultiplier;
        string text = "+" + eneme.Tools.ProcessFloat(points, 1);

        GameObject TextParticleGO;
        
        if (TextParticleGOs.Count >= maxTextParticles)
        {
            TextParticleGO = TextParticleGOs.Dequeue();
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

        TextParticleGOs.Enqueue(TextParticleGO);
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

    #region WallParticles
    void SpawnWallParticle(RaycastHit hit)
    {
        Vector3 pos = hit.point + (Pivot.transform.forward * wallEffectForwardMultiplier);

        GameObject WallParticleGO;
        if (WallParticleGOs.Count >= maxWallParticles)
        {
            WallParticleGO = WallParticleGOs.Dequeue();
            WallParticleGO.SetActive(false);
        }
        else
        {
            WallParticleGO = Instantiate
            (
                PlayerModelContainer.NearmissEffect,
                pos,
                Quaternion.identity
            );
        }

        WallParticleGO.SetActive(true);
        WallParticleGO.transform.position = pos;
        WallParticleGO.transform.rotation = Quaternion.identity;

        WallParticleGOs.Enqueue(WallParticleGO);
    }
    #endregion

    #region TextIndicator
    void SpawnTextIndicator(Vector3 position)
    {
        TextIndicatorGO = Instantiate(TextIndicatorPrefab, position, Quaternion.identity, Pivot.transform);
        TextIndicatorEffectGO = TextIndicatorGO.GetComponent<TextIndicatorEffect>();
        TextIndicatorEffectGO.cam = PlayerCamera;
        TextIndicatorEffectGO.SetText(" ");
    }
    #endregion

    #region Dash
    Coroutine dashSpinRoutine;
    public void AnimateDash(Vector3 direction, Vector3 animationAxis, float duration)
    {
        if (dashSpinRoutine != null) StopCoroutine(dashSpinRoutine);
        dashSpinRoutine = StartCoroutine(AnimateDashCoroutine(direction, animationAxis, duration));
    }

    IEnumerator AnimateDashCoroutine(Vector3 direction, Vector3 animationAxis, float duration)
    {
        Quaternion start = transform.localRotation;
        float timer = 0f;
        while (timer < duration)
        {
            float t = timer / duration;
            float angle = 360f * t * Mathf.Sign(direction.magnitude);
            transform.localRotation = start * Quaternion.Euler(animationAxis.x * angle, animationAxis.y * angle, animationAxis.z * angle);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        dashSpinRoutine = null;
    }
    #endregion

    #region Tools
    Vector3 projectedDirection(float pointEffectDistance, Vector3 origin, RaycastHit hit)
    {
        Vector3 direction = (hit.point - origin).normalized;
        Vector3 projectedDirection =
            Vector3.ProjectOnPlane(direction, transform.forward);
        if (projectedDirection.sqrMagnitude < 0.0001f)
            projectedDirection = transform.right;

        projectedDirection.Normalize();

        return (projectedDirection * pointEffectDistance);
    }
    void DestroyCourutineSafely(ref Coroutine Routine)
    {
        if (Routine != null)
            StopCoroutine(Routine);
        Routine = null;
    }
    #endregion

    public void NeamissEffetcSpawner(float normalDistance, int numberOfHits, Vector3 origin, RaycastHit hit)
    {
        if (Time.time - lastTextParticle > textParticleCooldown)
            SpawnWallParticle(hit);
        if(Time.time - lastTextParticle > textParticleCooldown)
            SpawnTextParticle(normalDistance, transform.position + projectedDirection(textParticleDistance, origin, hit));

        if(TextIndicatorGO == null) 
        {
            SpawnTextIndicator(transform.position + projectedDirection(textIndicatorDistance, origin, hit));
        }
    }
    
    public void DestroyAllEffectsGO()
    {
        DestroyPool(ref TextParticleGOs);
        DestroyPool(ref WallParticleGOs);

        DestroyGO(ref TextSecuredGO);
        TextIndicatorEffectGO = null;
        DestroyGO(ref TextIndicatorGO);

        DestroyCourutineSafely(ref RunRoutine);
    }

    public void DestroyPool(ref Queue<GameObject> GOs) 
    {
        foreach (GameObject go in GOs)
        {
            if (go != null)
                Destroy(go);
        }
        GOs.Clear();
        GOs = new Queue<GameObject>();
    }
    public void DestroyGO(ref GameObject GO) 
    {
        if (GO != null)
            Destroy(GO);
        GO = null;
    }

    void SpawnCrashModel()
    {
        GameObject CrashObject = Instantiate
                    (PlayerModelContainer.CrashModelPrefab, this.transform.position, Quaternion.identity);
        SpawnedCrashObject.Invoke(CrashObject);
        Destroy(CrashObject, 10f);
    }

    public void OnCrash() 
    {
        SpawnCrashModel();
        transform.localRotation = Quaternion.Euler(Vector3.zero); //fixing its rotation before spawnign in cse player dashes and dies
        DestroyAllEffectsGO();
    }

}
