using eneme;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerManager))]
public class PlayerUIManager : MonoBehaviour
{
    [SerializeField]
    PlayerManager pm;

    [SerializeField]
    CinemachineCamera cam;

    [Header("Speedometer")]
    [SerializeField] SpeedometerMode speedometerMode = 0;
    [SerializeField] int speedometerMultiplier = 10;

    enum SpeedometerMode
    {
        VELOCITY,
        FORWARDSPEED,
        DOWN,
        DASH,
    }

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


    private void Update()
    {
        HandleSpeedometerText();

        float vel = pm.droneMovement.GetTotalVelocity().magnitude;
        float speedFOV = minFov + Mathf.InverseLerp(30, 100, vel) * maxFovAdditive;
        cam.Lens.FieldOfView = speedFOV;

        TotalPointsText.text = Tools.ProcessFloat(pm.pointManager.totalPoints, 2);
        //TotalPointsText.text = Tools.ProcessFloat(pm.expectedPoints);

        if(pm.pointManager.runningPoints != 0) 
        {
            RunningPointsGO.SetActive(true);
            ComboMultGO.SetActive(true);
        } 
        else 
        {
            RunningPointsGO.SetActive(false);
            ComboMultGO.SetActive(false);
        }

        if (RunningPointsGO.activeInHierarchy) 
        {
            float runningPointsFill = Mathf.Abs(pm.pointManager.SecurePointsCooldown());
            runningPointsFill = runningPointsFill <= .1f ? 1 : runningPointsFill;
            RunningPointsImage.fillAmount = runningPointsFill;
            RunningPointsText.text = Tools.ProcessFloat(pm.pointManager.runningPoints, 2);


            float comboNumFill = Mathf.Abs(pm.pointManager.BeforeComboWindowCooldown());
            float comboMultFill = Mathf.Abs(pm.pointManager.ComboWindowCooldown() - 1);

            float comboFill =
                comboNumFill    > .1f   && comboNumFill     < 1 ? comboNumFill
                : comboMultFill > 0     && comboMultFill    < 1 ? comboMultFill 
                : 1;
            ComboMultImage.fillAmount = comboFill;
            //ComboMultText.text = "x" + Tools.LimitNumberLength(pm.ComboMultiplier(), 4);
            ComboMultText.text = "x" + pm.pointManager.numberOfCombos.ToString();
        }

    }

    #region Speedotemeter
    void HandleSpeedometerText() 
    {
        if(!pm.droneMovement.enabled)
            return;
        float speed = 0;
        if (speedometerMode == SpeedometerMode.VELOCITY)
            speed = pm.droneMovement.GetTotalVelocity().magnitude * speedometerMultiplier;
        if (speedometerMode == SpeedometerMode.FORWARDSPEED)
            speed = pm.droneMovement.GetForwardVelocity().magnitude * speedometerMultiplier;
        SpeedText.text = Tools.ProcessFloat(speed, 1);
    }
    #endregion

    #region CrashUI
    public void TriggerCrashUI(ControllerColliderHit hit) 
    {
        Debug.Log("Plyer Crashed with: " + hit.gameObject.name + "\n In position: " + hit.point);
    }
    #endregion
}
