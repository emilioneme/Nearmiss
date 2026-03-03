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
        SpeedometerText.text = Tools.ProcessFloat(UserData.Instance.droneVelocity.magnitude, 1);
    }

}
