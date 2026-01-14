using UnityEngine;

public class MatchRotation : MonoBehaviour
{
    [SerializeField] private Transform toMatchRotation;
    [SerializeField] private Vector3 RotationOffset;

    private void Update()
    {
        transform.rotation = toMatchRotation.rotation * Quaternion.Euler(RotationOffset);
    }
}
