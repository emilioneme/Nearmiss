using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextParticleEffect : MonoBehaviour
{
    [SerializeField]
    Canvas Canvas;

    [SerializeField]
    TMP_Text Text;

    [SerializeField]
    Image Image;

    [SerializeField]
    public Camera cam;

    [SerializeField]
    public Rigidbody rb;

    private void Start()
    {
        if (Image != null)
            SetImageFill(1);
    }

    public void SetText(string text) 
    {
        Text.text = text;
    }

    public void SetImageFill(float fill) 
    {
        Image.fillAmount = fill;
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
