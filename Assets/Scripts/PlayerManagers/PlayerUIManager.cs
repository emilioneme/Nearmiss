using DG.Tweening;
using eneme;
using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField]
    GameObject PlayerUICanvas;

    [SerializeField]
    CinemachineCamera cam;

    [Header("Speedometer")]
    [SerializeField] float speedometerMultiplier = 10;

    [Header("UI")]
    [SerializeField] GameObject Panel;

    [SerializeField] TMP_Text SpeedText;

    [Header("Hame Objects")]
    [SerializeField] GameObject RunningPointsGO;
    [SerializeField] GameObject ComboNumGO;
    [SerializeField] GameObject TotalPointsGO;

    [Header("Texts")]
    [SerializeField] TMP_Text TotalPointsText;
    [SerializeField] TMP_Text RunningPointsText;
    [SerializeField] TMP_Text ComboNumText;

    [Header("Circles")]
    [SerializeField] Image TotalPointsImage;
    [SerializeField] Image RunningPointsImage;
    [SerializeField] Image ComboNumImage;

    [SerializeField] AnimationCurve CircleFillCurve;

    [Header("Screen Effcts")]

    [Header("Shake")]
    [SerializeField] 
    float maxStrenghShake = 30f;
    [SerializeField]
    float durationShake = 0.5f;
    [SerializeField]
    int vibratoShake = 10;
    [SerializeField]
    float randomnessShake = 90;
    [SerializeField]
    bool fadeOutShake = false;
    [SerializeField]
    float minVelocityShake = 0;
    [SerializeField]
    float maxVelocityShake = 100;


    [Header("FOV")]
    [SerializeField] float baseFov = 60f;
    // How many degrees of FOV you get per 1 m/s above the running average.
    [SerializeField] float degreesPerSpeed = 1;
    // Clamp the effect so it doesn’t go crazy.
    [SerializeField] float maxFovOffset = 20f;
    // Smooth the actual camera FOV change.
    [SerializeField] float fovSmoothSpeed = 8f;
    float currentFov;
    Coroutine RunRoutine;
    Tween shakeTween;


    void Update()
    {
        UpdateFOV();
        UpdateSpeedometer();
        UpdatePanelShake();
    }

    #region SHake
    void UpdatePanelShake() 
    {
        float strengthShake = GetShakeStrength();
        if (strengthShake > 0.1f)
        {
            Panel.transform.localPosition = Vector3.zero;
            shakeTween?.Kill();
            shakeTween = Panel.transform.DOShakePosition(
                duration: durationShake,
                strength: strengthShake, // start with no shake
                vibrato: vibratoShake,
                randomness: randomnessShake,
                fadeOut: fadeOutShake
            )
            .SetLoops(-1)
            .SetUpdate(true);
        }
        else
        {
            StopShake();
        }
    }

    void StopShake()
    {
        shakeTween?.Kill();
        shakeTween = null;
        Panel.transform.localPosition = Vector3.zero;
    }

    float GetShakeStrength()
    {
        float v = UserData.Instance.droneVelocity.magnitude;
        float normlized = Mathf.InverseLerp(minVelocityShake, maxVelocityShake, v);
        return normlized * maxStrenghShake;
    }
    #endregion

    #region  Run 
    public void RunStarted(float timeToSecure)
    {
        DestroyCourutineSafely(ref RunRoutine);
        RunRoutine = StartCoroutine(RunCooldownCoroutine(timeToSecure));
    }

    public void RunContinued(float timeToSecure)
    {
        DestroyCourutineSafely(ref RunRoutine);
        RunRoutine = StartCoroutine(RunCooldownCoroutine(timeToSecure));
    }

    public void PointsSecured(float points)
    {
        TotalPointsText.text = Tools.ProcessFloat(points, 2);

        DOBounceTween(ref TotalPointsGO, .5f, .25f);
    }

    void DestroyCourutineSafely(ref Coroutine Routine) 
    {
        if (Routine != null)
            StopCoroutine(Routine);
        Routine = null;
    }
    #endregion

    #region RunCooldown
    IEnumerator RunCooldownCoroutine(float timeToSecure) 
    {
        float timeLapsed = 0;
        float secureNormalized = 0;
        float fill = 0;
        while (timeLapsed < timeToSecure) 
        {
            timeLapsed += Time.deltaTime;
            secureNormalized = timeLapsed / timeToSecure;
            fill = secureNormalized < .1f? 0 : secureNormalized;
            //RunningPointsImage.fillAmount = Mathf.Abs(fill - 1);
            ComboNumImage.fillAmount = CircleFillCurve.Evaluate(Mathf.Abs(fill - 1));
            yield return null;
        }
        RunRoutine = null;
    }
    #endregion

    #region Run Points
    public void UpdateRunPoints(float points)
    {
        //RunningPointsText.text = Tools.ProcessFloat(points, 2);
        if (!RunningPointsGO.activeSelf)
            //RunningPointsGO.SetActive(true);
        if (!ComboNumGO.activeSelf)
            ComboNumGO.SetActive(true);
    }

    public void ResetedRunPoints(float points)
    {
        //RunningPointsText.text = Tools.ProcessFloat(points, 2);
        RunningPointsGO.SetActive(false);
        ComboNumGO.SetActive(false);
    }
    #endregion

    #region Combos
    public void UpdateComboMult(float comboMult)
    {
        if (comboMult > 1)
        {
            ComboNumText.text = "x" + Tools.LimitNumberLength(comboMult, 4);
            DOBounceTween(ref ComboNumGO, .5f, .25f);
            DOBounceTween(ref TotalPointsGO, .9f, .25f);
        }
        else 
        {
            ComboNumText.text = " ";
        }
            
    }

    public void ResetedNumberOfCombo(float numberOfCombos)
    {
        ComboNumText.text = " ";
    }
    #endregion

    #region Totalpoints
    public void UpdateTotalPoints(float points)
    {
        TotalPointsText.text = Tools.ProcessFloat(points, 2);
    }

    public void ResetedTotalPoints(float points)
    {
        TotalPointsText.text = Tools.ProcessFloat(points, 2);
    }
    #endregion

    #region Speedotemeter
    public void UpdateSpeedometer()
    {
        SpeedText.text = Tools.ProcessFloat(UserData.Instance.droneVelocity.magnitude * speedometerMultiplier, 3);
    }

    //public void UpdateFOV()
    //{
    //  float speedFOV = minFov + Mathf.InverseLerp(minVelocityFOV, maxVelocityFOV, UserData.Instance.droneVelocity.magnitude) * maxFovAdditive;
    //  cam.Lens.FieldOfView = speedFOV;
    // }

    public void UpdateFOV()
    {
        float targetOffset = Mathf.Clamp(UserData.Instance.deltaVelocity * degreesPerSpeed, -maxFovOffset, maxFovOffset);
        float targetFov = baseFov + targetOffset;
        float fovT = 1f - Mathf.Exp(-fovSmoothSpeed * Time.deltaTime);
        currentFov = Mathf.Lerp(currentFov == 0 ? baseFov : currentFov, targetFov, fovT);
        cam.Lens.FieldOfView = currentFov;
    }
    #endregion

    #region CrashUI

    public void TriggerCrashUI(ControllerColliderHit hit) 
    {
    }
    #endregion

    public void DOBounceTween(ref GameObject GO, float toScale, float duration, Ease easeType = Ease.InOutSine) 
    {
        GO.transform.DOKill();
        GO.transform.localScale = Vector3.one;
        GO.transform
            .DOScale(toScale, duration)   // go smaller
            .SetEase(easeType)
            .SetLoops(2, LoopType.Yoyo);
    }

    public void PlayerSpawned() 
    {
        RunningPointsGO.SetActive(false);
        ComboNumGO.SetActive(false);
    }

    public void HidePlayerUI() 
    {
        PlayerUICanvas.SetActive(false);
    }

    public void UnhidePlayerUI()
    {
        PlayerUICanvas.SetActive(true);
    }
    
}
