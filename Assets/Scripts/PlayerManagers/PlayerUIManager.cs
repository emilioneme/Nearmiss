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

}
