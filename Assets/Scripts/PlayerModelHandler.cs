using System.Collections;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;
using static UnityEngine.UI.Image;

public class PlayerModelHandler : MonoBehaviour
{
    [SerializeField]
    float nearmissEffectForwardMultiplier = 1;
    [SerializeField]
    float trailFadeTimeMultiplier = 5;

    [SerializeField]
    float maxDroneSpeedForTrail = 300;

    [SerializeField]
    Camera TextEffectCamera;

    PlayerManager playerManager;
    [HideInInspector]
    public PlayerModelVisuals PlayerModelVisuals;

    GameObject TextParticle;

    private void Awake()
    {
        playerManager = GetComponentInParent<PlayerManager>();

        PlayerModelVisuals = GetComponentInChildren<PlayerModelVisuals>();
        if (PlayerModelVisuals == null)
            Debug.LogWarning("No Player Model Visuals Found");
    }

    private void FixedUpdate()
    {
        if (playerManager.droneMovement == null)
            return;
        
        foreach(TrailRenderer trail in PlayerModelVisuals.TrailRenderers)
        {
            float velocityNomralized = playerManager.droneMovement.GetTotalVelocity().magnitude / maxDroneSpeedForTrail;
            trail.time = velocityNomralized * trailFadeTimeMultiplier;
        }
    }

    public void NeamissEffetSpawner(float normalDistance, float distance, Vector3 origin, RaycastHit hit) 
    {
        //WallEffects
        Destroy(Instantiate
            (
                PlayerModelVisuals.NearmissEffect,
                hit.point  + (transform.forward * nearmissEffectForwardMultiplier),
                Quaternion.identity)
            ,1f);

        StartCoroutine(SpawnTextParticle(normalDistance, distance, origin, hit));
    }

    IEnumerator SpawnTextParticle(float normalDistance, float distance, Vector3 origin, RaycastHit hit) 
    {
        if (TextParticle == null)
        {
            Vector3 direction = (hit.point - origin).normalized;
            Vector3 projecteDirection = direction.ProjectOntoPlane(transform.forward);

            Vector3 position = this.transform.position + projecteDirection;
            TextParticle = Instantiate(PlayerModelVisuals.TextParticleEffect, position, Quaternion.identity, playerManager.transform);

            TextParticleEffect ParticleEffect = TextParticle.GetComponent<TextParticleEffect>();
            ParticleEffect.SetText("+" + eneme.Tools.ProcessFloat(Mathf.Abs(normalDistance - 1) * 100, 1));
            ParticleEffect.cam = TextEffectCamera;

            Destroy(TextParticle, .5f);
            yield return new WaitForSeconds(.10f);
            TextParticle = null;
            ParticleEffect.rb.useGravity = true;
            yield return new WaitForSeconds(.10f);
        }
    }

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
}
