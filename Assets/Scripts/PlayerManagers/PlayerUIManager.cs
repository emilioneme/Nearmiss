using DG.Tweening;
using eneme;
using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] GameObject CanvasGO;

    [SerializeField] TMP_Text SpeedometerText;

    [SerializeField] PlayerManager PlayerManager;

    #region Pause
    public void Paused() 
    {
        CanvasGO.SetActive(false);
    }   

    public void UnPaused() 
    {
        CanvasGO.SetActive(true);
    }
    #endregion

    private void Update()
    {
        SpeedometerText.text = Tools.ProcessFloat(PlayerManager.droneMovement.droneVelocity.magnitude, 1);
    }

    public void OpenSettings() 
    {
        SettingsManager.Instance.OpenSettings();
    }

}
