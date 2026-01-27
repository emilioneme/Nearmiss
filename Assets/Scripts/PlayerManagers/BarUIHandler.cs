using UnityEngine;
using UnityEngine.UI;

public class BarUIHandler : MonoBehaviour
{
    [SerializeField] GameObject BarGO;
    [SerializeField] Image Image;

    public void UpdateFill(float amount) 
    {
        Image.fillAmount = amount;
    }

}
