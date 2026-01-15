using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class FreezeScreenManager : MonoBehaviour
{
    [SerializeField] GameObject Canvas;
    [SerializeField] Transform CountGO;
    [SerializeField] TMP_Text CountDownText;
    string text;

    public void FreezeStarted(float duration) 
    {
        Canvas.SetActive(true);
        CountGO.localScale = Vector3.zero;
        CountGO.transform.DOScale(1, .25f);
        StartCoroutine(FreezeRoutine(duration));
    }

    IEnumerator FreezeRoutine(float duration) 
    {
        float countDown = duration;
        while (0 < countDown) 
        {
            countDown -= Time.deltaTime;
            float rounded = Mathf.Round(countDown * 10) / 10;
            int interger = Mathf.RoundToInt(countDown);

            if (countDown > 2)
                CountDownText.text = interger.ToString();
            else
                CountDownText.text = rounded.ToString();
            yield return null;
        }
        FreezeEnded();
    }

    void FreezeEnded()
    {
        CountGO.transform.DOScale(0, .25f);
        Canvas.SetActive(false);
        CountGO.localScale = Vector3.zero;
    }
}
