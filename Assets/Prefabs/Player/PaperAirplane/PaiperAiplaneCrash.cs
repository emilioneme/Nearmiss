using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class PaiperAiplaneCrash : MonoBehaviour
{
    [Header("CRashModel")]
    [SerializeField]
    float lifeTime = 10f;

    [Header("rigidody")]
    [SerializeField]
    float innitialForceStrenght = 1.0f;

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
        rb.AddForce(force * innitialForceStrenght, ForceMode.Impulse);
        rb.AddTorque(force * innitialForceStrenght, ForceMode.Impulse);
    }

    private void OnDestroy()
    {
        Destroy(Instantiate(destroyParticles, transform), destroyParticlesLifetime);
    }
}
