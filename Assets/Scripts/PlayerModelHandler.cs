using System.Collections;
using UnityEngine;

public class PlayerModelHandler : MonoBehaviour
{
    [SerializeField]
    float nearmissEffectForwardMultiplier = 1;
    [SerializeField]
    float trailFadeTimeMultiplier = 5;

    [SerializeField]
    float maxDroneSpeedForTrail = 300;


    PlayerManager playerManager;
    [HideInInspector]
    public PlayerModelVisuals PlayerModelVisuals;

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
        Destroy(Instantiate
            (
                PlayerModelVisuals.NearmissEffect,
                hit.point  + (transform.forward * nearmissEffectForwardMultiplier),
                Quaternion.identity)
            ,1f);
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
