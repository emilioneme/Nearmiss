using UnityEngine;
using UnityEngine.Events;

public class CollisionHandler : MonoBehaviour
{
    public UnityEvent<ControllerColliderHit> PlayerCrashed;
    bool crashed = false;

    [SerializeField] LayerMask layerMask;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Crash(hit);
    }

    public void Crash(ControllerColliderHit hit) 
    {
        if (crashed) return;
        int hitLayer = hit.collider.gameObject.layer;
        if (((1 << hitLayer) & layerMask) != 0)
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
