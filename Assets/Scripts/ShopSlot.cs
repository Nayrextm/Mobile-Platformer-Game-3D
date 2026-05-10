using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System; 

public class ShopSlot : MonoBehaviour
{
    [Header("UI Åëåìåíòè")]
    [SerializeField] private Image _colorIcon;
    [SerializeField] private TMP_Text _priceText;
    [SerializeField] private Button _buyButton;

    public void Setup(Color itemColor, int price, bool isUnlocked, Action onBuyAction)
    {
        _colorIcon.color = itemColor;

        _buyButton.onClick.RemoveAllListeners();

        if (isUnlocked)
        {
            _priceText.text = "ÊÓÏËÅÍÎ";
            _buyButton.interactable = false;
        }
        else
        {
            _priceText.text = price.ToString();
            _buyButton.interactable = true;

            _buyButton.onClick.AddListener(() => onBuyAction?.Invoke());
        }
    }
}