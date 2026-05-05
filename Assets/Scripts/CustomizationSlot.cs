using UnityEngine;
using UnityEngine.UI;

public class CustomizationSlot : MonoBehaviour
{
    public Button slotButton;
    public Image itemIcon;
    public GameObject equippedIndicator; 
    public GameObject selectedHighlight; 

    public void SetState(bool isEquipped, bool isSelected)
    {
        if (equippedIndicator != null) equippedIndicator.SetActive(isEquipped);
        if (selectedHighlight != null) selectedHighlight.SetActive(isSelected);
    }
}