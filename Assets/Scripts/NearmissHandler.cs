using UnityEngine;
using UnityEngine.Events;

public class NearmissHandler : MonoBehaviour
{
    public UnityEvent<Collider> PlayerNearmissed;

    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Player Nearmissed Collided <color=yellow>: " + collider.gameObject.name);
        if (collider.gameObject.CompareTag("RigidObstacle"))
            PlayerNearmissed.Invoke(collider);
    }
}
