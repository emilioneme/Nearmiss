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

    private void Awake()
    {
        SettingsManager.Instance.SettingsClosed.AddListener(CloseSettings);
    }

    private void Start()
    {
        HighScoreText.text = GameManager.Instance.highScore.ToString();
        HighScorerText.text = GameManager.Instance.highScorer;
    }

    public void GoToScene(string sceneName) 
    {
        SceneLoader.Instance.LoadScene(sceneName);
    }

    #region Settings
    public void OpenSettings()
    {
        SettingsManager.Instance.settingsCanvas.SetActive(true);
    }

    public void CloseSettings()
    {
        SettingsManager.Instance.settingsCanvas.SetActive(false);
    }
    #endregion

}
