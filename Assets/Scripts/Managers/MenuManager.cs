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

}
