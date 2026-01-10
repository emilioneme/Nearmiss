using DG.Tweening;
using eneme;
using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField]
    GameObject PlayerUICanvas;

    [SerializeField]
    CinemachineCamera cam;

    [Header("Speedometer")]
    [SerializeField] float speedometerMultiplier = 10;

    [Header("UI")]
    [SerializeField] TMP_Text SpeedText;

    [Header("Hame Objects")]
    [SerializeField] GameObject RunningPointsGO;
    [SerializeField] GameObject ComboNumGO;
    [SerializeField] GameObject TotalPointsGO;

    [Header("Texts")]
    [SerializeField] TMP_Text TotalPointsText;
    [SerializeField] TMP_Text RunningPointsText;
    [SerializeField] TMP_Text ComboNumText;

    [Header("Circles")]
    [SerializeField] Image TotalPointsImage;
    [SerializeField] Image RunningPointsImage;
    [SerializeField] Image ComboNumImage;

    [SerializeField] AnimationCurve CircleFillCurve;

    [Header("FOV")]
    [SerializeField]
    float minFov = 60;
    [SerializeField]
    float maxFovAdditive = 30;

    Coroutine RunRoutine;

    #region  Run 
    public void RunStarted(float timeToSecure)
    {
        DestroyCourutineSafely(ref RunRoutine);
        RunRoutine = StartCoroutine(RunCooldownCoroutine(timeToSecure));
    }

    public void RunContinued(float timeToSecure)
    {
        DestroyCourutineSafely(ref RunRoutine);
        RunRoutine = StartCoroutine(RunCooldownCoroutine(timeToSecure));
    }

    public void PointsSecured(float points)
    {
        TotalPointsText.text = Tools.ProcessFloat(points, 2);

        TotalPointsGO.transform.DOKill();

        TotalPointsGO.transform
            .DOScale(0.5f, 0.25f)   // go smaller
            .SetEase(Ease.InOutSine)
            .SetLoops(2, LoopType.Yoyo);
    }

    void DestroyCourutineSafely(ref Coroutine Routine) 
    {
        if (Routine != null)
            StopCoroutine(Routine);
        Routine = null;
    }
    #endregion

    #region RunCooldown
    IEnumerator RunCooldownCoroutine(float timeToSecure) 
    {
        float timeLapsed = 0;
        float secureNormalized = 0;
        float fill = 0;
        while (timeLapsed < timeToSecure) 
        {
            timeLapsed += Time.deltaTime;
            secureNormalized = timeLapsed / timeToSecure;
            fill = secureNormalized < .1f? 0 : secureNormalized;
            //RunningPointsImage.fillAmount = Mathf.Abs(fill - 1);
            ComboNumImage.fillAmount = CircleFillCurve.Evaluate(Mathf.Abs(fill - 1));
            yield return null;
        }
        RunRoutine = null;
    }
    #endregion

    #region Run Points
    public void UpdateRunPoints(float points)
    {
        //RunningPointsText.text = Tools.ProcessFloat(points, 2);
        if (!RunningPointsGO.activeSelf)
            //RunningPointsGO.SetActive(true);
        if (!ComboNumGO.activeSelf)
            ComboNumGO.SetActive(true);
    }

    public void ResetedRunPoints(float points)
    {
        //RunningPointsText.text = Tools.ProcessFloat(points, 2);
        RunningPointsGO.SetActive(false);
        ComboNumGO.SetActive(false);
    }
    #endregion

    #region Combos
    public void UpdateNumberOfCombo(float numberOfCombos)
    {
        if(numberOfCombos > 1)
            ComboNumText.text = "x" + numberOfCombos.ToString();
        else
            ComboNumText.text = " ";

    }
    public void ResetedNumberOfCombo(float numberOfCombos)
    {
        ComboNumText.text = " ";
    }
    #endregion

    #region Totalpoints
    public void UpdateTotalPoints(float points)
    {
        TotalPointsText.text = Tools.ProcessFloat(points, 2);
    }

    public void ResetedTotalPoints(float points)
    {
        TotalPointsText.text = Tools.ProcessFloat(points, 2);
    }
    #endregion

    #region Speedotemeter
    public void UpdateSpeedometer()
    {
        SpeedText.text = Tools.ProcessFloat(UserData.Instance.droneVelocity.magnitude * speedometerMultiplier, 3);
    }

    public void UpdateFOV() 
    {
        float speedFOV = minFov + Mathf.InverseLerp(30, 100, UserData.Instance.droneVelocity.magnitude) * maxFovAdditive;
        cam.Lens.FieldOfView = speedFOV;
    }
    #endregion

    #region CrashUI

    public void TriggerCrashUI(ControllerColliderHit hit) 
    {
    }
    #endregion

    public void PlayerSpawned() 
    {
        RunningPointsGO.SetActive(false);
        ComboNumGO.SetActive(false);
    }

    public void HidePlayerUI() 
    {
        PlayerUICanvas.SetActive(false);
    }

    public void UnhidePlayerUI()
    {
        PlayerUICanvas.SetActive(true);
    }
    
}
