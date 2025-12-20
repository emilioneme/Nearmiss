using UnityEngine;

public class CompassTransformation : MonoBehaviour
{
    void Update()
    {
        transform.rotation = Quaternion.Euler(Vector3.zero);    
    }
}
