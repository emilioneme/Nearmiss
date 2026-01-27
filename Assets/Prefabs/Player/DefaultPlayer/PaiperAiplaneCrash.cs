using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class PaiperAiplaneCrash : MonoBehaviour
{
    [Header("CRashModel")]
    [SerializeField]
    float lifeTime = 10f;

    [Header("rigidody")]
    [SerializeField]
    float innitialForceStrength = 1.0f;
    float innitialRotationStrength = 1.0f;

    [Header("onCRash")]
    [SerializeField]
    float crashParticlesLifetime = 2f;
    [SerializeField]
    GameObject crashParticles;

    [Header("onDestroy")]
    [SerializeField]
    float destroyParticlesLifetime = 10f;
    [SerializeField]
    GameObject destroyParticles;

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>(); 
    }

    void Start()
    {
        Destroy(this.transform, lifeTime);
        Destroy(Instantiate(crashParticles, transform), crashParticlesLifetime);
        Quaternion randomRotation = Random.rotation;
        Vector3 force = randomRotation.eulerAngles.normalized;
        rb.AddForce(force * innitialForceStrength, ForceMode.Impulse);
        rb.AddTorque(force * innitialRotationStrength, ForceMode.Impulse);
    }

    private void OnDestroy()
    {
        Destroy(Instantiate(destroyParticles, transform.position, Quaternion.identity), destroyParticlesLifetime);
    }
}
