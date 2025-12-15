using TMPro;
using UnityEngine;

[RequireComponent(typeof(PlayerManager))]
public class PlayerUIManager : MonoBehaviour
{
    PlayerManager playerManager;

    [Header("Speedometer")]
    [SerializeField] SpeedometerMode speedometerMode = 0;
    [SerializeField] int speedometerMultiplier = 10;
    [SerializeField] int maxStringLength = 3;

    enum SpeedometerMode
    {
        VELOCITY,
        FORWARDSPEED,
        TOTALSPEED,
    }

    [Header("UI")]
    [SerializeField] TMP_Text SpeedText;
    [SerializeField] TMP_Text PointsText;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
    }

    private void Update()
    {
        HandleSpeedometerText();

        HandlePointsText();
    }

    void HandleSpeedometerText() 
    {
        string text = "";

        if (speedometerMode == SpeedometerMode.VELOCITY)
            text = FilterSpeedToText(playerManager.droneMovement.GetVelocity());

        if (speedometerMode == SpeedometerMode.FORWARDSPEED)
            text = FilterSpeedToText(playerManager.droneMovement.CurrentForwardSpeed());
        if (speedometerMode == SpeedometerMode.TOTALSPEED)
            text = FilterSpeedToText(playerManager.droneMovement.GetTotalSpeed());

        SpeedText.text = ClampText(text);
    }

    void HandlePointsText() 
    {
        //string text = ClampText(playerManager.totalPoints.ToString());
        //PointsText.text = text;
    }

    string FilterSpeedToText(float speed) 
    {
        return (speed * speedometerMultiplier).ToString();
    }

    string ClampText(string text) 
    {
        if (text.Length > maxStringLength)
            text = text.Substring(0, maxStringLength);

        if (text[text.Length - 1] == '.')
            text = text.Substring(0, text.Length - 1);
        return text;
    }

    public void TriggerCrashUI(ControllerColliderHit hit) 
    {
        Debug.Log("Plyer Crashed with: " + hit.gameObject.name + "\n In position: " + hit.point);
    }
}
