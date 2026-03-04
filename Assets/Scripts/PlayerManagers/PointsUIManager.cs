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
    [SerializeField] GameObject HighScoreGO;

    [Header("Texts")]
    [SerializeField] TMP_Text TotalPointsText;
    [SerializeField] TMP_Text HighScoreText;

    [Header("Circles")]
    //[SerializeField] Image ComboNumImage;
    //[SerializeField] Image TotalPointsCircle;
    [SerializeField] AnimationCurve CircleFillCurve;

    Vector3 startHighScoreGOScale = Vector3.one;
    Vector3 startTotalScoreGOScale = Vector3.one;

    private void Start()
    {
        startHighScoreGOScale = HighScoreGO.transform.localScale;
        startTotalScoreGOScale = TotalPointsGO.transform.localScale;
    }

    #region Securing
    public void PointsSecured(float points)
    {
        TotalPointsGO.SetActive(true);
        TotalPointsText.text = Tools.ProcessFloat(points, 2);
        DOBounceTween(ref TotalPointsGO, startTotalScoreGOScale, .5f, .25f);
    }

    public void UpdatePersonalHighScore()
    {
        HighScoreText.text = Tools.ProcessFloat(UserData.Instance.personalHighScore, 1);
        DOBounceTween(ref HighScoreGO, startHighScoreGOScale, .5f, .25f);
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
        TotalPointsGO.SetActive(false);
        PointsCanvas.transform.localScale = Vector3.one;
        PointsCanvas.transform.DOKill();
        PointsCanvas.transform
            .DOScale(3, .25f)
            .OnComplete(() => PointsCanvas.SetActive(false));
        ResetText();
    }

    public void UnHidePointsUI()
    {
        TotalPointsGO.SetActive(true);
        PointsCanvas.SetActive(true);
        ResetText();

        PointsCanvas.transform.localScale = Vector3.zero;
        PointsCanvas.transform.DOKill();
        PointsCanvas.transform
            .DOScale(1, .25f);
    }

    public void ResetText() 
    {
        TotalPointsText.text = "0";
        TotalPointsGO.SetActive(false);
    }

    #region Tools
    public void DOBounceTween(ref GameObject GO, Vector3 fromScale, float toScale, float duration, Ease easeType = Ease.InOutSine)
    {
        GO.transform.DOKill();
        GO.transform.localScale = fromScale;
        GO.transform
            .DOScale(toScale, duration)
            .SetEase(easeType)
            .SetLoops(2, LoopType.Yoyo);
    }
    #endregion
}
