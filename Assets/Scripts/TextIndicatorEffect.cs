using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextIndicatorEffect : MonoBehaviour
{
    [SerializeField]
    Canvas Canvas;

    [SerializeField]
    TMP_Text RunText;

    [SerializeField]
    public Camera cam;

    public void SetText(string text)
    {
        RunText.text = text;
    }

    void LateUpdate()
    {
        if (!cam) return;
        transform.LookAt(
            transform.position + cam.transform.rotation * Vector3.forward,
            cam.transform.rotation * Vector3.up
        );
    }
}
