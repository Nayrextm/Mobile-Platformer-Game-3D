using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [Header("Бази Даних")]
    [SerializeField] private ColorDatabase _colorDatabase;

    [Header("UI Магазину")]
    [SerializeField] private Transform _shopContainer;
    [SerializeField] private ShopSlot _shopSlotPrefab;

    [Header("Зв'язок з вашим UI")]
    [SerializeField] private CollectiblesDisplay _coinDisplay;

    private List<ShopSlot> _spawnedSlots = new List<ShopSlot>();

    private void OnEnable()
    {
        InitializeShop();
    }

    private void InitializeShop()
    {
        if (DatabaseManager.Instance == null || _colorDatabase == null) return;

        UpdateCoinDisplay();

        if (_spawnedSlots.Count > 0)
        {
            RefreshExistingSlots();
            return;
        }

        
        for (int i = 1; i < _colorDatabase.Count; i++)
        {
            int index = i;
            int itemPrice = _colorDatabase.GetPriceByIndex(index);
            bool isUnlocked = DatabaseManager.Instance.IsItemUnlocked("TrailColor", index);

            ShopSlot slot = Instantiate(_shopSlotPrefab, _shopContainer);
            slot.Setup(_colorDatabase.GetColorByIndex(index), itemPrice, isUnlocked, () => OnBuyTrailClicked(index));

            _spawnedSlots.Add(slot);
        }
    }

    private void RefreshExistingSlots()
    {
        
        for (int i = 0; i < _spawnedSlots.Count; i++)
        {
            int index = i + 1; 
            bool isUnlocked = DatabaseManager.Instance.IsItemUnlocked("TrailColor", index);
            int itemPrice = _colorDatabase.GetPriceByIndex(index);

            _spawnedSlots[i].Setup(_colorDatabase.GetColorByIndex(index), itemPrice, isUnlocked, () => OnBuyTrailClicked(index));
        }
    }

    private void OnBuyTrailClicked(int index)
    {
        int itemPrice = _colorDatabase.GetPriceByIndex(index);

        if (DatabaseManager.Instance.TrySpendCoins(itemPrice))
        {
            DatabaseManager.Instance.UnlockItem("Color", index);

#if UNITY_EDITOR
            Debug.Log($"Колір #{index} куплено як загальний доступ!");
#endif

            RefreshExistingSlots();
            UpdateCoinDisplay();
        }
    }

    private void UpdateCoinDisplay()
    {
        if (_coinDisplay != null)
        {
            _coinDisplay.UpdateDisplay();
        }
    }
}