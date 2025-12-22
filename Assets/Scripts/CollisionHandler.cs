using UnityEngine;
using UnityEngine.Events;

public class CollisionHandler : MonoBehaviour
{
    public UnityEvent<ControllerColliderHit> PlayerCrashed;

    public bool on;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!hit.gameObject.CompareTag("Player"))
            PlayerCrashed.Invoke(hit);
    }

}
