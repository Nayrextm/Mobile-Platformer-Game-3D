using UnityEngine;
using TMPro;

public class TotalDiamondsDisplay : MonoBehaviour
{
    [Header("UI Елементи")]
    public TMP_Text diamondsText; 

    [Header("Налаштування")]
    public string prefix = "Diamonds: "; 

    void Start()
    {
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if (DatabaseManager.Instance != null)
        {
            int amount = DatabaseManager.Instance.GetTotalCollectedStarCoins();

            if (diamondsText != null)
            {
                diamondsText.text = $"{prefix}{amount}";
            }
        }
    }
}