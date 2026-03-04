using eneme;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

public class MenuManager : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] GameObject CharacterSelectionGO;
    [SerializeField] RectTransform CharacterSelectionRT;

    [Header("HighScore")]
    [SerializeField] GameObject HighScoreGO;
    [SerializeField] TMP_Text HighScoreText;

    Vector3 startHighScoreScale;

    private void Start()
    {
        if(CharacterSelectionGO.activeInHierarchy)
            CharacterSelectionGO.SetActive(false);

        startHighScoreScale = HighScoreGO.transform.localScale;
        HighScoreText.text = Tools.ProcessFloat(UserData.Instance.personalHighScore, 2);
    }

    public void HighScoreBounce() 
    {
        DOBounceTween(ref HighScoreGO, startHighScoreScale, .5f, .25f);
    }

    public void OpenCharacterSelection()
    {
        CharacterSelectionGO.SetActive(true);
        float canvasWidth = CharacterSelectionRT.rect.width;

        CharacterSelectionRT.anchoredPosition = new Vector2(canvasWidth, 0);
        CharacterSelectionRT.DOAnchorPos(Vector2.zero, .25f).SetEase(Ease.OutCubic);
    }

    public void CloseCrashCanvas()
    {
        float canvasWidth = CharacterSelectionRT.rect.width;
        CharacterSelectionRT.anchoredPosition = Vector2.zero;
        CharacterSelectionRT.DOAnchorPos(new Vector2(-canvasWidth, 0), .25f).SetEase(Ease.OutCubic)
           .OnComplete(() => CharacterSelectionGO.SetActive(false));
    }

    public void GoToScene(string sceneName) 
    {
        SceneLoader.Instance.LoadScene(sceneName);
    }

    #region Settings
    public void OpenSettings()
    {
        SettingsManager.Instance.OpenSettings();
    }

    public void CloseSettings()
    {
        SettingsManager.Instance.OpenSettings();
    }
    #endregion

    public void DOBounceTween(ref GameObject GO, Vector3 fromScale, float toScale, float duration, Ease easeType = Ease.InOutSine)
    {
        GO.transform.DOKill();
        GO.transform.localScale = fromScale;
        GO.transform
            .DOScale(toScale, duration)
            .SetEase(easeType)
            .SetLoops(2, LoopType.Yoyo);
    }

}
