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
    [SerializeField] TMP_Text ExpectedPoints;
    [SerializeField] TMP_Text HighScoreText;

    [SerializeField]
    Image expectedPointsCooldownCircle;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
    }

    private void Update()
    {
        HandleSpeedometerText();

        expectedPointsCooldownCircle.fillAmount = playerManager.ComboCooldown();
    }

    #region Points Text
    public void UpdateExpectedPoints() 
    {
        ExpectedPoints.text = ProcessFloat(playerManager.expectedPoints);
    }

    public void UpdatePointsText()
    {
        PointsText.text = ProcessFloat(playerManager.totalPoints);
        ExpectedPoints.text = ProcessFloat(playerManager.expectedPoints);
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
            speed = playerManager.droneMovement.GetTotalVelocity().magnitude * speedometerMultiplier;
        if (speedometerMode == SpeedometerMode.FORWARDSPEED)
            speed = playerManager.droneMovement.CurrentForwardSpeed() * speedometerMultiplier;

        SpeedText.text = ProcessFloat(speed);
    }
    #endregion

    #region Text Filters
    string ProcessFloat(float f) 
    {
        if (f == 0)
            return "";
        float number = f;
        string unit = "";

        if      (f >= 1_000_000_000f) { number = f / 1_000_000_000f; unit = "b"; }
        else if (f >= 1_000_000f) { number = f / 1_000_000f; unit = "m"; }
        else if (f >= 1_000f) { number = f / 1_000f; unit = "k"; }
        else    { return Mathf.Round(f).ToString(); }

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
