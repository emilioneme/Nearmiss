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
    CinemachineCamera cam;

    [Header("Speedometer")]
    [SerializeField] float speedometerMultiplier = 10;

    [Header("UI")]
    [SerializeField] TMP_Text SpeedText;

    [Header("Hame Objects")]
    [SerializeField] GameObject RunningPointsGO;
    [SerializeField] GameObject ComboMultGO;

    [Header("Texts")]
    [SerializeField] TMP_Text TotalPointsText;
    [SerializeField] TMP_Text RunningPointsText;
    [SerializeField] TMP_Text ComboMultText;
    [SerializeField] TMP_Text ComboNumText;

    [Header("Circles")]
    [SerializeField] Image TotalPointsImage;
    [SerializeField] Image RunningPointsImage;
    [SerializeField] Image ComboMultImage;
    [SerializeField] Image ComboNumImage;

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
            RunningPointsImage.fillAmount = Mathf.Abs(fill - 1);
            yield return null;
        }
        RunRoutine = null;
    }
    #endregion

    #region Run Points
    public void UpdateRunPoints(float points)
    {
        RunningPointsText.text = Tools.ProcessFloat(points, 2);
        if(!RunningPointsGO.activeSelf)
            RunningPointsGO.SetActive(true);
    }

    public void ResetedRunPoints(float points)
    {
        RunningPointsText.text = Tools.ProcessFloat(points, 2);
        RunningPointsGO.SetActive(false);
    }
    #endregion

    #region Combos
    public void UpdateNumberOfCombo(float numberOfCombos)
    {
        ComboMultText.text = "x" + numberOfCombos.ToString();
        if(numberOfCombos >= 1 && !ComboMultGO.activeSelf) 
            ComboMultGO.SetActive(true);
    }
    public void ResetedNumberOfCombo(float numberOfCombos)
    {
        ComboMultText.text = "x" + numberOfCombos.ToString();
        ComboMultGO.SetActive(false);
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
    public void UpdateSpeedometer(float speed)
    {
        SpeedText.text = Tools.ProcessFloat(speed * speedometerMultiplier, 3);
        float speedFOV = minFov + Mathf.InverseLerp(30, 100, speed) * maxFovAdditive;
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
        ComboMultGO.SetActive(false);
    }
}
