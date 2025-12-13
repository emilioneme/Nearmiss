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
    }

    [Header("UI")]
    [SerializeField] TMP_Text SpeedText;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
    }

    private void Update()
    {
        string text = "";
        if (speedometerMode == SpeedometerMode.VELOCITY)
            text = (playerManager.droneMovement.GetVelocity() * speedometerMultiplier).ToString();

        if (speedometerMode == SpeedometerMode.FORWARDSPEED)
            text = (playerManager.droneMovement.CurrentForwardSpeed() * speedometerMultiplier).ToString();

        if (text.Length > maxStringLength)
            text = text.Substring(0, maxStringLength);

        if (text[text.Length - 1] == '.')
               text = text.Substring(0, text.Length - 1);
        


        SpeedText.text = text;
    }

    public void TriggerCrashUI(ControllerColliderHit hit) 
    {
        Debug.Log("Plyer Crashed with: " + hit.gameObject.name + "\n In position: " + hit.point);
    }
}
