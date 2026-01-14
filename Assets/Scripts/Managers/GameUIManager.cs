using eneme;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
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

    public void GoToScene(string sceneName)
    {
        SceneLoader.Instance.LoadScene(sceneName);
    }
}
