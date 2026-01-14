using DG.Tweening;
using eneme;
using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField]
    GameObject PlayerUICanvas;

    [SerializeField]
    CinemachineCamera cam;

    [Header("Speedometer")]
    [SerializeField] float speedometerMultiplier = 10;

    [Header("UI")]
    [SerializeField] RectTransform Panel;
    [SerializeField] Transform camTransform;   // your camera (or Cinemachine brain output camera)
    [SerializeField] Transform droneTransform; // your plane

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

    //[Header("Screen Effcts")]

    [Header("Lag")]
    [SerializeField] float minLagSpeed = 20f;   // when close
    [SerializeField] float maxLagSpeed = 3f;    // when far
    [SerializeField] float maxLagDistance = 200f;
    [SerializeField] float distStrength = 1f;

    [Header("FOV")]
    [SerializeField] float baseFov = 60f;
    [SerializeField] float degreesPerSpeed = 1;
    [SerializeField] float maxFovOffset = 20f;
    [SerializeField] float fovSmoothSpeed = 8f;
    float currentFov;
    Coroutine RunRoutine;



    void Update()
    {
        UpdateFOV();
        UpdatePanelLag(AchnorePosition());
        UpdateSpeedometer();
    }

    #region Lag
    public Vector2 AchnorePosition() 
    {
        float diffX = camTransform.position.x - droneTransform.position.x;
        float diffY = camTransform.position.y - droneTransform.position.y;
        return new Vector2 (diffX, diffY) * distStrength;
    }
    public void UpdatePanelLag(Vector2 targetAnchoredPos)
    {
        Vector2 current = Panel.anchoredPosition;
        float distance = Vector2.Distance(current, targetAnchoredPos);
        // Normalize distance (0 = close, 1 = far)
        float t = Mathf.Clamp01(distance / maxLagDistance);

        // Invert so far = slower
        float followSpeed = Mathf.Lerp(minLagSpeed, maxLagSpeed, t);
        Panel.anchoredPosition = Vector2.Lerp(
            current,
            targetAnchoredPos,
            followSpeed * Time.unscaledDeltaTime
        );
    }

    #endregion

    #region  Run 
    public void RunStarted(float timeToSecure)
    {
        DestroyCourutineSafely(ref RunRoutine);
        RunRoutine = StartCoroutine(RunCooldownCoroutine(timeToSecure));

        ComboNumText.text = "";
        if (!ComboNumGO.activeSelf)
            ComboNumGO.SetActive(true);
        DOBounceTween(ref ComboNumGO, 1.3f, .25f);
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
        ComboNumGO.transform
            .DOScale(0f, .25f)
            .OnComplete(HideComboUI);
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
    void HideComboUI()
    {
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

    public void OnCrash(ControllerColliderHit hit) 
    {
        HideComboUI();
    }
    #endregion

    public void PlayerSpawned()
    {
        HideComboUI();
    }

    public void HidePlayerUI() 
    {
        PlayerUICanvas.SetActive(false);
    }

    public void UnhidePlayerUI()
    {
        PlayerUICanvas.SetActive(true);
    }

    #region tool
    public void DOBounceTween(ref GameObject GO, float toScale, float duration, Ease easeType = Ease.InOutSine)
    {
        GO.transform.DOKill();
        GO.transform.localScale = Vector3.one;
        GO.transform
            .DOScale(toScale, duration)   // go smaller
            .SetEase(easeType)
            .SetLoops(2, LoopType.Yoyo);
    }
    #endregion

}
