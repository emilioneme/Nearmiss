using DG.Tweening;
using eneme;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PointsUIManager : MonoBehaviour
{
    [SerializeField] GameObject PointsCanvas;
    [SerializeField] GameObject ComboGO;
    [SerializeField] GameObject TotalPointsGO;

    [Header("Texts")]
    [SerializeField] TMP_Text TotalPointsText;
    [SerializeField] TMP_Text ComboNumText;

    [Header("Circles")]
    [SerializeField] Image ComboNumImage;
    [SerializeField] Image TotalPointsCircle;
    [SerializeField] AnimationCurve CircleFillCurve;

    Coroutine RunRoutine;

    #region  Run 
    public void RunStarted(float timeToSecure)
    {
        this.StopSafely(ref RunRoutine);
        RunRoutine = StartCoroutine(RunCooldownCoroutine(timeToSecure));
        ComboNumText.text = " ";
        if(!ComboGO.activeSelf)
            ComboGO.SetActive(true);
        DOBounceTween(ref ComboGO, 1.3f, .25f);
    }

    public void RunContinued(float timeToSecure)
    {
        this.StopSafely(ref RunRoutine);
        RunRoutine = StartCoroutine(RunCooldownCoroutine(timeToSecure));
    }
    #endregion

    #region Securing
    public void PointsSecured(float points)
    {
        TotalPointsText.text = Tools.ProcessFloat(points, 2);
        DOBounceTween(ref TotalPointsGO, .5f, .25f);
        ComboGO.transform
            .DOScale(0f, .25f)
            .OnComplete(() => ComboGO.SetActive(false));
    }
    #endregion

    #region RunCooldown
    IEnumerator RunCooldownCoroutine(float timeToSecure)
    {
        float timeLapsed = 0;
        float secureNormalized = 0;
        float fill = 0;
        while (timeLapsed < timeToSecure)
        {
            timeLapsed += Time.deltaTime;
            secureNormalized = timeLapsed / timeToSecure;
            fill = secureNormalized < .1f ? 0 : secureNormalized;
            ComboNumImage.fillAmount = CircleFillCurve.Evaluate(Mathf.Abs(fill - 1));
            yield return null;
        }
        RunRoutine = null;
    }
    #endregion

    #region Combos
    public void UpdateComboMult(float comboMult)
    {
        if (comboMult > 1)
        {
            ComboNumText.text = "x" + Tools.LimitNumberLength(comboMult, 4);
            DOBounceTween(ref ComboGO, .5f, .25f);
            DOBounceTween(ref TotalPointsGO, .9f, .25f);
        }
        else
        {
            ComboNumText.text = " ";
        }
    }
    #endregion

    #region Totalpoints
    public void UpdateTotalPoints(float points)
    {
        if (points == 0) 
        {
            TotalPointsText.text = " ";
        }
        else 
        {
            TotalPointsCircle.gameObject.SetActive(true);
            TotalPointsText.text = Tools.ProcessFloat(points, 2);
            TotalPointsCircle.fillAmount = points / 1000;
        }
            
    }
    #endregion

    public void HidePointsUI() 
    {
        PointsCanvas.transform.localScale = Vector3.one;
        PointsCanvas.transform.DOKill();
        PointsCanvas.transform
            .DOScale(3, .25f)
            .OnComplete(() => PointsCanvas.SetActive(false));
        ResetText();
    }

    public void UnHidePointsUI()
    {
        PointsCanvas.SetActive(true);
        ResetText();

        PointsCanvas.transform.localScale = Vector3.zero;
        PointsCanvas.transform.DOKill();
        PointsCanvas.transform
            .DOScale(1, .25f);
    }

    public void ResetText() 
    {
        ComboNumText.text = " ";
        TotalPointsText.text = " ";
        TotalPointsCircle.gameObject.SetActive(false);
        ComboGO.SetActive(false);
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
