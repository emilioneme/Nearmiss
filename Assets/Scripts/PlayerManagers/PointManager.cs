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
    public float comboMultiplier = 1;

    [Header("TypesOfCOmbo")]
    [SerializeField] int numberOfSwerveCombos = 0;
    [SerializeField] int numberOfSkimCombos = 0;


    #region  Multipliers
    [Header("Point Calculation")]
    [SerializeField]
    public float runningPointsMultiplier = .5f;
    [SerializeField]
    public float speedPointMultiplier = .5f;
    [SerializeField]
    public float maxDistancePoints = .5f;

    [Header("Combo Calculation")]
    [SerializeField]
    float swerveMultiplier = 2;
    [SerializeField]
    float skimMultiplier = .25f;
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
    [Header("Run")]
    [SerializeField]
    UnityEvent<float> RunStarted; //minTimeBeforeCombo and comboWindowDuration
    [SerializeField]
    UnityEvent<float> RunContinued; //minTimeBeforeCombo and comboWindowDuration

    [Header("UpdatedPoints")]
    [SerializeField]
    UnityEvent<float> UpdatedRunningPoints;
    [SerializeField]
    UnityEvent<float> UpdatedTotalPoints;
    [SerializeField]
    UnityEvent<float> UpdatedComboMultiplier; //comboMuliplierCalciation

    [Header("UpdatedComb")]
    [SerializeField]
    UnityEvent<float> UpdatedNumberOfSkims; //comboMuliplierCalciation
    [SerializeField]
    UnityEvent<float> UpdatedNumberOfSwerves; //comboMuliplierCalciation


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
        
        if (dashCoroutine != null) 
        {
            DestroyCourutineSafely(ref dashCoroutine);
            UpdateNumberOfSwerves(normalizedDistance);
        }
        
        if (normalized > timeToSkim)
        {
            UpdateNumberOfSkims(normalizedDistance);
        }


        lastNearmiss = Time.time;
        UpdatePoints(normalizedDistance, numberOfHits);
        SetSecureTimer();
    }

    void UpdateCombMult(float normalizedDistance)
    {
        float vel = UserData.Instance.droneVelocity.sqrMagnitude;
        float swerve = numberOfSwerveCombos * swerveMultiplier;
        float skims = numberOfSkimCombos * skimMultiplier;
        comboMultiplier = 1 + swerve + skims;
        UpdatedComboMultiplier.Invoke(comboMultiplier);
    }

    void UpdateNumberOfSkims(float normalizedDistance) 
    {
        numberOfSkimCombos++;

        if (numberOfSkimCombos == 1)
            return;
        UpdatedNumberOfSkims.Invoke(numberOfSkimCombos);
        UpdateCombMult(normalizedDistance);
    }

    void UpdateNumberOfSwerves(float normalizedDistance)
    {
        numberOfSwerveCombos++;
        UpdatedNumberOfSwerves.Invoke(numberOfSwerveCombos);
        UpdateCombMult(normalizedDistance);
    }

    public void Dashed(Vector2 dir, float duration) 
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
        float points = RunnignPointsCalculation(normalizedDistance, numberOfHits, UserData.Instance.droneVelocity.magnitude);
        runningPoints += points * comboMultiplier;
        expectedPoints = totalPoints + runningPoints;

        UpdatedRunningPoints.Invoke(runningPoints);
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
        numberOfSkimCombos = 0;
        numberOfSwerveCombos = 0;
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
    float RunnignPointsCalculation(float normalizedDistance, int numberOfHits, float velocity)
    {
        return  SpeedPoints(velocity, numberOfHits) * DistancePoints(normalizedDistance) * runningPointsMultiplier;
    }
    float DistancePoints(float normalizedDistance) 
    {
        return 1 + (normalizedDistance * normalizedDistance);
    }
    float SpeedPoints(float droneVelocity, int numberOfHits)
    {
        return 1 + (droneVelocity * numberOfHits * speedPointMultiplier);
    }
    #endregion

    public void ResetPointManager()
    {
        totalPoints = 0;
        runningPoints = 0;
        expectedPoints = 0;

        numberOfSkimCombos = 0;
        numberOfSwerveCombos = 0;

        comboMultiplier = 1;
        DestroyCourutineSafely(ref secureTimer);
        DestroyCourutineSafely(ref dashCoroutine);
    }


    void DestroyCourutineSafely(ref Coroutine Routine)
    {
        if (Routine != null)
            StopCoroutine(Routine);
        Routine = null;
    }
    
}
