using TMPro;
using UnityEngine;

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
        TOTALSPEED,
    }

    [Header("UI")]
    [SerializeField] TMP_Text SpeedText;
    [SerializeField] TMP_Text AirResistanceText;
    [SerializeField] TMP_Text PointsText;
    [SerializeField] TMP_Text HighScoreText;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
    }

    private void Update()
    {
        HandleSpeedometerText();
    }

    #region Points Text
    public void UpdatePointsText()
    {
        PointsText.text = ProcessFloat(playerManager.totalPoints);
    }

    public void UpdateHighScoreText()
    {
        HighScoreText.text = ProcessFloat(GameManager.Instance.highScore);
    }
    #endregion

    #region Speedotemeter
    void HandleSpeedometerText() 
    {
        float speed = 0;

        if (speedometerMode == SpeedometerMode.VELOCITY)
            speed = playerManager.droneMovement.GetVelocity() * speedometerMultiplier;
        if (speedometerMode == SpeedometerMode.FORWARDSPEED)
            speed = playerManager.droneMovement.CurrentForwardSpeed() * speedometerMultiplier;
        if (speedometerMode == SpeedometerMode.TOTALSPEED)
            speed = playerManager.droneMovement.GetTotalSpeed() * speedometerMultiplier;

        SpeedText.text = ProcessFloat(speed);
    }
    #endregion

    #region Text Filters
    string ProcessFloat(float f) 
    {
        float number = f;
        string unit = "";

        if (f >= 1_000_000_000f) { number = f / 1_000_000_000f; unit = "b"; }
        else if (f >= 1_000_000f) { number = f / 1_000_000f; unit = "m"; }
        else if (f >= 1_000f) { number = f / 1_000f; unit = "k"; }
        else { return Mathf.Round(f).ToString(); }

        number = Mathf.Round(number * 10f) / 10f;   // 1 decimal place
        return number.ToString("0.#") + unit;
    }
    #endregion

    #region CrashUI
    public void TriggerCrashUI(ControllerColliderHit hit) 
    {
        Debug.Log("Plyer Crashed with: " + hit.gameObject.name + "\n In position: " + hit.point);
    }
    #endregion
}
