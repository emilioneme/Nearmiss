using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectManager : MonoBehaviour
{
    [Header("Characters")]
    [SerializeField] DroneData[] droneData;
    int currentDrone = 0;

    [Header("RectTransforms")]
    [SerializeField] GameObject currentRT;
    [SerializeField] GameObject rightRT;
    [SerializeField] GameObject leftRT;

    [Header("Text")]
    [SerializeField] TMP_Text selecButtonText;
    [SerializeField] TMP_Text currentCharacterName;

    [Header("Buttons")]
    [SerializeField] Button leftButton;
    [SerializeField] Button rightButton;
    [SerializeField] Button selectButton;

    [Header("Pivots")]
    [SerializeField] GameObject selectCharacterPivot;

    [SerializeField] GameObject currentCharacterPivot;
    [SerializeField] GameObject leftCharacterPivot;
    [SerializeField] GameObject rightCharacterPivot;

    private void Start()
    {
        InitiateSelectionMenu();
    }

    public void InitiateSelectionMenu() 
    {
        currentDrone = SelectedDroneID();
        UpdateSelectionMenu();
    }

    public void UpdateSelectionMenu()
    {
        //current
        if (SelectedDroneID() == currentDrone)
            selecButtonText.alpha = .1f;
        else
            selecButtonText.alpha = 1f;

        //Select
        ReplaceMesh(ref selectCharacterPivot, SelectedDroneID());

        //current
        currentCharacterName.text = droneData[currentDrone].DroneName;
        ReplaceMesh(ref currentCharacterPivot, currentDrone);

        //left
        ReplaceMesh(ref leftCharacterPivot, LeftDrone());

        //right
        ReplaceMesh(ref rightCharacterPivot, RightDrone());
    }

    void ReplaceMesh(ref GameObject pivot, int i) 
    {
        if (pivot.transform.childCount > 0)
            Destroy(pivot.transform.GetChild(0).gameObject);

        Instantiate(droneData[i].Container, pivot.transform);
    }

    public void RightButton() 
    {
        ButtonPressed(RightDrone());
    }

    public void LeftButton()
    {
        ButtonPressed(LeftDrone());
    }

    void ButtonPressed(int toDrone) 
    {
        rightButton.interactable = false;
        leftButton.interactable = false;
        selectButton.interactable = false;

        currentRT.transform.DOKill();
        currentRT.transform.localScale = Vector3.one;
        currentRT.transform
            .DOScale(0, .15f)
            .SetLoops(2, LoopType.Yoyo)
            .OnComplete(() =>
            {
                currentDrone = toDrone; // left
                UpdateSelectionMenu();
                rightButton.interactable = true;
                leftButton.interactable = true;
                selectButton.interactable = true;
            });

        rightRT.transform.DOKill();
        rightRT.transform.localScale = Vector3.one;
        rightRT.transform
            .DOScale(0, .15f)
            .SetLoops(2, LoopType.Yoyo);

        leftRT.transform.DOKill();
        leftRT.transform.localScale = Vector3.one;
        leftRT.transform
            .DOScale(0, .15f)
            .SetLoops(2, LoopType.Yoyo);
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
        selectButton.interactable = false;
        rightButton.interactable = false;
        leftButton.interactable = false;

        currentRT.transform.DOKill();
        currentRT.transform.localScale = Vector3.one;
        currentRT.transform
            .DOScale(1.15f, .15f)
            .SetLoops(2, LoopType.Yoyo)
            .OnComplete(() =>
            {
                UpdateSelectionMenu();
                rightButton.interactable = true;
                leftButton.interactable = true;
                selectButton.interactable = true;
            });
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
