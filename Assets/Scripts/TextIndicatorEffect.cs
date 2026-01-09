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
    Image CircleImage;

    [SerializeField]
    GameObject ComboObject;

    [SerializeField]
    TMP_Text ComboText;

    [SerializeField]
    public Camera cam;

    private void Start()
    {
        if (CircleImage != null)
            SetImageFill(1);
    }

    public void SetText(string text)
    {
        RunText.text = text;
    }

    public void SetComboText(string text)
    {
        ComboText.text = text;
    }

    public void HideComboIndicator() 
    {
        ComboObject.SetActive(false);
    }

    public void ShowComboIndicator()
    {
        ComboObject.SetActive(true);
    }

    public void SetImageFill(float fill)
    {
        CircleImage.fillAmount = fill;
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
