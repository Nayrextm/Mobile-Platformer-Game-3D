using UnityEngine;
using TMPro; // Потрібно для TextMeshPro

public class CoinUI : MonoBehaviour
{
    public static CoinUI Instance; // Сінґлтон для легкого доступу

    [Header("Посилання на текст")]
    public TextMeshProUGUI coinText;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // При старті сцени одразу показуємо актуальну суму
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if (DatabaseManager.Instance != null)
        {
            // Беремо дані з бази (метод GetTotalCoins вже налаштований у вас на CollectiblesStat)
            int totalCoins = DatabaseManager.Instance.GetTotalCoins();

            // Оновлюємо текст
            if (coinText != null)
            {
                coinText.text = totalCoins.ToString();
            }
        }
    }
}