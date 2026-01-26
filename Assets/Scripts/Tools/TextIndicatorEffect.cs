using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextIndicatorEffect : MonoBehaviour
{
    [SerializeField]
    Canvas Canvas;

    [SerializeField]
    public Camera cam;

    void LateUpdate()
    {
        if (!cam) return;
        transform.LookAt(
            transform.position + cam.transform.rotation * Vector3.forward,
            cam.transform.rotation * Vector3.up
        );
    }
}
