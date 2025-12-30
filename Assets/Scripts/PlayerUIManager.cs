using eneme;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerManager))]
public class PlayerUIManager : MonoBehaviour
{
    PlayerManager pm;

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



    private void Awake()
    {
        pm = GetComponent<PlayerManager>();
    }

    private void Update()
    {
        HandleSpeedometerText();

        TotalPointsText.text = Tools.ProcessFloat(pm.totalPoints);

        float runningPointsFill = Mathf.Abs(pm.SecurePointsCooldown());
        runningPointsFill = runningPointsFill <= .1f? 1 : runningPointsFill;
        RunningPointsText.text = Tools.ProcessFloat(pm.runningPoints);
        RunningPointsImage.fillAmount = runningPointsFill;

        float comboMultFill = Mathf.Abs(pm.ComboWindowCooldown() - 1);
        comboMultFill = comboMultFill == 0 ? 1 : comboMultFill;
        ComboMultText.text = Tools.LimitNumberLength(pm.ComboMultiplier(), 3);
        ComboMultImage.fillAmount = comboMultFill;

        float comboNumFill = Mathf.Abs(pm.ComboMultCooldwon());
        comboNumFill = comboNumFill <= .1f? 1: comboNumFill;
        ComboNumText.text = pm.numberOfCombos.ToString();
        ComboNumImage.fillAmount = comboNumFill;

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
        SpeedText.text = Tools.ProcessFloat(speed);
    }
    #endregion

    #region CrashUI
    public void TriggerCrashUI(ControllerColliderHit hit) 
    {
        Debug.Log("Plyer Crashed with: " + hit.gameObject.name + "\n In position: " + hit.point);
    }
    #endregion
}
