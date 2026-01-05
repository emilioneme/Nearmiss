using eneme;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    GameObject menuCanvas;

    [SerializeField]
    GameObject settingsCanvas;

    [SerializeField]
    GameObject custumizationCanvas;

    [SerializeField]
    GameObject leaderboardCanvas;


    public enum Canvas
    {
        menu,
        settings,
        custumization,
        leaderboard
    }

    Canvas currentCanvas = Canvas.menu;

    private void Start()
    {
        SettingsManager.Instance.SettingsClosed.AddListener(SettingsClosed);
    }

    #region Settings
    public void OpenSettings()
    {
        ChangeCanvasTo(Canvas.settings);
    }

    public void SettingsClosed()
    {
        ChangeCanvasTo(Canvas.menu);
    }

    #endregion

    #region CanvasManager
    public void ChangeCanvasTo(Canvas canvas)
    {
        if (canvas == currentCanvas)
            return;
        currentCanvas = canvas;
        CanvasChange();
    }

    void CanvasChange()
    {
        menuCanvas.SetActive(currentCanvas == Canvas.menu);
        settingsCanvas.SetActive(currentCanvas == Canvas.settings);
        //leaderboardCanvas.SetActive(currentCanvas == Canvas.leaderboard);
        //custumizationCanvas.SetActive(currentCanvas == Canvas.custumization);
    }
    #endregion
}
