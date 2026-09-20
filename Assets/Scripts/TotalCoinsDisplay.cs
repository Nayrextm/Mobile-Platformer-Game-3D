using UnityEngine;
using TMPro;

public class TotalCoinsDisplay : MonoBehaviour
{
    [Header("UI Елементи")]
    public TMP_Text coinsText;

    [Header("Налаштування")]
    public string prefix = "";

    void Start()
    {
        UpdateCoinDisplay();
    }

    public void UpdateCoinDisplay()
    {
        if (DatabaseManager.Instance != null)
        {
            int amount = DatabaseManager.Instance.GetTotalCoins();

            if (coinsText != null)
            {
                coinsText.text = $"{prefix}{amount}";
            }
        }
    }
}