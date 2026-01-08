using UnityEngine;

public class CrashCamera : MonoBehaviour
{
    public GameObject LookAt;

    public void Update()
    {
        if(LookAt != null)
            this.transform.LookAt(LookAt.transform);
    }

    public void SetLookAt(GameObject LookAtObject) 
    {
        LookAt = LookAtObject;
    }
}
