using System.Collections;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class PlayerModelHandler : MonoBehaviour
{
    [Header("Nearmis Particles")]
    [SerializeField]
    float nearmissEffectForwardMultiplier = 1;

    [Header("Trails")]
    [SerializeField]
    float trailFadeTimeMultiplier = 5;
    [SerializeField]
    float maxDroneSpeedForTrail = 300;

    [Header("TextIndicatorEffct")]
    [SerializeField]
    float indicatorForwardOffsett = 1;
    [SerializeField]
    float textIndicatorDistance = 1;

    [Header("TextParticleEffct")]
    [SerializeField]
    float particleForwardOffsett = 1;
    [SerializeField]
    float textParticleDistance = 1.2f;
    [SerializeField]
    float plusPointsMultiplier = 10;

    [Header("Camp")]
    [SerializeField]
    Camera TextEffectCamera;

    [Header("Other")]
    PlayerManager pm;
    [HideInInspector]
    public PlayerModelVisuals PlayerModelVisuals;

    GameObject TextParticle;
    GameObject TextIndicator;
    Coroutine runningPointRoutine;

    private void Awake()
    {
        pm = GetComponentInParent<PlayerManager>();

        PlayerModelVisuals = GetComponentInChildren<PlayerModelVisuals>();
        if (PlayerModelVisuals == null)
            Debug.LogWarning("No Player Model Visuals Found");
    }

    private void FixedUpdate()
    {
        if (pm.droneMovement == null)
            return;
        
        foreach(TrailRenderer trail in PlayerModelVisuals.TrailRenderers)
        {
            float velocityNomralized = pm.droneMovement.GetTotalVelocity().magnitude / maxDroneSpeedForTrail;
            trail.time = velocityNomralized * trailFadeTimeMultiplier;
        }

        if(TextIndicator != null) 
        {
            TextParticleEffect TextIndicatorEffect = TextIndicator.GetComponent<TextParticleEffect>();
            float fill = pm.pointManager.SecurePointsCooldown();
            fill = fill > .1f ? fill : 0;
            TextIndicatorEffect.SetImageFill(Mathf.Abs(pm.pointManager.SecurePointsCooldown() - 1));
        }
           
    }

    public void NeamissEffetSpawner(float normalDistance, float distance, Vector3 origin, RaycastHit hit) 
    {
        SpawnWallParticle(hit);

        SpawnTextIndicator(normalDistance, origin, hit);

        StartCoroutine(SpawnTextParticle(normalDistance, SpawnPosition(textParticleDistance, origin, hit)));
    }

    #region Point Indicators
    void SpawnTextIndicator(float normalDistance, Vector3 origin, RaycastHit hit)
    {
        Vector3 forwardOffset = (transform.forward * indicatorForwardOffsett);
        Vector3 position = SpawnPosition(textIndicatorDistance, origin, hit);

        if (TextIndicator == null)
            TextIndicator = Instantiate(PlayerModelVisuals.TextIndicatorEffect, position + forwardOffset, Quaternion.identity, pm.transform);

        //TextIndicator.transform.position = position;
        TextParticleEffect TextIndicatorEffect = TextIndicator.GetComponent<TextParticleEffect>();
        float runningPoints = pm.pointManager.runningPoints;
        TextIndicatorEffect.cam = TextEffectCamera;
        TextIndicatorEffect.SetText(eneme.Tools.ProcessFloat(runningPoints, 1));


        if(runningPointRoutine != null)
            DestroyCourutineSafely(ref runningPointRoutine);
        runningPointRoutine = StartCoroutine(DestroyEffct(TextIndicator, pm.pointManager.timeToSecurePoints));

    }

    IEnumerator SpawnTextParticle(float normalDistance, Vector3 position) 
    {
        if(TextParticle == null) 
        {
            Vector3 forwardOffset = (transform.forward * particleForwardOffsett);
            TextParticle = Instantiate(PlayerModelVisuals.TextParticleEffect, position + forwardOffset, Quaternion.identity, pm.transform);
            TextParticleEffect ParticleEffect = TextParticle.GetComponent<TextParticleEffect>();

            float plusPoints = Mathf.Abs(normalDistance - 1);
            float velocity = pm.droneMovement.GetTotalVelocity().magnitude;
            string text = eneme.Tools.ProcessFloat(plusPoints * velocity * plusPointsMultiplier, 1);
            ParticleEffect.SetText("+" + text);
            ParticleEffect.cam = TextEffectCamera;


            yield return new WaitForSeconds(.1f);
            ParticleEffect.rb.useGravity = true;
            yield return new WaitForSeconds(.2f);
            TextParticle = null;

            Destroy(TextParticle, 1f);
        }
    }
    void SpawnWallParticle(RaycastHit hit)
    {
        //WallEffects
        Destroy(Instantiate
            (
                PlayerModelVisuals.NearmissEffect,
                hit.point + (transform.forward * nearmissEffectForwardMultiplier),
                Quaternion.identity)
            , .5f);
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

    #region tools
    Vector3 SpawnPosition(float pointEffectDistance, Vector3 origin, RaycastHit hit)
    {
        Vector3 direction = (hit.point - origin).normalized;
        Vector3 projectedDirection =
            Vector3.ProjectOnPlane(direction, transform.forward);
        if (projectedDirection.sqrMagnitude < 0.0001f)
            projectedDirection = transform.right;
        projectedDirection.Normalize();
        float distance = Mathf.Max(pointEffectDistance, .5f);
        return transform.position + projectedDirection * distance;
    }
    IEnumerator DestroyEffct(GameObject effect, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(effect);
    }
    void DestroyCourutineSafely(ref Coroutine Routine)
    {
        if (Routine != null)
            StopCoroutine(Routine);
        Routine = null;
    }
    #endregion

    public void OnCrash() 
    {
        DestroyCourutineSafely(ref runningPointRoutine);
        if (TextIndicator != null)
            Destroy(TextIndicator);
        if (TextParticle != null)
            Destroy(TextParticle);

    }
}
