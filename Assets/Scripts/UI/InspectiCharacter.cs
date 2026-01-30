using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class InspectiCharacter : MonoBehaviour
{

    float speed = 1f;

    [SerializeField] float maxAngularVel = 1;

    bool allowRotate = true;

    Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void RotatePivot(Vector2 input) 
    {
        if (!allowRotate) return;

        Vector2 newInput = new Vector2(input.y, -input.x);

        rb.AddRelativeTorque(newInput);
        rb.maxAngularVelocity = maxAngularVel;
    }

    public void CanRotate() 
    {
        allowRotate = true;
    }

    public void CannotRotate()
    {
        allowRotate = false;
    }

}
