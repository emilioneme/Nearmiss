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
    public Camera cam;

    [SerializeField]
    public Rigidbody rb;

    public Coroutine lifeRoutine;


    public void SetText(string text) 
    {
        Text.text = text;
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
