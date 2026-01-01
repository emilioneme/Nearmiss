using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PointManager : MonoBehaviour
{
    [Header("Points")]
    [SerializeField]
    public float totalPoints = 0;
    [SerializeField]
    public float runningPoints = 0;
    [SerializeField]
    public float expectedPoints = 0;
    [SerializeField]
    public int numberOfCombos = 0;

    #region  Multipliers
    [Header("Point Calculation")]
    [SerializeField]
    [Range(0f, 1f)]
    public float maxDistancePoints = 10;
    [SerializeField]
    [Range(0f, .1f)]
    public float speedPointsMultiplier = .5f;
    [SerializeField]
    public float maxComboMultiplier = 10;

    [Header("Combo Calculation")]
    [SerializeField]
    AnimationCurve comboMultiplierCurve;
    [SerializeField]
    public float minComboMultiplier = 1;
    [SerializeField]
    public float minCombos = 1;
    [SerializeField]
    public float maxCombos = 10;
    #endregion

    #region Expected Points
    float lastNearmiss = 0;
    [Header("Time For")]
    [SerializeField]
    [Range(0, 10f)]
    float timeToSecurePoints = 1.5f;
    [SerializeField]
    [Range(0, 0.9f)]
    float minTimeBeforeCombo = .5f; //has to be less than timeToSecurePointys
    #endregion

    #region Events
    [SerializeField]
    UnityEvent NewHighScore;
    [SerializeField]
    UnityEvent SecuredPoints;
    [SerializeField]
    UnityEvent RunStarted;
    [SerializeField]
    UnityEvent<float> ComboIncreased;
    #endregion

    PlayerManager pm;

    private void Awake()
    {
        pm = GetComponent<PlayerManager>();
    }

    #region NearmissHandler
    Coroutine secureTimer;
    public void PlayerNearmissed(float normalizedDistance, float distance, Vector3 playerPos, RaycastHit hit) //This is a float from 0 to 1
    {
        UpdateNumberOfCOmbos();
        lastNearmiss = Time.time;

        UpdatePoints(normalizedDistance);
        UpdateSecureTimer();
    }

    void UpdateNumberOfCOmbos() 
    {
        if (BeforeComboWindowCooldown() >= 1)
        {
            numberOfCombos++;
            ComboIncreased.Invoke(timeToSecurePoints - minComboMultiplier);
        }
    }

    void UpdatePoints(float normalizedDistance)
    {
        /* Running Points has Multiplier
        runningPoints   += RunnignPointsCalculation(normalizedDistance, true);
        expectedPoints  += RunnignPointsCalculation(normalizedDistance, true);
        //*/

        //* Running Points does not have multiplier
        runningPoints += RunnignPointsCalculation(normalizedDistance, false);
        expectedPoints += RunnignPointsCalculation(normalizedDistance, true);
        //*/
    }

    void UpdateSecureTimer() 
    {
        if (secureTimer != null)
        {
            StopCoroutine(secureTimer);
            secureTimer = null;
        }
        else
            RunStarted.Invoke();
        secureTimer = StartCoroutine(SecureTimer());
    }
    #endregion

    #region SecurePoints
    public float SecurePointsCooldown()
    {
        return eneme.Tools.CooldownSince(lastNearmiss, timeToSecurePoints);
    }
    IEnumerator SecureTimer()
    {
        while (SecurePointsCooldown() < 1)
        {
            yield return null;
        }
        PointsSecured();
        secureTimer = null;
    }

    void PointsSecured()
    {
        totalPoints = expectedPoints;
        SecuredPoints.Invoke();

        HighSchoreCheck();
        ResetRunPoints();
    }

    void ResetRunPoints() 
    {
        runningPoints = 0;
        numberOfCombos = 0;
        if (secureTimer != null)
        {
            StopCoroutine(secureTimer);
            secureTimer = null;
        }
    }

    void HighSchoreCheck()
    {
        if (GameManager.Instance.highScore > totalPoints)
            return;

        NewHighScore.Invoke();
        GameManager.Instance.highScore = totalPoints;
        GameManager.Instance.highScorer = UserData.Instance.UserName;
    }
    #endregion

    #region Combo Multiuplier
    public float BeforeComboWindowCooldown()
    {
        return eneme.Tools.CooldownSince(lastNearmiss, timeToSecurePoints - (timeToSecurePoints - minTimeBeforeCombo));
    }
     
    public float ComboWindowCooldown()
    {
        return eneme.Tools.CooldownSince(lastNearmiss + minTimeBeforeCombo, timeToSecurePoints);
    }

    #endregion

    #region Point Fomrulas
    float RunnignPointsCalculation(float normalizedDistance, bool comomboMultiplier)
    {
        float points = DistancePoints(normalizedDistance) + SpeedPoints();
        return comomboMultiplier ? points * ComboPointsMultiplier() : points;
    }

    float DistancePoints(float normalizedDistance)
    {
        return normalizedDistance * maxDistancePoints;
    }
    float SpeedPoints()
    {
        return pm.droneMovement.GetTotalVelocity().magnitude * speedPointsMultiplier;
    }
    public float ComboPointsMultiplier()
    {
        if (numberOfCombos <= 1)
            return numberOfCombos;
        float lerped = Mathf.InverseLerp(minCombos, maxCombos, numberOfCombos);
        return Mathf.Clamp(comboMultiplierCurve.Evaluate(lerped) * maxComboMultiplier, minComboMultiplier, maxComboMultiplier);
    }
    #endregion

    public void ResetPoints()
    {
        totalPoints = 0;
        runningPoints = 0;
        numberOfCombos = 0;
        expectedPoints = 0;
    }

}
