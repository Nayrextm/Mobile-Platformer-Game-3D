//using UnityEngine;
//using TMPro; // Обов'язково для TextMeshPro

//public class LevelStatsDisplay : MonoBehaviour
//{
//    [Header("Налаштування")]
//    public string levelID = "Level_1"; // ID рівня, як він записаний в базі (має співпадати з назвою сцени)

//    [Header("UI Елементи")]
//    public TMP_Text timeText;      // Поле TIME
//    public TMP_Text attemptsText;  // Поле ATTEMPTS
//    public TMP_Text scoreText;     // Поле SCORE (якщо є)

//    // Цей метод викликається щоразу, коли об'єкт вмикається (коли заходиш в меню)
//    void OnEnable()
//    {
//        UpdateStatsUI();
//    }

//    public void UpdateStatsUI()
//    {
//        // Перевіряємо, чи база існує
//        if (DatabaseManager.Instance == null) return;

//        // 1. Отримуємо дані з бази
//        LevelStat stats = DatabaseManager.Instance.GetLevelData(levelID);

//        // 2. Якщо даних ще немає (рівень ніколи не грали)
//        if (stats == null)
//        {
//            if (timeText) timeText.text = "TIME: 0s";
//            if (attemptsText) attemptsText.text = "ATTEMPTS: 0";
//            if (scoreText) scoreText.text = "SCORE: 0%"; // Або DIFFICULTY
//            return;
//        }

//        // 3. Записуємо дані в текст
//        if (timeText)
//        {
//            // Форматування часу (хвилини:секунди)
//            float t = stats.TotalTime;
//            string formattedTime = string.Format("{0:00}:{1:00}", Mathf.Floor(t / 60), t % 60);
//            timeText.text = $"TIME: {formattedTime}";
//        }

//        if (attemptsText)
//            attemptsText.text = $"ATTEMPTS: {stats.TotalAttempts}";

//        // Якщо у тебе є поле Score або Difficulty
//        if (scoreText)
//            scoreText.text = "SCORE: ..."; // Тут твоя логіка очок
//    }
//}
//using UnityEngine;
//using TMPro;

//public class LevelStatsDisplay : MonoBehaviour
//{
//    public string levelID; // Впиши тут назву сцени рівня (напр. "Synthwave")
//    public TMP_Text timeText;
//    public TMP_Text attemptsText;

//    void Start()
//    {
//        if (DatabaseManager.Instance != null)
//        {
//            var stats = DatabaseManager.Instance.GetLevelData(levelID);

//            if (stats != null)
//            {
//                // Конвертуємо секунди в хвилини:секунди
//                float t = stats.TotalTime;
//                string formattedTime = string.Format("{0:00}:{1:00}", Mathf.Floor(t / 60), t % 60);

//                if (timeText) timeText.text = $"TIME: {formattedTime}";
//                if (attemptsText) attemptsText.text = $"ATTEMPTS: {stats.TotalAttempts}";
//            }
//            else
//            {
//                // Якщо ще не грали
//                if (timeText) timeText.text = "TIME: 00:00";
//                if (attemptsText) attemptsText.text = "ATTEMPTS: 0";
//            }
//        }
//    }
//}
//using UnityEngine;
//using TMPro;
//using UnityEngine.UI; // ! Додайте це, щоб працювати з картинками

//public class LevelStatsDisplay : MonoBehaviour
//{
//    [Header("Налаштування")]
//    public string levelID; // Назва рівня в базі (напр. "Level1")

//    [Header("UI Елементи")]
//    public TMP_Text timeText;
//    public TMP_Text attemptsText;

//    [Header("Іконки Алмазів (UI Image)")]
//    public Image coin1Image;
//    public Image coin2Image;
//    public Image coin3Image;

//    [Header("Кольори")]
//    public Color collectedColor = Color.white; // Який вигляд має зібрана монета
//    public Color missingColor = Color.black;   // Який вигляд має незібрана (тінь)

//    void Start() // Або OnEnable, якщо меню вмикається/вимикається
//    {
//        UpdateStats();
//    }

//    public void UpdateStats()
//    {
//        if (DatabaseManager.Instance != null)
//        {
//            // Отримуємо дані про рівень
//            var stats = DatabaseManager.Instance.GetLevelData(levelID);

//            if (stats != null)
//            {
//                // 1. Час
//                float t = stats.TotalTime;
//                if (timeText) timeText.text = string.Format("TIME: {0:00}:{1:00}", Mathf.Floor(t / 60), t % 60);

//                // 2. Спроби
//                if (attemptsText) attemptsText.text = $"ATTEMPTS: {stats.TotalAttempts}";

//                // 3. АЛМАЗИ (Перевіряємо кожен окремо)
//                SetCoinState(coin1Image, stats.StarCoin1);
//                SetCoinState(coin2Image, stats.StarCoin2);
//                SetCoinState(coin3Image, stats.StarCoin3);
//            }
//            else
//            {
//                // Якщо даних немає (рівень ще не грали)
//                if (timeText) timeText.text = "TIME: 00:00";
//                if (attemptsText) attemptsText.text = "ATTEMPTS: 0";

//                // Всі монети сірі
//                SetCoinState(coin1Image, false);
//                SetCoinState(coin2Image, false);
//                SetCoinState(coin3Image, false);
//            }
//        }
//    }

//    // Допоміжний метод для фарбування іконок
//    void SetCoinState(Image coinImg, bool collected)
//    {
//        if (coinImg != null)
//        {
//            coinImg.color = collected ? collectedColor : missingColor;
//        }
//    }
//}
//using UnityEngine;
//using TMPro;
//using UnityEngine.UI;

//public class LevelStatsDisplay : MonoBehaviour
//{
//    [Header("Налаштування")]
//    public string levelID; // Назва рівня в базі (напр. "Level1")

//    [Header("UI Елементи")]
//    public TMP_Text timeText;
//    public TMP_Text attemptsText;

//    [Header("Іконки Алмазів (UI Image)")]
//    // Це самі об'єкти на сцені, в яких ми будемо міняти картинку
//    public Image coin1Image;
//    public Image coin2Image;
//    public Image coin3Image;

//    [Header("Спрайти (Картинки)")]
//    // Сюди перетягни файл картинки "Золотий алмаз"
//    public Sprite collectedSprite;
//    // Сюди перетягни файл картинки "Сірий/Пустий алмаз"
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
//            }
//            else
//            {
//                // Якщо даних немає
//                if (timeText) timeText.text = "TIME: 00:00";
//                if (attemptsText) attemptsText.text = "ATTEMPTS: 0";

//                // Всі монети як "незібрані"
//                SetCoinState(coin1Image, false);
//                SetCoinState(coin2Image, false);
//                SetCoinState(coin3Image, false);
//            }
//        }
//    }

//    // Допоміжний метод для заміни спрайта
//    void SetCoinState(Image coinImg, bool collected)
//    {
//        if (coinImg != null)
//        {
//            // Якщо зібрано -> ставимо кольорову картинку
//            // Якщо ні -> ставимо сіру картинку
//            coinImg.sprite = collected ? collectedSprite : missingSprite;

//            // ВАЖЛИВО: Скидаємо колір на білий. 
//            // Якщо раніше ви міняли колір на чорний, то нова картинка теж буде чорною.
//            // Білий колір в Unity UI означає "показувати оригінальні кольори картинки".
//            coinImg.color = Color.white;
//        }
//    }
//}
using UnityEngine;
using TMPro;       // Потрібно для роботи з текстом
using UnityEngine.UI;

public class LevelStatsDisplay : MonoBehaviour
{
    [Header("Налаштування")]
    public string levelID; // Назва рівня в базі (напр. "Level1")

    [Header("UI Елементи")]
    public TMP_Text timeText;
    public TMP_Text attemptsText;

    // ---> НОВЕ: Посилання на текстове поле статусу
    [Header("Статус Рівня (Текст)")]
    public TMP_Text statusText;

    // Налаштування написів (можна змінити прямо в Інспекторі Unity)
    public string textIfCompleted = "COMPLETED";
    public string textIfNotCompleted = "NOT COMPLETED";

    // Налаштування кольорів
    public Color colorCompleted = Color.green;
    public Color colorNotCompleted = Color.white;

    [Header("Іконки Алмазів (UI Image)")]
    public Image coin1Image;
    public Image coin2Image;
    public Image coin3Image;

    [Header("Спрайти (Картинки)")]
    public Sprite collectedSprite;
    public Sprite missingSprite;

    void Start()
    {
        UpdateStats();
    }

    public void UpdateStats()
    {
        if (DatabaseManager.Instance != null)
        {
            var stats = DatabaseManager.Instance.GetLevelData(levelID);

            if (stats != null)
            {
                // 1. Час
                float t = stats.TotalTime;
                if (timeText) timeText.text = string.Format("TIME: {0:00}:{1:00}", Mathf.Floor(t / 60), t % 60);

                // 2. Спроби
                if (attemptsText) attemptsText.text = $"ATTEMPTS: {stats.TotalAttempts}";

                // 3. АЛМАЗИ
                SetCoinState(coin1Image, stats.StarCoin1);
                SetCoinState(coin2Image, stats.StarCoin2);
                SetCoinState(coin3Image, stats.StarCoin3);

                // 4. ---> НОВЕ: ЛОГІКА ТЕКСТУ СТАТУСУ
                if (statusText != null)
                {
                    // Перевіряємо поле IsCompleted з бази даних
                    if (stats.IsCompleted)
                    {
                        // Якщо рівень пройдено
                        statusText.text = textIfCompleted;
                        statusText.color = colorCompleted;
                    }
                    else
                    {
                        // Якщо рівень грали, але не пройшли до кінця
                        statusText.text = textIfNotCompleted;
                        statusText.color = colorNotCompleted;
                    }
                }
            }
            else
            {
                // Рівень новий (жодної гри не було)
                if (timeText) timeText.text = "TIME: 00:00";
                if (attemptsText) attemptsText.text = "ATTEMPTS: 0";

                SetCoinState(coin1Image, false);
                SetCoinState(coin2Image, false);
                SetCoinState(coin3Image, false);

                // ---> Ставимо текст "Не пройдено" за замовчуванням
                if (statusText != null)
                {
                    statusText.text = textIfNotCompleted;
                    statusText.color = colorNotCompleted;
                }
            }
        }
    }

    // Допоміжний метод для заміни спрайта
    void SetCoinState(Image coinImg, bool collected)
    {
        if (coinImg != null)
        {
            coinImg.sprite = collected ? collectedSprite : missingSprite;
            coinImg.color = Color.white;
        }
    }
}