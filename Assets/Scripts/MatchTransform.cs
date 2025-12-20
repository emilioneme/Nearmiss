using UnityEngine;

public class MatchTransform : MonoBehaviour
{
    [SerializeField] private Transform toMatch;
    [SerializeField] private Vector3 positionOffset;
    private void Update()
    {
        this.transform.position = toMatch.position + positionOffset;
    }
}
