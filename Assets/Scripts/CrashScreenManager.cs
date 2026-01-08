using eneme;
using TMPro;
using UnityEngine;

public class CrashScreenManager : MonoBehaviour
{
    [SerializeField]
    GameObject CrashCanvas;

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
        CrashCanvas.SetActive(true);
        UserData.Instance.canPause = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseCrashCanvas()
    {
        CrashCanvas.SetActive(false);
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
