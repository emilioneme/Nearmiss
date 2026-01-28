using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PlayerModelHandler : MonoBehaviour
{
    [SerializeField]
    public GameObject PlayerModelPrefab;

    [HideInInspector]
    public GameObject PlayerModelGO;
    [HideInInspector]
    public PlayerModelContainer PlayerModelContainer;

    [Header("Dash Amimation")]
    [SerializeField]
    int numberOfFlips = 1;
    [SerializeField]
    AnimationCurve dashRotationSpeed;

    [Header("Event")]
    public UnityEvent<GameObject> SpawnedCrashObject;

    #region Instantiate
    public void SetPlayerModelVisual(GameObject newPlayerModelPrefab)
    {
        if(PlayerModelGO != null)
            Destroy(PlayerModelGO);

        PlayerModelPrefab = newPlayerModelPrefab;
    }

    public void InitiatePlayerModel() 
    {
        if (PlayerModelGO == null)
            PlayerModelGO = Instantiate(PlayerModelPrefab, transform);

        PlayerModelContainer = PlayerModelGO.GetComponent<PlayerModelContainer>();
    }
    #endregion

    #region Dash
    Coroutine dashSpinRoutine;
    public void AnimateDash(Vector2 direction, float duration)
    {
        if (dashSpinRoutine != null) StopCoroutine(dashSpinRoutine);
        dashSpinRoutine = StartCoroutine(AnimateDashCoroutine(direction, duration));
    }

    IEnumerator AnimateDashCoroutine(Vector3 direction, float duration)
    {
        Quaternion start = transform.localRotation;
        float timer = 0f;
        while (timer < duration)
        {
            float t = timer / duration;
            float angle = 360f * numberOfFlips * t;

            Quaternion rotationForX = Quaternion.identity;
            Quaternion rotationForY = Quaternion.identity;
            if(direction.x != 0) 
                rotationForX = Quaternion.Euler(0, 0, angle * -Mathf.Sign(direction.x));
            else
                rotationForX = Quaternion.Euler(0, 0, angle * -Mathf.Sign(direction.x));

            transform.localRotation = start * rotationForX * rotationForY;

            timer += Time.deltaTime;
            yield return null;
        }
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        dashSpinRoutine = null;
    }
    #endregion

    void SpawnCrashModel()
    {
        GameObject CrashObject = Instantiate
                    (PlayerModelContainer.CrashModelPrefab, this.transform.position, Quaternion.identity);
        SpawnedCrashObject.Invoke(CrashObject);
        Destroy(CrashObject, 10f);
    }

    public void ResetPlayerModel() 
    {
        SpawnCrashModel();
        transform.localRotation = Quaternion.Euler(Vector3.zero); //fixing its rotation before spawnign in cse player dashes and dies
    }

}
