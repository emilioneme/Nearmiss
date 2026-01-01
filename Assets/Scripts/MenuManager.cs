using eneme;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField]
    GameObject settingsPanel;
    [SerializeField]
    GameObject mainButtonsPannel;

    [Header("Settings")]
    [SerializeField]
    Slider masterVolumeSlider;
    [SerializeField]
    Slider musicVolumeSlider;
    [SerializeField]
    Slider mouseSenseSlider;
    [SerializeField]
    Slider stickSenseSlider;

    [Header("Highscore")]
    [SerializeField]
    TMP_Text HighScoreText;

    GameManager gm;
    UserData ud;

    private void Start()
    {
        gm = GameManager.Instance;
        ud = UserData.Instance;

        SetUpMenu();
    }
    public void SetUpMenu() 
    {
        SliderSetUp();
        HighScoreUpdate();
    }

    public void SliderSetUp() 
    {
        masterVolumeSlider.value = ud.masterVolume;
        musicVolumeSlider.value = ud.musicVolume;
        mouseSenseSlider.value = ud.mouseSensitivity;
        stickSenseSlider.value = ud.stickSensitivity;
    }
    public void SliderUpdate() 
    {
        ud.masterVolume = masterVolumeSlider.value;
        ud.musicVolume = musicVolumeSlider.value;
        ud.mouseSensitivity = mouseSenseSlider.value;
        ud.stickSensitivity = stickSenseSlider.value;
    }
    public void HighScoreUpdate() 
    {
        HighScoreText.text = gm.highScore.ToString(); //Process float later
    }

    #region Panels
    public void ToggleSettings() 
    {
        Debug.Log("settings");
        TogglePanel(settingsPanel);
        TogglePanel(mainButtonsPannel);
    }
    void TogglePanel(GameObject panel) 
    {
        panel.SetActive(!panel.activeInHierarchy);
    }
    void SwitchPanel(GameObject onPanel, GameObject offPanel = null) 
    {
        onPanel.SetActive(true);

        if(offPanel != null)
            onPanel.SetActive(false);
    }
    #endregion

    public void LoadScene(string scene)
    {
        Debug.Log("sceneLoad");
        SceneLoader.Instance.LoadScene(scene);
    }

}
