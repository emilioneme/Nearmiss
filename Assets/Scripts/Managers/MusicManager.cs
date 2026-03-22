using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    #region Singleton
    public static MusicManager Instance;
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

    [SerializeField] float menuPitch = 1;
    [SerializeField] float gamePitch = 1.1f;
    [SerializeField] float pausePitch = .8f;
    [SerializeField] float crashPitch = .75f;

    [SerializeField]
    AudioSource Music;

    private void Start()
    {
        Music.volume = UserData.Instance.musicVolume;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void LerpPitch(float pitch, float speed = 1) 
    {
        Music.DOPitch(pitch, speed).SetUpdate(true);
    }

    public void onPause() 
    {
        LerpPitch(pausePitch);
    }

    public void onUnPause()
    {
        LerpPitch(gamePitch);
    }

    public void onCrash()
    {
        LerpPitch(crashPitch, .5f);
    }

    public void onSpawn()
    {
        LerpPitch(gamePitch, .5f);
    }

    public void onMusicVolumeChange(float vol) 
    {
        Music.volume = vol;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Music.volume = UserData.Instance.musicVolume;
        UserData.Instance.MusicVolumeChange.AddListener(onMusicVolumeChange);

        if(PauseManager.Instance) PauseManager.Instance.OnPause.AddListener(onPause);
        if(PauseManager.Instance) PauseManager.Instance.OnUnpause.AddListener(onUnPause);

        if (scene.name == "MenuScene") LerpPitch(menuPitch);
        else LerpPitch(gamePitch);
       
    }


}
