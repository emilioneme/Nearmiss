using eneme;
using System.Collections;
using Unity.Burst;
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
    [Header("Secure")]
    [SerializeField]
    UnityEvent<float> ScuredPoints; //total points

    [Header("Run")]
    [SerializeField]
    UnityEvent<float> RunStarted; //secure duration
    [SerializeField]
    UnityEvent<float> RunContinued; //secure duration

    [Header("Skim")]
    [SerializeField]
    UnityEvent SkimBreak; //points

    [Header("Points")]
    [SerializeField]
    UnityEvent<float, float, Vector3, RaycastHit> CalculatedRayPoints; //points, normal dist, origin, hit
    [SerializeField]
    UnityEvent<float> UpdatedRunningPoints; //running pioints
    [SerializeField]
    UnityEvent<float> UpdatedTotalPoints; //total points

    [Header("Combo")]
    [SerializeField]
    UnityEvent<float> UpdatedComboMultiplier; //combo mult
    [SerializeField]
    UnityEvent<float, RaycastHit> UpdatedNumberOfSkims;  //numberOfSkims, hit
    [SerializeField]
    UnityEvent<float, RaycastHit> UpdatedNumberOfSwerves; //swerves and hit

    [Header("HighScore")]
    [SerializeField]
    UnityEvent<float> NewHighScore;
    [SerializeField]
    UnityEvent<float> NewPersonalHighScore;

    [Header("Other")]
    [SerializeField]
    UnityEvent<float> ThrillThruster;
    [SerializeField] bool thrillOnSkimBreak = false;
    [SerializeField] bool thrillOnSkim = false;
    [SerializeField] bool thrillOnSwerve = false;
    [SerializeField] bool thrillOnSecure = false;
    [SerializeField] bool thrillOnCombo = false;
    #endregion

    Coroutine secureTimer;
    Coroutine dashCoroutine;
    Coroutine skimBreakCoroutie;


    #region NearmissHandler
    public void OnNearmiss(float normalizedDistance, int numberOfHits, Vector3 origin, RaycastHit hit) //This is a float from 0 to 1
    {
        float timeDifferenceBetweenMisses = Time.time - lastNearmiss;
        float normalized = timeDifferenceBetweenMisses / timeToSecurePoints;

        this.StopSafely(ref skimBreakCoroutie);
        skimBreakCoroutie = StartCoroutine(SkimBreakCourotine());

        if (normalized > timeToSkim)
            UpdateNumberOfSkims(hit);

        if (dashCoroutine != null) 
        {
            this.StopSafely(ref dashCoroutine);
            UpdateNumberOfSwerves(hit);
        }

        lastNearmiss = Time.time;
        UpdatePoints(normalizedDistance, numberOfHits, origin, hit);
        SetSecureTimer();
    }
    #endregion

    #region Skim Done Update
    IEnumerator SkimBreakCourotine() 
    {
        yield return new WaitForSeconds(timeToSkim);
        SkimBreak.Invoke();

        if(thrillOnSkimBreak)
            ThrillThruster.Invoke(runningPoints);

        skimBreakCoroutie = null;
    }

    #endregion

    #region Combos
    public void AddComboMultiplier(float amount) 
    {
        UpdateCombMult(amount);
    }

    void UpdateCombMult(float amount)
    {
        comboMultiplier += amount;
        UpdatedComboMultiplier.Invoke(comboMultiplier);

        if (thrillOnCombo)
            ThrillThruster.Invoke(runningPoints);
    }

    void UpdateNumberOfSkims(RaycastHit hit) 
    {
        numberOfSkimCombos++;

        if (numberOfSkimCombos == 1)
            return;

        UpdatedNumberOfSkims.Invoke(numberOfSkimCombos, hit);

        UpdateCombMult(numberOfSkimCombos * skimMultiplier);

        if(thrillOnSkim)
            ThrillThruster.Invoke(runningPoints);
    }
   
    void UpdateNumberOfSwerves(RaycastHit hit)
    {
        numberOfSwerveCombos++;

        UpdatedNumberOfSwerves.Invoke(numberOfSwerveCombos, hit);
        UpdateCombMult(numberOfSwerveCombos * swerveMultiplier);

        if (thrillOnSwerve)
            ThrillThruster.Invoke(runningPoints);
    }
    #endregion

    #region Dash
    public void Dashed(Vector2 dir, float duration) 
    {
        if (dashCoroutine == null)
            this.StopSafely(ref dashCoroutine);
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
        this.StopSafely(ref dashCoroutine);  
        dashCoroutine = null;
    }
    #endregion

    #region Points
    void UpdatePoints(float normalizedDistance, int numberOfHits, Vector3 origin, RaycastHit hit)
    {
        float points = RunnignPointsCalculation(normalizedDistance, numberOfHits, UserData.Instance.droneVelocity.magnitude);

        CalculatedRayPoints.Invoke(points * comboMultiplier, normalizedDistance, origin, hit);

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
        if (thrillOnSecure)
            ThrillThruster.Invoke(runningPoints);
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
            this.StopSafely(ref secureTimer);
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
        comboMultiplier = 1;

        ResetSecureTimer();
    }
    #endregion

    #region HighScore
    void HighSchoreCheck()
    {
        //if (GameManager.Instance.highScore >= totalPoints)
            return;

        //NewHighScore.Invoke(totalPoints);
        //GameManager.Instance.highScore = totalPoints;
        //GameManager.Instance.highScorer = UserData.Instance.UserName;
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

        this.StopSafely(ref secureTimer);
        this.StopSafely(ref dashCoroutine);
        this.StopSafely(ref skimBreakCoroutie);
    }
    
}
