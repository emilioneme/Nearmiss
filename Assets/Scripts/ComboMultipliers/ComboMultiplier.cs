using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ComboMultiplier : MonoBehaviour
{
    public float comboMultiplier = 1f;

    [SerializeField] bool randomMultiplier = false;
    [SerializeField] float minMultiplier = 1.1f;
    [SerializeField] float maxMultiplier = 2f;

    [SerializeField] GameObject popSound;
    Collider c;

    private void Awake()
    {
        c = GetComponent<Collider>();
    }

    void Start()
    {
        if(randomMultiplier)
            comboMultiplier = (int)Random.Range(minMultiplier, maxMultiplier);

        transform
            .DOScale(.5f, .5f)
            .SetRelative()
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void InitiateDestroy() 
    {
        c.enabled = false;
        Instantiate(popSound, transform.position, Quaternion.identity, null);
        transform.DOKill();
        transform
            .DOScale(0, .15f)
            .SetRelative()
            .OnComplete(() => Destroy(gameObject));
    }

    bool consumed = false;
    private void OnTriggerEnter(Collider other)
    {
        if (consumed) return;

        if (other.TryGetComponent(out PointManager playerPoints))
        {
            consumed = true;     
            playerPoints.AddComboMultiplier((float)comboMultiplier);
            InitiateDestroy();
        }
    }

}
