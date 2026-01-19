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
    [HideInInspector]
    public GameObject PlayerModelGO;
    [HideInInspector]
    public PlayerModelContainer PlayerModelContainer;

    [Header("Dash Amimation")]
    [SerializeField]
    int numberOfFlips = 1;
    [SerializeField]
    AnimationCurve dashRotationSpeed;

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

    [Header("Cam")]
    [SerializeField]
    Camera PlayerCamera;
    [SerializeField]
    Transform Pivot;

    [Header("Event")]
    public UnityEvent<GameObject> SpawnedCrashObject;

    [Header("Other")]
    GameObject TextIndicatorGO = null;
    GameObject TextSecuredGO = null;
    TextIndicatorEffect TextIndicatorEffectGO;
    Coroutine RunRoutine;

    #region Instantiate
    public void SetPlayerModelVisual(GameObject newPlayerModelPrefab)
    {
        if(PlayerModelGO != null)
            Destroy(PlayerModelGO);

        PlayerModelPrefab = newPlayerModelPrefab;
        InitiatePlayerModel();
    }

    public void InitiatePlayerModel() 
    {
        if (PlayerModelGO == null)
            PlayerModelGO = Instantiate(PlayerModelPrefab, transform);

        PlayerModelContainer = PlayerModelGO.GetComponent<PlayerModelContainer>();
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
    public void AnimateDash(Vector2 direction, float duration)
    {
        if (dashSpinRoutine != null) StopCoroutine(dashSpinRoutine);
        dashSpinRoutine = StartCoroutine(AnimateDashCoroutine(direction, duration));
    }

    IEnumerator AnimateDashCoroutine(Vector3 direction, float duration)
    {
        Quaternion start = transform.localRotation;
        float timer = 0f;
        while (timer < duration)
        {
            float t = timer / duration;
            float angle = 360f * numberOfFlips * t;

            Quaternion rotationForX = Quaternion.identity;
            Quaternion rotationForY = Quaternion.identity;
            if(direction.x != 0) 
                rotationForX = Quaternion.Euler(0, 0, angle * -Mathf.Sign(direction.x));
            else
                rotationForX = Quaternion.Euler(0, 0, angle * -Mathf.Sign(direction.x));

            transform.localRotation = start * rotationForX * rotationForY;

            timer += Time.deltaTime;
            yield return null;
        }
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        dashSpinRoutine = null;
    }
    #endregion

    #region Tools
    void DestroyCourutineSafely(ref Coroutine Routine)
    {
        if (Routine != null)
            StopCoroutine(Routine);
        Routine = null;
    }
    #endregion

    public void NeamissEffetcSpawner(float normalDistance, int numberOfHits, Vector3 origin, RaycastHit hit)
    {
        if(TextIndicatorGO == null) 
        {
            SpawnTextIndicator(transform.position + eneme.Tools.projectedDirection(textIndicatorDistance, transform, origin, hit));
        }
    }
    
    public void DestroyAllEffectsGO()
    {
        //DestroyPool(ref WallParticleGOs);

        DestroyGO(ref TextSecuredGO);
        TextIndicatorEffectGO = null;

        DestroyGO(ref TextIndicatorGO);

        DestroyCourutineSafely(ref RunRoutine);
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

    public void ResetPlayerModel() 
    {
        SpawnCrashModel();
        transform.localRotation = Quaternion.Euler(Vector3.zero); //fixing its rotation before spawnign in cse player dashes and dies
        DestroyAllEffectsGO();
    }

}
