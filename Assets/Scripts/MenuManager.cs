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

    private void Start()
    {
        gm = GameManager.Instance;

        SetUpMenu();
    }
    public void SetUpMenu() 
    {
        SliderSetUp();
        HighScoreUpdate();
    }

    public void SliderSetUp() 
    {
        masterVolumeSlider.value = gm.masterVolume;
        musicVolumeSlider.value = gm.musicVolume;
        mouseSenseSlider.value = gm.mouseSensitivity;
        stickSenseSlider.value = gm.stickSensitivity;
    }
    public void SliderUpdate() 
    {
        gm.masterVolume = masterVolumeSlider.value;
        gm.musicVolume = musicVolumeSlider.value;
        gm.mouseSensitivity = mouseSenseSlider.value;
        gm.stickSensitivity = stickSenseSlider.value;
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
