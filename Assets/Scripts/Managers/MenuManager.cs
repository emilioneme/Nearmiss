using eneme;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MenuManager : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField]
    TMP_Text HighScoreText;
    [SerializeField]
    TMP_Text HighScorerText;

    private void Start()
    {
        HighScoreText.text = Tools.ProcessFloat(GameManager.Instance.highScore, 2);
        HighScorerText.text = GameManager.Instance.highScorer;
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

}
