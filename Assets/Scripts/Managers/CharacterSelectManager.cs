using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectManager : MonoBehaviour
{
    [Header("Characters")]
    [SerializeField] DroneData[] droneData;
    int currentDroneID = 0;

    [Header("RectTransforms")]
    [SerializeField] GameObject currentRT;
    [SerializeField] GameObject rightRT;
    [SerializeField] GameObject leftRT;

    [Header("Circles")]
    [SerializeField] Image leftCircle;
    [SerializeField] Image rightCircle;
    [SerializeField] Image currentCircle;

    [SerializeField] Color selectedColor = Color.blue;
    [SerializeField] Color normalColor = Color.white;
    [SerializeField] Color lockedColor = Color.gray;

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
        currentDroneID = SelectedDroneID();
        UpdateSelectionMenu();
    }

    public void UpdateSelectionMenu()
    {
        //Selected
        ReplaceMesh(ref selectCharacterPivot, SelectedDroneID());

        //Current
        if (SelectedDroneID() == currentDroneID)
        {
            selecButtonText.alpha = .1f;
            selecButtonText.text = "Selected";
            currentCharacterName.color = selectedColor;
        }
        else
        {
            selecButtonText.alpha = 1;
            selecButtonText.text = "Select";
            currentCharacterName.color = normalColor;
            selectButton.interactable = true;
        }
        currentCharacterName.text = droneData[currentDroneID].DroneName;
        ReplaceMesh(ref currentCharacterPivot, currentDroneID);

        ReplaceMesh(ref leftCharacterPivot, LeftDroneID());
        
        ReplaceMesh(ref rightCharacterPivot, RightDroneID());

        UpdateCircleColors();

    }


    void UpdateCircleColors()
    {
        // Reset all to normal first
        currentCircle.color = normalColor;
        rightCircle.color = normalColor;
        leftCircle.color = normalColor;

        if (SelectedDroneID() == currentDroneID)
            currentCircle.color = selectedColor;
        else if (SelectedDroneID() == LeftDroneID()) // This was correct
            leftCircle.color = selectedColor;
        else if (SelectedDroneID() == RightDroneID()) // FIX: Changed from LeftDroneID to RightDroneID
            rightCircle.color = selectedColor;
    }

    void ReplaceMesh(ref GameObject pivot, int i) 
    {
        if (pivot.transform.childCount > 0)
            Destroy(pivot.transform.GetChild(0).gameObject);

        Instantiate(droneData[i].Container, pivot.transform);
    }

    public void RightButton() 
    {
        ButtonPressed(RightDroneID());
    }

    public void LeftButton()
    {
        ButtonPressed(LeftDroneID());
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
                currentDroneID = toDrone; // left
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

    int LeftDroneID() 
    {
        return (currentDroneID - 1 + droneData.Length) % droneData.Length;
    }

    int RightDroneID()
    {
        return (currentDroneID + 1) % droneData.Length;
    }

    public void SelectCharacter()
    {
        UserData.Instance.startDroneData = droneData[currentDroneID];
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
