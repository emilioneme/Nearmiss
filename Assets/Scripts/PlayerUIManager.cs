using eneme;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField]
    PlayerManager pm;

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

    float runCircleFill = 0;
    Corutine RunRutine;    
    private void Update()
    {
        if(pm == null)
            return;
        
        float runningPointsFill = Mathf.Abs(pm.pointManager.SecurePointsCooldown());
        runningPointsFill = runningPointsFill <= .1f ? 1 : runningPointsFill;
        RunningPointsImage.fillAmount = runningPointsFill;

        float comboNumFill = Mathf.Abs(pm.pointManager.BeforeComboWindowCooldown());
        float comboMultFill = Mathf.Abs(pm.pointManager.ComboWindowCooldown() - 1);
        float comboFill =
            comboNumFill    > .1f   && comboNumFill     < 1 ? comboNumFill
            : comboMultFill > 0     && comboMultFill    < 1 ? comboMultFill 
            : 1;
        ComboMultImage.fillAmount = comboFill;
    }

    #region  Run
    public void RunStarted()
    {
        EnableRunObjects(true);
    }

    public void PointsSecured(float points)
    {
        EnableRunObjects(false);
        TotalPointsText.text = Tools.ProcessFloat(points, 2);
    }

    public void EnableRunObjects(bool enable)
    {
        RunningPointsGO.SetActive(enable);
        ComboMultGO.SetActive(enable);
        if(!enable)
            return;
        StopCorutine(RunRutine);
        RunRutine = null;
    }
    #endregion

    #region Run Points
    public void UpdateRunPoints(float points)
    {
        RunningPointsText.text = Tools.ProcessFloat(points, 2);
    }

    public void ResetedRunPoints(float points)
    {
        RunningPointsText.text = Tools.ProcessFloat(points, 2);
    }
    #endregion

    #region Combos
    public void UpdateNumberOfCombo(float numberOfCombos)
    {
        ComboMultText.text = "x" + numberOfCombos.ToString();
    }
    public void ResetedNumberOfCombo(float numberOfCombos)
    {
        ComboMultText.text = "x" + numberOfCombos.ToString();
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
        //Debug.Log("Plyer Crashed with: " + hit.gameObject.name + "\n In position: " + hit.point);
        EnableRunObjects(false);
    }
    #endregion
}
