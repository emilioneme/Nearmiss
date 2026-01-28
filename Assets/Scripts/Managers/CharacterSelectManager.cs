using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectManager : MonoBehaviour
{
    
    [SerializeField] DroneData[] droneData;

    int currentDrone = 0;
    [SerializeField] TMP_Text selecButtonText;

    [SerializeField] TMP_Text currentCharacterName;
    [SerializeField] Image currentCharacter;

    [SerializeField] Image leftCharacter;

    [SerializeField] Image rightCharacter;


    public void InitiateSelectionMenu() 
    {
        currentDrone = SelectedDroneID();
        UpdateSelectionMenu();
    }

    public void UpdateSelectionMenu()
    {
        //current
        if (SelectedDroneID() == currentDrone)
            selecButtonText.alpha = .5f;
        else
            selecButtonText.alpha = 1f;

        currentCharacter.sprite = droneData[currentDrone].DroneImage;
        currentCharacterName.text = droneData[currentDrone].DroneName;

        //left
        leftCharacter.sprite = droneData[LeftDrone()].DroneImage;

        //right
        rightCharacter.sprite = droneData[RightDrone()].DroneImage;
    }

    public void RightButton() 
    {
        currentDrone = RightDrone();
        UpdateSelectionMenu();
    }

    public void LeftButton()
    {
        currentDrone = LeftDrone();
        UpdateSelectionMenu();
    }

    int LeftDrone() 
    {
        return (currentDrone - 1 + droneData.Length) % droneData.Length;
    }

    int RightDrone()
    {
        return (currentDrone + 1) % droneData.Length;
    }

    public void SelectCharacter()
    {
        UserData.Instance.startDroneData = droneData[currentDrone];
    }

    int SelectedDroneID() 
    {
        if (UserData.Instance.startDroneData == null) 
        {
            Debug.LogWarning("DRONE DATA DOES NOT EXIST IN MENU LIST OH OH!!");
            return 0;
        }

        for (int i = 0; i < droneData.Length; i++) 
        {
            if (droneData[i] == UserData.Instance.startDroneData)
                return i;
        }

        Debug.LogWarning("DRONE DATA DOES NOT EXIST IN MENU LIST OH OH!!  Data:" + UserData.Instance.startDroneData);
        return 0;
    }

}
