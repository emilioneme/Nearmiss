using eneme;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;

public class SecureTimeUIHandler : MonoBehaviour
{
    [SerializeField] Image Image;
    [SerializeField] AnimationCurve fillCurve;

    Coroutine RunRoutine;

    private void Start()
    {
        ResetUI();
    }

    public void UpdateSecureTime(float SecureTime) 
    {
        this.StopSafely(ref RunRoutine);
        RunRoutine = StartCoroutine(RunCooldownCoroutine(SecureTime));
    }
   
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
            fill = secureNormalized < .1f ? 0 : secureNormalized;
            Image.fillAmount = fillCurve.Evaluate(Mathf.Abs(fill - 1));
            yield return null;
        }
        RunRoutine = null;
    }

    public void ResetUI() 
    {
        this.StopSafely(ref RunRoutine);
        Image.fillAmount = 0;
    }
    #endregion
}
