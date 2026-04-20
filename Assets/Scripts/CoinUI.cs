using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CoinUI : MonoBehaviour
{
    public static CoinUI Instance;

    [Header("Налаштування рівня")]
    [Tooltip("Вкажіть, скільки всього монет розставлено на цій сцені")]
    public int totalCoinsOnLevel = 33;

    [Header("Посилання на текст")]
    public TextMeshProUGUI coinText;

    private int _collectedInThisLevel = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (DatabaseManager.Instance != null)
        {
            string currentScene = SceneManager.GetActiveScene().name;
            _collectedInThisLevel = DatabaseManager.Instance.GetCollectedCoinsCount(currentScene);
        }

        UpdateDisplay();
    }

    public void AddCoinAndDisplay()
    {
        _collectedInThisLevel++;
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if (coinText != null)
        {
            coinText.text = $"{_collectedInThisLevel}/{totalCoinsOnLevel}";
        }
    }
}