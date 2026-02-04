using UnityEngine;
using TMPro; // Не забудь підключити TextMeshPro

public class TotalDiamondsDisplay : MonoBehaviour
{
    [Header("UI Елементи")]
    public TMP_Text diamondsText; // Сюди перетягни свій текстовий об'єкт

    [Header("Налаштування")]
    public string prefix = "Diamonds: "; // Текст перед цифрою

    void Start()
    {
        UpdateDisplay();
    }

    // Можна викликати цей метод, якщо ти купив щось за алмази і треба оновити текст
    public void UpdateDisplay()
    {
        if (DatabaseManager.Instance != null)
        {
            // Викликаємо метод підрахунку з DatabaseManager
            int amount = DatabaseManager.Instance.GetTotalCollectedStarCoins();

            if (diamondsText != null)
            {
                diamondsText.text = $"{prefix}{amount}";
            }
        }
    }
}