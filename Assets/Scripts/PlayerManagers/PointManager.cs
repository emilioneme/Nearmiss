using eneme;
using System.Collections;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Events;

public class PointManager : MonoBehaviour
{
    [SerializeField] PointData pointData;

    [Header("Points")]
    public float startTotalPoints = 0;
    public float startRunningPoints = 0;
    public float startExpectedPoints = 0;
    public float startComboMultiplier = 1;

    float totalPoints = 0;
    float runningPoints = 0;
    float expectedPoints = 0;
    float comboMultiplier = 1;

    [Header("TypesOfCOmbo")]
    int numberOfSwerveCombos = 0;
    int numberOfSkimCombos = 0;


    #region  Multipliers
    [Header("Point Calculation")]
    public float runningPointsMultiplier = .5f;
    public float speedPointMultiplier = .5f;
    public float maxDistancePoints = .5f;

    [Header("Combo Calculation")]
    public float maxSwerveCombo = 2;
    public float maxSkimCombo = 1f;
    #endregion

    #region Expected Points

    [Header("Time For")] //time to secure point is minTimeBeforeCombo + comboWindowDuratio
    public float scureTimeDecrease = .25f;
    public float minSecureTime = 1;
    public float maxSecureTime = 2;
    float secureTime;
    int numberOfBreaks = 0;
    float timeToSkim = .3f;
    float lastNearmiss = 0;
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
    [SerializeField] UnityEvent<float, float, Vector3, RaycastHit> CalculatedRayPoints; //points, normal dist, origin, hit
    [SerializeField] UnityEvent<float> UpdatedRunningPoints; //running pioints
    [SerializeField] UnityEvent<float> UpdatedTotalPoints; //total points

    [Header("Combo")]
    [SerializeField] UnityEvent<float> UpdatedComboMultiplier; //combo mult
    [SerializeField] UnityEvent<float, RaycastHit> UpdatedNumberOfSkims;  //numberOfSkims, hit
    [SerializeField] UnityEvent<float, RaycastHit> UpdatedNumberOfSwerves; //swerves and hit

    [Header("HighScore")]
    [SerializeField] UnityEvent<float> NewHighScore;
    [SerializeField] UnityEvent<float> NewPersonalHighScore;

    [Header("Other")]
    [SerializeField] UnityEvent<float> ThrillThruster;
    [SerializeField] bool thrillOnSkimBreak = false;
    [SerializeField] bool thrillOnSkim = false;
    [SerializeField] bool thrillOnSwerve = false;
    [SerializeField] bool thrillOnSecure = false;
    [SerializeField] bool thrillOnCombo = false;
    #endregion

    Coroutine secureTimer;
    Coroutine dashCoroutine;
    Coroutine skimBreakCoroutie;

    private void Awake()
    {
        if (pointData == null) return;
        SetStartData(pointData);
    }
    public void SetStartData(PointData pointData) 
    {
        startTotalPoints = pointData.startTotalPoints;
        startRunningPoints = pointData.startRunningPoints;
        startExpectedPoints = pointData.startExpectedPoints;
        startComboMultiplier = pointData.startComboMultiplier;

        runningPointsMultiplier = pointData.runningPointsMultiplier;
        speedPointMultiplier = pointData.speedPointMultiplier;
        maxDistancePoints = pointData.maxDistancePoints;

        scureTimeDecrease = pointData.scureTimeDecrease;
        minSecureTime = pointData.minSecureTime;
        maxSecureTime = pointData.maxSecureTime;
    }

    private void Start()
    {
        ResetPointManager();
    }

    #region NearmissHandler
    public void OnNearmiss(float normalizedDistance, int numberOfHits, Vector3 origin, RaycastHit hit) //This is a float from 0 to 1
    {
        float timeDifferenceBetweenMisses = Time.time - lastNearmiss;
        float normalized = timeDifferenceBetweenMisses / secureTime;

        this.StopSafely(ref skimBreakCoroutie);
        skimBreakCoroutie = StartCoroutine(SkimBreakCourotine());

        if (normalized > timeToSkim)
            UpdateNumberOfSkims(normalizedDistance, hit);

        if (dashCoroutine != null) 
        {
            this.StopSafely(ref dashCoroutine);
            UpdateNumberOfSwerves(normalizedDistance, hit);
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
        UpdateSecureTime();

        if (thrillOnSkimBreak)
            ThrillThruster.Invoke(runningPoints);

        skimBreakCoroutie = null;
    }

    public void UpdateSecureTime() 
    {
        numberOfBreaks++;
        secureTime -= scureTimeDecrease;
        secureTime = Mathf.Clamp(secureTime, minSecureTime, maxSecureTime);
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

    void UpdateNumberOfSkims(float normalizedDistance, RaycastHit hit) 
    {
        numberOfSkimCombos++;
        if (numberOfSkimCombos == 1)
            return;
        UpdatedNumberOfSkims.Invoke(numberOfSkimCombos, hit);

        float multiplierAdded = normalizedDistance + .5f * maxSkimCombo;
        UpdateCombMult(multiplierAdded);

        if(thrillOnSkim)
            ThrillThruster.Invoke(runningPoints);
    }
   
    void UpdateNumberOfSwerves(float normalizedDistance, RaycastHit hit)
    {
        numberOfSwerveCombos++;
        UpdatedNumberOfSwerves.Invoke(numberOfSwerveCombos, hit);

        float multiplierAdded = normalizedDistance + .5f * maxSwerveCombo;
        UpdateCombMult(multiplierAdded);

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

    #region SecurePoints
    public float SecurePointsCooldown()
    {
        return eneme.Tools.CooldownSince(lastNearmiss, secureTime);
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
            RunStarted.Invoke(secureTime);
        else
            RunContinued.Invoke(secureTime);

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

    void ResetRunPoints()
    {
        runningPoints = startTotalPoints;
        numberOfSkimCombos = 0;
        numberOfSwerveCombos = 0;
        comboMultiplier = startComboMultiplier;
        numberOfBreaks = 0;

        secureTime = maxSecureTime;

        ResetSecureTimer();
    }

    public void ResetPointManager()
    {
        totalPoints = startTotalPoints;
        runningPoints = startRunningPoints;
        expectedPoints = startExpectedPoints;

        numberOfSkimCombos = 0;
        numberOfSwerveCombos = 0;

        numberOfBreaks = 0;
        secureTime = maxSecureTime;

        comboMultiplier = startComboMultiplier;

        this.StopSafely(ref secureTimer);
        this.StopSafely(ref dashCoroutine);
        this.StopSafely(ref skimBreakCoroutie);
    }
    
}
