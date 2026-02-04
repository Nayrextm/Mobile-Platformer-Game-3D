using UnityEngine;
using TMPro; // Підключаємо TextMeshPro

public class TotalCoinsDisplay : MonoBehaviour
{
    [Header("UI Елементи")]
    public TMP_Text coinsText; // Сюди перетягніть ваш текст (напр. "0")

    [Header("Налаштування")]
    public string prefix = ""; // Якщо треба написати щось перед цифрою (напр. "Coins: ")

    void Start()
    {
        UpdateCoinDisplay();
    }

    // Публічний метод, щоб можна було викликати ззовні (наприклад, після покупки в магазині)
    public void UpdateCoinDisplay()
    {
        if (DatabaseManager.Instance != null)
        {
            // 1. Отримуємо кількість монет з бази
            int amount = DatabaseManager.Instance.GetTotalCoins();

            // 2. Виводимо
            if (coinsText != null)
            {
                coinsText.text = $"{prefix}{amount}";
            }
        }
    }
}