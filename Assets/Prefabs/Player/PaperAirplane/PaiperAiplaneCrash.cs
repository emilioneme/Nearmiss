using UnityEngine;

public class PaiperAiplaneCrash : MonoBehaviour
{
    [SerializeField]
    Rigidbody rb;
    void Start()
    {
        rb.AddExplosionForce(100, transform.position, 1);
    }
}
