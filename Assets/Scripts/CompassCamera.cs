using UnityEngine;

public class CompassCamera : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform compass;
    [SerializeField] private Vector3 localOffset = new Vector3(0f, 0.2f, -2f);
    [SerializeField] private bool invert = false;

    void LateUpdate()
    {
        if (!player || !compass) return;

        Quaternion r = player.rotation;
        if (invert) r = Quaternion.Inverse(r);

        Vector3 worldOffset = r * localOffset;
        transform.position = compass.position + worldOffset;

        // IMPORTANT: roll-aware "up"
        transform.rotation = Quaternion.LookRotation(compass.position - transform.position, player.up);
    }
}
