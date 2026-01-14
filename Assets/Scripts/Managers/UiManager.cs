using eneme;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    #region Singleton
    public static UiManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

}
