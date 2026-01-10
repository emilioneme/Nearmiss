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

    [Header("TypesOfCOmbo")]
    [SerializeField] int numberOfDashCombos = 0;
    [SerializeField] int numberOfSkimCombos = 0;
    #endregion

    #region Expected Points
    float lastNearmiss = 0;
    [Header("Time For")] //time to secure point is minTimeBeforeCombo + comboWindowDuration
    [SerializeField]
    public float timeToSecurePoints = 1;
    [SerializeField]
    public float timeToSkim = .3f;
    #endregion


    #region Events
    [SerializeField]
    UnityEvent<float> RunStarted; //minTimeBeforeCombo and comboWindowDuration
    [SerializeField]
    UnityEvent<float> RunContinued; //minTimeBeforeCombo and comboWindowDuration
    [SerializeField]
    UnityEvent<float> RunningPointsCalculated;

    [SerializeField]
    UnityEvent<float> UpdatedRunningPoints;
    [SerializeField]
    UnityEvent<float> UpdatedExpectedPoints;
    [SerializeField]
    UnityEvent<float> UpdatedTotalPoints;
    [SerializeField]
    UnityEvent<float> UpdatedNumberOfCombos; //number of combos = number of skims + number of swerves
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
    UnityEvent<float> NewPersonalHighScore;
    [SerializeField]
    UnityEvent<float> ScuredPoints;
    #endregion

    Coroutine secureTimer;
    Coroutine dashCoroutine;


    #region NearmissHandler
    public void OnNearmiss(float normalizedDistance, int numberOfHits, Vector3 playerPos, RaycastHit hit) //This is a float from 0 to 1
    {
        float diff = Time.time - lastNearmiss;
        float normalized = diff / timeToSecurePoints;
        if (normalized > timeToSkim)
            UpdateNumberOfCombos(); // skim combo

        if(dashCoroutine != null) 
        {
            DestroyCourutineSafely(ref dashCoroutine);
            UpdateNumberOfCombos();
        }


        lastNearmiss = Time.time;
        UpdatePoints(normalizedDistance, numberOfHits);
        SetSecureTimer();
    }

    void UpdateNumberOfCombos()
    {
        numberOfCombos++;
        UpdatedNumberOfCombos.Invoke(numberOfCombos);
        UpdatedComboMultiplier.Invoke(ComboPointsMultiplier());
    }

    public void Dashed(Vector3 dir, Vector3 axis, float duration) 
    {
        if (dashCoroutine == null)
            DestroyCourutineSafely(ref dashCoroutine);
        dashCoroutine = StartCoroutine(DashCoroutine(duration));
    }

    IEnumerator DashCoroutine(float duration) 
    {
        float cooldown = 0;
        while(cooldown < duration)
        {
            cooldown += Time.deltaTime;
            yield return null;
        }
        DestroyCourutineSafely(ref dashCoroutine);
        dashCoroutine = null;
    }

    void UpdatePoints(float normalizedDistance, int numberOfHits)
    {
        float points = RunnignPointsCalculation(normalizedDistance, numberOfHits, UserData.Instance.droneVelocity.magnitude, speedPointsMultiplier);
        runningPoints += points * ComboPointsMultiplier();
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
            DestroyCourutineSafely(ref secureTimer);
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

        PersonalHighSchoreCheck();
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
        if (GameManager.Instance.highScore >= totalPoints)
            return;

        NewHighScore.Invoke(totalPoints);
        GameManager.Instance.highScore = totalPoints;
        GameManager.Instance.highScorer = UserData.Instance.UserName;
    }
    void PersonalHighSchoreCheck()
    {
        if (UserData.Instance.personalHighScore >= totalPoints)
            return;

        NewPersonalHighScore.Invoke(totalPoints);
        UserData.Instance.personalHighScore = totalPoints;
    }
    #endregion

    #region Point Fomrulas
    float RunnignPointsCalculation(float normalizedDistance, int numberOfHits, float velocity, float speedPointsMultiplier)
    {
        return (1 + normalizedDistance) * SpeedPoints(velocity, speedPointsMultiplier, numberOfHits);
    }
    float SpeedPoints(float droneVelocity, float speedPointsMultiplier, int numberOfHits)
    {
        return droneVelocity * speedPointsMultiplier * numberOfHits;
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

        DestroyCourutineSafely(ref secureTimer);
    }


    void DestroyCourutineSafely(ref Coroutine Routine)
    {
        if (Routine != null)
            StopCoroutine(Routine);
        Routine = null;
    }

    public void OnCrash()
    {
        DestroyCourutineSafely(ref secureTimer);
        DestroyCourutineSafely(ref dashCoroutine);
    }
}
