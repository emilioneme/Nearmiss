using System.Collections;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField] GameObject DashAudioPrefab;
    [SerializeField] GameObject SecureAudioPrefab;
    [SerializeField] GameObject SwerveAudioPrefab;
    [SerializeField] GameObject SkimAudioPrefab;
    [SerializeField] GameObject RespawnAudioPrefab;
    [SerializeField] GameObject CrashAudioPrefab;
    [SerializeField] GameObject WallAudioPrefab;
    GameObject WallAudioSound;
    Coroutine wallCoroutine;

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

    public void PlayDashSound() 
    {
        Destroy(Instantiate(DashAudioPrefab, transform.position, Quaternion.identity), 1f);
    }

    public void PlaySecureSound()
    {
        Destroy(Instantiate(SecureAudioPrefab, transform.position, Quaternion.identity), 1f);
        DestoryWallSund();
    }

    public void PlaySkimSound()
    {
        Destroy(Instantiate(SkimAudioPrefab, transform.position, Quaternion.identity), 1f);
    }

    public void PlaySwerveSound()
    {
        Destroy(Instantiate(SwerveAudioPrefab, transform.position, Quaternion.identity), 1f);
    }

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


    void DestoryCoroutneSafely(ref Coroutine routine) 
    {
        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }
    }
}
