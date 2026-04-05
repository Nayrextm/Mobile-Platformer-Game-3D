
//using UnityEngine;
//using TMPro;       // Потрібно для роботи з текстом
//using UnityEngine.UI;

//public class LevelStatsDisplay : MonoBehaviour
//{
//    [Header("Налаштування")]
//    public string levelID; // Назва рівня в базі (напр. "Level1")

//    [Header("UI Елементи")]
//    public TMP_Text timeText;
//    public TMP_Text attemptsText;

//    // ---> НОВЕ: Посилання на текстове поле статусу
//    [Header("Статус Рівня (Текст)")]
//    public TMP_Text statusText;

//    // Налаштування написів (можна змінити прямо в Інспекторі Unity)
//    public string textIfCompleted = "COMPLETED";
//    public string textIfNotCompleted = "NOT COMPLETED";

//    // Налаштування кольорів
//    public Color colorCompleted = Color.green;
//    public Color colorNotCompleted = Color.white;

//    [Header("Іконки Алмазів (UI Image)")]
//    public Image coin1Image;
//    public Image coin2Image;
//    public Image coin3Image;

//    [Header("Спрайти (Картинки)")]
//    public Sprite collectedSprite;
//    public Sprite missingSprite;

//    void Start()
//    {
//        UpdateStats();
//    }

//    public void UpdateStats()
//    {
//        if (DatabaseManager.Instance != null)
//        {
//            var stats = DatabaseManager.Instance.GetLevelData(levelID);

//            if (stats != null)
//            {
//                // 1. Час
//                float t = stats.TotalTime;
//                if (timeText) timeText.text = string.Format("TIME: {0:00}:{1:00}", Mathf.Floor(t / 60), t % 60);

//                // 2. Спроби
//                if (attemptsText) attemptsText.text = $"ATTEMPTS: {stats.TotalAttempts}";

//                // 3. АЛМАЗИ
//                SetCoinState(coin1Image, stats.StarCoin1);
//                SetCoinState(coin2Image, stats.StarCoin2);
//                SetCoinState(coin3Image, stats.StarCoin3);

//                // 4. ---> НОВЕ: ЛОГІКА ТЕКСТУ СТАТУСУ
//                if (statusText != null)
//                {
//                    // Перевіряємо поле IsCompleted з бази даних
//                    if (stats.IsCompleted)
//                    {
//                        // Якщо рівень пройдено
//                        statusText.text = textIfCompleted;
//                        statusText.color = colorCompleted;
//                    }
//                    else
//                    {
//                        // Якщо рівень грали, але не пройшли до кінця
//                        statusText.text = textIfNotCompleted;
//                        statusText.color = colorNotCompleted;
//                    }
//                }
//            }
//            else
//            {
//                // Рівень новий (жодної гри не було)
//                if (timeText) timeText.text = "TIME: 00:00";
//                if (attemptsText) attemptsText.text = "ATTEMPTS: 0";

//                SetCoinState(coin1Image, false);
//                SetCoinState(coin2Image, false);
//                SetCoinState(coin3Image, false);

//                // ---> Ставимо текст "Не пройдено" за замовчуванням
//                if (statusText != null)
//                {
//                    statusText.text = textIfNotCompleted;
//                    statusText.color = colorNotCompleted;
//                }
//            }
//        }
//    }

//    // Допоміжний метод для заміни спрайта
//    void SetCoinState(Image coinImg, bool collected)
//    {
//        if (coinImg != null)
//        {
//            coinImg.sprite = collected ? collectedSprite : missingSprite;
//            coinImg.color = Color.white;
//        }
//    }
//}
using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelStatsDisplay : MonoBehaviour
{
    [Header("Налаштування")]
    [SerializeField] private string _levelID;

    [Header("UI Елементи")]
    [SerializeField] private TMP_Text _timeText;
    [SerializeField] private TMP_Text _attemptsText;
    [SerializeField] private TMP_Text _statusText;

    [Header("Тексти Статусу")]
    [SerializeField] private string _textIfCompleted = "COMPLETED";
    [SerializeField] private string _textIfNotCompleted = "NOT COMPLETED";

    [Header("Кольори")]
    [SerializeField] private Color _colorCompleted = Color.green;
    [SerializeField] private Color _colorNotCompleted = Color.white;

    [Header("Іконки Алмазів")]
    [SerializeField] private Image _coin1Image;
    [SerializeField] private Image _coin2Image;
    [SerializeField] private Image _coin3Image;

    [Header("Спрайти")]
    [SerializeField] private Sprite _collectedSprite;
    [SerializeField] private Sprite _missingSprite;

    private void Start()
    {
        UpdateStats();
    }

    public void UpdateStats()
    {
        if (DatabaseManager.Instance == null) return;

        var stats = DatabaseManager.Instance.GetLevelData(_levelID);

        if (stats != null)
        {
            double t = stats.TotalTime;

            if (_timeText)
            {
                TimeSpan time = TimeSpan.FromSeconds(t);
                _timeText.text = $"TIME: {time.Minutes:D2}:{time.Seconds:D2}";
            }
            if (_attemptsText) _attemptsText.text = $"ATTEMPTS: {stats.TotalAttempts}";

            SetCoinState(_coin1Image, stats.StarCoin1);
            SetCoinState(_coin2Image, stats.StarCoin2);
            SetCoinState(_coin3Image, stats.StarCoin3);

            if (_statusText != null)
            {
                if (stats.IsCompleted)
                {
                    _statusText.text = _textIfCompleted;
                    _statusText.color = _colorCompleted;
                }
                else
                {
                    _statusText.text = _textIfNotCompleted;
                    _statusText.color = _colorNotCompleted;
                }
            }
        }
        else
        {
            if (_timeText) _timeText.text = "TIME: 00:00";
            if (_attemptsText) _attemptsText.text = "ATTEMPTS: 0";

            SetCoinState(_coin1Image, false);
            SetCoinState(_coin2Image, false);
            SetCoinState(_coin3Image, false);

            if (_statusText != null)
            {
                _statusText.text = _textIfNotCompleted;
                _statusText.color = _colorNotCompleted;
            }
        }
    }

    private void SetCoinState(Image coinImg, bool collected)
    {
        if (coinImg != null)
        {
            coinImg.sprite = collected ? _collectedSprite : _missingSprite;
            coinImg.color = Color.white;
        }
    }
}