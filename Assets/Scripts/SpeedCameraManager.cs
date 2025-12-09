using UnityEngine;
using UnityEngine.UI;

public class SpeedCameraManager : MonoBehaviour
{
    [SerializeField] private Camera SpeedCamera;

    [SerializeField] private Transform Player;

    [SerializeField] private Vector3 positionOffset = new Vector3(-5, 0, 0);


    private void Update()
    {
        SpeedCamera.transform.position = Player.position + positionOffset;
    }


}
