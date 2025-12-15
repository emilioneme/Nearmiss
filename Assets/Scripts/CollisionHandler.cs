using UnityEngine;
using UnityEngine.Events;

public class CollisionHandler : MonoBehaviour
{
    public UnityEvent<ControllerColliderHit> PlayerCrashed;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("RigidObstacle"))
            PlayerCrashed.Invoke(hit);
    }

}
