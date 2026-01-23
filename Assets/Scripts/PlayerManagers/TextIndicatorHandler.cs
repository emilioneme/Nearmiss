using eneme;
using System.Collections;
using UnityEngine;

public class TextIndicatorHandler : MonoBehaviour
{
    [SerializeField]
    public GameObject TextIndicatorPrefab;

    [Header("Cam")]
    [SerializeField]
    Camera PlayerCamera;
    [SerializeField]
    Transform Pivot;

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

    [Header("Other")]
    [SerializeField]
    GameObject TextIndicatorGO = null;
    GameObject TextSecuredGO = null;
    TextIndicatorEffect TextIndicatorEffect;

    Coroutine RunRoutine;

    private void Awake()
    {
        TextIndicatorEffect = TextIndicatorGO.GetComponent<TextIndicatorEffect>();
        TextIndicatorEffect.cam = PlayerCamera;
        TextIndicatorEffect.SetText(" ");
    }

    public void NeamissEffetcSpawner(float normalDistance, int numberOfHits, Vector3 origin, RaycastHit hit)
    {
        if (!TextIndicatorGO.activeSelf)
            SetTextIndicator(transform.position + eneme.Tools.projectedDirection(textIndicatorDistance, transform, origin, hit));
    }

    void SetTextIndicator(Vector3 position)
    {
        TextIndicatorGO.transform.position = position;
        TextIndicatorGO.SetActive(true);
    }

    #region  Run Start
    public void RunStarted(float timeToSecure)
    {
        if (!TextIndicatorGO)
            return;
        this.StopSafely(ref RunRoutine);
        RunRoutine = StartCoroutine(RunCooldownCoroutine(timeToSecure));
    }

    public void RunContinued(float timeToSecure)
    {
        this.StopSafely(ref RunRoutine);
        RunRoutine = StartCoroutine(RunCooldownCoroutine(timeToSecure));
    }

    IEnumerator RunCooldownCoroutine(float timeToSecure)
    {
        yield return new WaitForSeconds(timeToSecure);

        Transform textTransform = TextIndicatorGO.transform;
        TextSecuredGO = Instantiate(TextIndicatorGO, textTransform.position, textTransform.rotation, textTransform.parent);
        Destroy(TextSecuredGO, securePointsEffectDuration + .01f);

        TextSecuredGO.transform.SetParent(PlayerCamera.transform, true);

        TextIndicatorGO.SetActive(false);


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
            if(TextSecuredGO)
                TextSecuredGO.transform.localPosition = Vector3.Lerp(startLocal, targetLocal, t);
            yield return null;
        }
        DisableText();
    }

    public void UpdateRunPoints(float points)
    {
        if (TextIndicatorEffect != null)
            TextIndicatorEffect.SetText(eneme.Tools.ProcessFloat(points, 2));
    }
    #endregion

    public void DisableText()
    {
        if (TextSecuredGO)
            Destroy(TextSecuredGO);

        TextIndicatorGO.SetActive(false);
        this.StopSafely(ref RunRoutine);
    }
}
