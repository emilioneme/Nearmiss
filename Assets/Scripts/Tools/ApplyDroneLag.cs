using UnityEngine;

public class ApplyDroneLag : MonoBehaviour
{
    [Header("Lag")]
    [SerializeField] RectTransform rt;
    [SerializeField] Transform camTransform;   // your camera (or Cinemachine brain output camera)
    [SerializeField] Transform droneTransform; // your plane
    [SerializeField] float minLagSpeed = 20f;   // when close
    [SerializeField] float maxLagSpeed = 3f;    // when far
    [SerializeField] float maxLagDistance = 200f;
    [SerializeField] float distStrength = 1f;

    void Update()
    {
        UpdatePanelLag(AchnorePosition());
    }

    #region Lag
    public Vector2 AchnorePosition()
    {
        float diffX = camTransform.position.x - droneTransform.position.x;
        float diffY = camTransform.position.y - droneTransform.position.y;
        return new Vector2(diffX, diffY) * distStrength;
    }
    public void UpdatePanelLag(Vector2 targetAnchoredPos)
    {
        Vector2 current = rt.anchoredPosition;
        float distance = Vector2.Distance(current, targetAnchoredPos);
        // Normalize distance (0 = close, 1 = far)
        float t = Mathf.Clamp01(distance / maxLagDistance);

        // Invert so far = slower
        float followSpeed = Mathf.Lerp(minLagSpeed, maxLagSpeed, t);
        rt.anchoredPosition = Vector2.Lerp(
            current,
            targetAnchoredPos,
            followSpeed * Time.unscaledDeltaTime
        );
    }

    public void ResetAnchor()
    {
        rt.anchoredPosition = Vector3.zero;
    }
    #endregion
}
