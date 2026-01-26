using DG.Tweening;
using eneme;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PointsUIManager : MonoBehaviour
{
    [SerializeField] GameObject PointsCanvas;
    //[SerializeField] GameObject ComboGO;
    [SerializeField] GameObject TotalPointsGO;

    [Header("Texts")]
    [SerializeField] TMP_Text TotalPointsText;
    //[SerializeField] TMP_Text ComboNumText;

    [Header("Circles")]
    //[SerializeField] Image ComboNumImage;
    //[SerializeField] Image TotalPointsCircle;
    [SerializeField] AnimationCurve CircleFillCurve;

    #region Securing
    public void PointsSecured(float points)
    {
        TotalPointsText.text = Tools.ProcessFloat(points, 2);
        DOBounceTween(ref TotalPointsGO, .5f, .25f);
    }
    #endregion

    #region Totalpoints
    public void UpdateTotalPoints(float points)
    {
        if (points < 1) 
        {
            TotalPointsText.text = " ";
        }
        else 
        {
            TotalPointsText.text = Tools.ProcessFloat(points, 2);
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
        TotalPointsText.text = " ";
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
