using UnityEngine;
using UnityEngine.Events;

public class CollisionHandler : MonoBehaviour
{
    public UnityEvent<ControllerColliderHit> PlayerCrashed;
    bool crashed = false;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!hit.gameObject.CompareTag("Player") && !crashed) 
        {
            crashed = true;
            PlayerCrashed.Invoke(hit);
        }
    }

    private void OnEnable()
    {
        crashed = false;
    }


}
