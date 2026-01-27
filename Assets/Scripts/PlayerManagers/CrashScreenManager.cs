using DG.Tweening;
using eneme;
using TMPro;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;

public class CrashScreenManager : MonoBehaviour
{
    [SerializeField]
    GameObject CrashPanel;
    [SerializeField]
    RectTransform rt;

    [SerializeField]
    TMP_Text currentScoreText;
    [SerializeField]
    TMP_Text highScoreText;
    [SerializeField]
    TMP_Text personalScoreText;

    [SerializeField] Camera cam;

    public void Start()
    {
        UpdateHighScore();
        UpdatePersonalHighScore();
    }

    public void UpdateHighScore() 
    {
        //highScoreText.text = Tools.ProcessFloat(GameManager.Instance.highScore, 1);
    }

    public void UpdatePersonalHighScore()
    {
        personalScoreText.text = Tools.ProcessFloat(UserData.Instance.personalHighScore, 1);
    }

    public void UpdateCurrentScore(float score)
    {
        currentScoreText.text = Tools.ProcessFloat(score, 1);
    }

    public void OpenCrashCanvas()
    {
        UpdateHighScore();
        UpdatePersonalHighScore();


        CrashPanel.SetActive(true);
        float canvasWidth = rt.rect.width;

        rt.anchoredPosition = new Vector2(canvasWidth, 0);
        rt.DOAnchorPos(Vector2.zero, .25f).SetEase(Ease.OutCubic);

        UserData.Instance.canPause = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseCrashCanvas()
    {
        float canvasWidth = rt.rect.width;
        rt.anchoredPosition = Vector2.zero;
        rt.DOAnchorPos(new Vector2(-canvasWidth, 0), .25f).SetEase(Ease.OutCubic)
           .OnComplete(() => CrashPanel.SetActive(false));

        UserData.Instance.canPause = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void GoToScene(string sceneName)
    {
        SceneLoader.Instance.LoadScene(sceneName);
    }

    public void OpenSettings() 
    {
        SettingsManager.Instance.OpenSettings();
    }

    public void CloseSettings()
    {
        SettingsManager.Instance.CloseSettings();
    }

}
