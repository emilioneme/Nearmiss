using DG.Tweening;
using eneme;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;

public class CrashScreenManager : MonoBehaviour
{
    [SerializeField]
    GameObject CrashPanel;
    [SerializeField]
    RectTransform Panel;

    [SerializeField]
    TMP_Text currentScoreText;
    [SerializeField]
    TMP_Text highScoreText;
    [SerializeField]
    TMP_Text personalScoreText;

    public void Start()
    {
        UpdateHighScore();
        UpdatePersonalHighScore();
    }

    public void UpdateHighScore() 
    {
        highScoreText.text = Tools.ProcessFloat(GameManager.Instance.highScore, 1);
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
        Panel.transform.localScale = Vector3.zero;
        Panel.transform
            .DOScale(1, .2f);

        UserData.Instance.canPause = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseCrashCanvas()
    {
        CrashPanel.SetActive(false);
        Panel.transform.localScale = Vector3.one;
        Panel.transform
            .DOScale(0, .2f);

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
