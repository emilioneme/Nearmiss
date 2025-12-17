using System.Collections;
using UnityEngine;

public class PlayerModelHandler : MonoBehaviour
{
    public PlayerModelVisuals PlayerModelVisuals;

    private void Awake()
    {
        PlayerModelVisuals = GetComponentInChildren<PlayerModelVisuals>();
        if (PlayerModelVisuals == null)
            Debug.LogWarning("No Player Model Visuals Found");
    }

    #region Dash
    Coroutine dashSpinRoutine;
    public void AnimateDash(int direction, Vector3 animationAxis, float duration)
    {
        if (dashSpinRoutine != null) StopCoroutine(dashSpinRoutine);
        dashSpinRoutine = StartCoroutine(AnimateDashCoroutine(direction, animationAxis, duration));
    }

    IEnumerator AnimateDashCoroutine(int direction, Vector3 animationAxis, float duration)
    {
        Quaternion start = transform.localRotation;

        float timer = 0f;
        while (timer < duration)
        {
            float t = timer / duration;

            float angle = 360f * t * Mathf.Sign(direction);
            transform.localRotation = start * Quaternion.Euler(animationAxis.x * angle, animationAxis.y * angle, animationAxis.z * angle);

            timer += Time.deltaTime;
            yield return null;
        }
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        dashSpinRoutine = null;
    }
    #endregion
}
