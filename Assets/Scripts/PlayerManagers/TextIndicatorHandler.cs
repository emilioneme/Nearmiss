using DG.Tweening;
using eneme;
using System.Collections;
using TMPro;
using UnityEngine;

public class TextIndicatorHandler : MonoBehaviour
{
    [SerializeField] public GameObject TextIndicatorPrefab;

    [SerializeField] public GameObject ComboTextPivot;

    [SerializeField]
    TMP_Text RunText;

    [SerializeField]
    TMP_Text ComboText;

    [Header("Cam")]
    [SerializeField]
    Camera PlayerCamera;
    [SerializeField]
    Transform Pivot;

 
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

    Coroutine SecureAnimationRoutine;

    private void Awake()
    {
        TextIndicatorEffect = TextIndicatorGO.GetComponent<TextIndicatorEffect>();
        TextIndicatorEffect.cam = PlayerCamera;
        RunText.text = " ";
        ComboText.text = " ";
    }

    #region  Run Start
    public void RunStarted(float timeToSecure)
    {
        TextIndicatorGO.SetActive(true);
        DOBounceTween(ref TextIndicatorGO, 1.3f, .25f);
    }

    #region SecureAnimation
    public void StartPointsSecureAnimation()
    {
        //Swap
        TextIndicatorGO.SetActive(false);
        ComboText.text = " ";
        Transform textTransform = TextIndicatorGO.transform;

        this.StopSafely(ref SecureAnimationRoutine);
        SecureAnimationRoutine = StartCoroutine(SecureAnimationCoroutine(textTransform));
    }

    IEnumerator SecureAnimationCoroutine(Transform textTransform)
    {
        TextSecuredGO = Instantiate(TextIndicatorGO, textTransform.position, textTransform.rotation, textTransform.parent);
        TextSecuredGO.transform.SetParent(PlayerCamera.transform, true);
        TextSecuredGO.SetActive(true);

        //STart Effect
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
            if (TextSecuredGO)
                TextSecuredGO.transform.localPosition = Vector3.Lerp(startLocal, targetLocal, t);
            yield return null;
        }
        DisableText();
        Destroy(TextSecuredGO);
        SecureAnimationRoutine = null;
    }
    #endregion
    

    public void UpdateRunPoints(float points)
    {
        RunText.text = eneme.Tools.ProcessFloat(points, 2);
        TextIndicatorGO.SetActive(true);
    }
    #endregion

    #region Combos
    public void UpdateComboMult(float comboMult)
    {
        if (comboMult > 1)
        {
            ComboText.text = "x" + Tools.LimitNumberLength(comboMult, 4);
            DOBounceTween(ref ComboTextPivot, 1.5f, .25f);
        }
        else
        {
            ComboText.text = " ";
        }
    }
    #endregion

    public void DisableText()
    {
        if (TextSecuredGO)
            Destroy(TextSecuredGO);

        TextIndicatorGO.SetActive(false);
        this.StopSafely(ref SecureAnimationRoutine);
    }

    #region Tools
    public void DOBounceTween(ref GameObject GO, float toScale, float duration, Ease easeType = Ease.InOutSine)
    {
        GO.transform.DOKill();
        GO.transform.localScale = Vector3.one;
        GO.transform
            .DOScale(toScale, duration)
            .SetEase(easeType)
            .SetLoops(2, LoopType.Yoyo);
    }
    #endregion
}
