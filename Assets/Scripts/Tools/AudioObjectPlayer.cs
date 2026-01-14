using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class AudioObjectPlayer : MonoBehaviour
{
    [SerializeField]
    float pitchRandomizerOffset;

    float startingPitch;
    private AudioSource AudioSource;

    [SerializeField]
    private AudioClip[] AudioClips;

    [SerializeField]
    bool loop;
    [SerializeField]
    bool automaticDestroy = false;
    [SerializeField]
    bool playOnStart;
    [SerializeField]
    bool playOnEnable;

    private float lastAudioTime = 0f; // Track the last known play time

    void Awake()
    {
        AudioSource = GetComponent<AudioSource>();
        startingPitch = AudioSource.pitch;
        AudioSource.playOnAwake = playOnStart;
        AudioSource.loop = loop;
        RandomizeSound();
    }

    void Start()
    {
        if (playOnStart)
            PlaySound();
    }
    void OnEnable()
    {
        if (playOnEnable)
            PlaySound();
    }

    void Update()
    {
        if (AudioSource.isPlaying)
            return;

        if (AudioSource.time < lastAudioTime)
            if (AudioSource.loop)
                PlaySound();
            else if (automaticDestroy)
                Destroy(this.gameObject);
    }

    public void PlaySound()
    {
        AudioSource.loop = loop;
        lastAudioTime = AudioSource.time;
        RandomizeSound();
        AudioSource.Play();
    }

    void RandomizeSound()
    {
        AudioSource.clip = AudioClips[Random.Range(0, AudioClips.Length)];
        float randomPitch = Random.Range(startingPitch - pitchRandomizerOffset, startingPitch + pitchRandomizerOffset);
        AudioSource.pitch = Mathf.Clamp(randomPitch, .1f, 3f);
    }
}