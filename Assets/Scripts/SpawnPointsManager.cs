using UnityEngine;

public class SpawnPointsManager : MonoBehaviour
{
    #region Singleton
    public static SpawnPointsManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    [SerializeField]
    public Transform spawnPointParent;

    [HideInInspector]
    public Transform[] spawnPoints;

    private void Start() 
    {
        spawnPoints = spawnPointParent.GetComponentsInChildren<Transform>();
    }
}
