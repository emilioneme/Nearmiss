using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerManager))]
public class PlayerUIManager : MonoBehaviour
{
    PlayerManager playerManager;

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

    [SerializeField] TMP_Text PointsText;
    [SerializeField] TMP_Text RunningPointsText;
    [SerializeField] TMP_Text ComboMultText;
    //[SerializeField] TMP_Text HighScoreText;

    [SerializeField]
    Image SecurePointsImage;
    [SerializeField]
    Image CanAddMultImage;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
    }

    private void Update()
    {
        HandleSpeedometerText();

        ComboMultText.text = (playerManager.comboMultiplier - 1).ToString();
        PointsText.text = eneme.Tools.ProcessFloat(playerManager.expectedPoints);

        SecurePointsImage.fillAmount = playerManager.SecurePointsCooldown();
        CanAddMultImage.fillAmount = playerManager.ComboMultCooldwon();
    }

    #region Speedotemeter
    void HandleSpeedometerText() 
    {
        if(!playerManager.droneMovement.enabled)
            return;
        float speed = 0;
        if (speedometerMode == SpeedometerMode.VELOCITY)
            speed = playerManager.droneMovement.GetTotalVelocity().magnitude * speedometerMultiplier;
        if (speedometerMode == SpeedometerMode.FORWARDSPEED)
            speed = playerManager.droneMovement.GetForwardVelocity().magnitude * speedometerMultiplier;
        SpeedText.text = eneme.Tools.ProcessFloat(speed);
    }
    #endregion

    #region CrashUI
    public void TriggerCrashUI(ControllerColliderHit hit) 
    {
        Debug.Log("Plyer Crashed with: " + hit.gameObject.name + "\n In position: " + hit.point);
    }
    #endregion
}
