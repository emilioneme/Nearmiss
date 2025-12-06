using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    static GameObject Instance { get; set; }

    private void Awake()
    {
        if(Instance != null)
               Destroy(gameObject);

        Instance = this.gameObject;
    }
    #endregion

}
