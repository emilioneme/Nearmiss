using UnityEngine;

public class SprintSound : MonoBehaviour
{
    [SerializeField] AudioSource auidoSource;
    [SerializeField] DroneMovement droneMovement;
    [SerializeField] float maxVolume = .75f;

    private void Update()
    {
        auidoSource.volume = droneMovement.sprint * maxVolume;
    }

}
