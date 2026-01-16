using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField]
    [Range(-3f, 3f)]
    float airPitch = 1.0f;
    [SerializeField]
    float pitchDeltaMultiplier = .5f;
    [SerializeField]
    float maxVelocityChange = 3;

    [Header("Air")]
    [SerializeField] AudioSource AirSound;
    [Header("Dash")]
    [SerializeField] GameObject DashAudioPrefab;
    [Header("Secure")]
    [SerializeField] GameObject SecureAudioPrefab;
    [Header("Combo")]
    [SerializeField] GameObject SwerveAudioPrefab;
    [SerializeField] GameObject SkimAudioPrefab;
    [Header("Dead")]
    [SerializeField] GameObject RespawnAudioPrefab;
    [SerializeField] GameObject CrashAudioPrefab;
    [SerializeField] GameObject TickAudioPrefab;
    [Header("Wall")]
    [SerializeField] GameObject WallAudioPrefab;
    GameObject WallAudioSound;
    Coroutine wallCoroutine;

    private void Update()
    {
        if(AirSound.enabled)
            FlySound();
    }

    #region Wall
    public void PlayWallSound(float normalizedDistance, int numberOfHits, Vector3 playerPos, RaycastHit hit)
    {
        if(WallAudioSound == null)
            WallAudioSound = Instantiate(WallAudioPrefab, transform);
        DestoryCoroutneSafely(ref wallCoroutine);
        wallCoroutine = StartCoroutine(WallCoroutine());
    }

    IEnumerator WallCoroutine() 
    {
        yield return new WaitForSeconds(.25f);
        Destroy(WallAudioSound);
        wallCoroutine = null;
    }

    public void DestoryWallSund() 
    {
        DestoryCoroutneSafely(ref wallCoroutine);
        if(WallAudioSound != null)
            Destroy(WallAudioSound);
    }
    #endregion

    #region Dash
    public void PlayDashSound() 
    {
        Destroy(Instantiate(DashAudioPrefab, transform.position, Quaternion.identity), 1f);
    }
    #endregion

    #region Secure
    public void PlaySecureSound()
    {
        Destroy(Instantiate(SecureAudioPrefab, transform.position, Quaternion.identity), 1f);
        DestoryWallSund();
    }
    #endregion

    #region Combo
    public void PlaySkimSound()
    {
        Destroy(Instantiate(SkimAudioPrefab, transform.position, Quaternion.identity), 1f);
    }

    public void PlaySwerveSound()
    {
        Destroy(Instantiate(SwerveAudioPrefab, transform.position, Quaternion.identity), 1f);
    }
    #endregion

    #region Death
    public void PlayRespawnSound()
    {
        Destroy(Instantiate(RespawnAudioPrefab, transform.position, Quaternion.identity), 1f);
        DestoryWallSund();
    }

    public void PlayCrashSound()
    {
        Destroy(Instantiate(CrashAudioPrefab, transform.position, Quaternion.identity), 1f);
        DestoryWallSund();
    }
    #endregion

    #region FlySound

    void FlySound() 
    {
        AirSound.pitch = airPitch + NoseDiveSpeed() * pitchDeltaMultiplier;
    }

    public void ActivateFlySound() 
    {
        AirSound.enabled = true;
    }
    public void DeActivateFlySound()
    {
        AirSound.enabled = false;
    }

    float NoseDiveSpeed()
    {
        float dot = Vector3.Dot(transform.forward, Vector3.down);
        return Mathf.InverseLerp(-1f, 1f, dot);
    }

    #endregion

    #region Freeze SCreen
    public void PlayTickSound() 
    {
        Destroy(Instantiate(TickAudioPrefab, transform), .5f);
    }

    #endregion

    #region Tools

    void DestoryCoroutneSafely(ref Coroutine routine) 
    {
        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }
    }
    #endregion
}

