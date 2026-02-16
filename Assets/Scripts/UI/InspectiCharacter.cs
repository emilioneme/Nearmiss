using UnityEngine;

public class InspectiCharacter : MonoBehaviour
{

    [SerializeField] float speed = 100f;

    bool allowRotate = true;

    //Rigidbody rb;
    private void Awake()
    {
        //rb = GetComponent<Rigidbody>();
    }

    public void RotatePivot(Vector2 input) 
    {
        if (!allowRotate) return;

        Vector2 newInput = new Vector2(input.y * Time.deltaTime * speed, -input.x * Time.deltaTime * speed);

        transform.Rotate(newInput);

        //rb.AddRelativeTorque(newInput);
        //rb.maxAngularVelocity = maxAngularVel;
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
