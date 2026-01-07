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
    [Header("Time For")] //time to secure point is minTimeBeforeCombo + comboWindowDuration
    [SerializeField]
    public float timeToSecurePoints = 1;
    #endregion


    #region Events
    [SerializeField]
    UnityEvent<float> RunStarted; //minTimeBeforeCombo and comboWindowDuration
    [SerializeField]
    UnityEvent<float> RunContinued; //minTimeBeforeCombo and comboWindowDuration

    [SerializeField]
    UnityEvent<float> UpdatedRunningPoints;
    [SerializeField]
    UnityEvent<float> UpdatedExpectedPoints;
    [SerializeField]
    UnityEvent<float> UpdatedTotalPoints;
    [SerializeField]
    UnityEvent<float> UpdatedNumberOfCombos; //number of combos
    [SerializeField]
    UnityEvent<float> UpdatedComboMultiplier; //comboMuliplierCalciation

    [SerializeField]
    UnityEvent<float> ResetedRunningPoints;
    [SerializeField]  
    UnityEvent<float> ResetedExpectedPoints;
    [SerializeField]  
    UnityEvent<float> ResetedTotalPoints;
    [SerializeField]  
    UnityEvent<float> ResetedNumberOfCombos;
    [SerializeField]  
    UnityEvent<float> ResetedComboMultiplier;
    

    [SerializeField]
    UnityEvent<float, float> ComboStarted; //minTimeBeforeCombo and comboWindowDuration
    [SerializeField]
    UnityEvent<float> NewHighScore;
    [SerializeField]
    UnityEvent<float> ScuredPoints;
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
        float diff = Time.time - lastNearmiss;
        float normalized = diff / timeToSecurePoints;
        if (normalized > .1f)
            UpdateNumberOfCombos();

        lastNearmiss = Time.time;
        UpdatePoints(Mathf.Abs(normalizedDistance - 1));
        SetSecureTimer();
    }
    void UpdateNumberOfCombos() 
    {
        numberOfCombos++;
        UpdatedNumberOfCombos.Invoke(numberOfCombos);
        UpdatedComboMultiplier.Invoke(ComboPointsMultiplier());
    }

    void UpdatePoints(float normalizedDistance)
    {
        runningPoints += RunnignPointsCalculation(normalizedDistance, false);
        expectedPoints = totalPoints + runningPoints;

        UpdatedRunningPoints.Invoke(runningPoints);
        UpdatedExpectedPoints.Invoke(expectedPoints);
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
    void SetSecureTimer()
    {
        if (!ResetSecureTimer())
            RunStarted.Invoke(timeToSecurePoints);
        else
            RunContinued.Invoke(timeToSecurePoints);

        secureTimer = StartCoroutine(SecureTimer());
    }

    bool ResetSecureTimer()
    {
        if (secureTimer != null)
        {
            StopCoroutine(secureTimer);
            secureTimer = null;
            return true;
        }
        return false;
    }

    void PointsSecured()
    {
        totalPoints = expectedPoints;
        ScuredPoints.Invoke(totalPoints);
        UpdatedTotalPoints.Invoke(totalPoints);

        HighSchoreCheck();
        ResetRunPoints();
    }

    void ResetRunPoints() 
    {
        runningPoints = 0;
        numberOfCombos = 0;

        ResetedRunningPoints.Invoke(runningPoints);
        ResetedNumberOfCombos.Invoke(numberOfCombos);
        ResetedComboMultiplier.Invoke(ComboPointsMultiplier());

        ResetSecureTimer();
    }
    #endregion

    #region HighScore
    void HighSchoreCheck()
    {
        if (GameManager.Instance.highScore > totalPoints)
            return;

        NewHighScore.Invoke(totalPoints);
        GameManager.Instance.highScore = totalPoints;
        GameManager.Instance.highScorer = UserData.Instance.UserName;
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

        ResetedTotalPoints.Invoke(totalPoints);
        ResetedRunningPoints.Invoke(runningPoints);
        ResetedNumberOfCombos.Invoke(numberOfCombos);
        ResetedExpectedPoints.Invoke(expectedPoints);
    }

}
