using UnityEngine;
using TMPro;

public class CollectiblesDisplay : MonoBehaviour
{
    public enum CollectibleType
    {
        CrystalCoins,
        Diamods
    }

    [Header("Тип Колекційного Предмета")]
    [SerializeField] private CollectibleType _collectibleType;

    [Header("UI Елементи")]
    [SerializeField] private TMP_Text _amountText;

    [Header("Налаштування")]
    [SerializeField] private string _prefix = "";

    private void Start()
    {
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if (DatabaseManager.Instance == null || _amountText == null) return;

        int amount = _collectibleType switch
        {
            CollectibleType.CrystalCoins => DatabaseManager.Instance.GetTotalCoins(),
            CollectibleType.Diamods => DatabaseManager.Instance.GetTotalCollectedStarCoins(),
            _ => 0
        };

        _amountText.text = $"{_prefix}{amount}";
    }
}