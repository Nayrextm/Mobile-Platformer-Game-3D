////using UnityEngine;
////using TMPro;
////using UnityEngine.SceneManagement;
////using UnityEngine.UI;

////public class WinScreenController : MonoBehaviour
////{
////    [Header("UI Elements")]
////    public TMP_Text timeText;      // для часу проходження рівня
////    public TMP_Text attemptsText;  // для кількості спроб
////    public Button restartButton;
////    public Button mainMenuButton;

////    private void Awake()
////    {
////        // Прив'язуємо кнопки
////        if (restartButton != null)
////            restartButton.onClick.AddListener(RestartLevel);

////        if (mainMenuButton != null)
////            mainMenuButton.onClick.AddListener(GoToMainMenu);
////    }

////    /// <summary>
////    /// Показує WinScreen та заповнює статистику
////    /// </summary>
////    /// <param name="time">Час проходження у секундах</param>
////    /// <param name="attempts">Кількість спроб</param>
////    public void Show(float time, int attempts)
////    {
////        gameObject.SetActive(true);
////        Time.timeScale = 0f;

////        if (timeText != null)
////            timeText.text = $"Time: {time:F2} s";

////        if (attemptsText != null)
////            attemptsText.text = $"Attempts: {attempts}";
////    }

////    private void RestartLevel()
////    {
////        Time.timeScale = 1f;
////        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
////    }

////    private void GoToMainMenu()
////    {
////        Time.timeScale = 1f;
////        SceneManager.LoadScene("MainMenu");
////    }
////}
//using UnityEngine;
//using TMPro;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;

//public class WinScreenController : MonoBehaviour
//{
//    [Header("UI Elements")]
//    public TMP_Text timeText;      // для часу проходження
//    public TMP_Text attemptsText;  // для кількості спроб
//    public Button restartButton;
//    public Button mainMenuButton;

//    private void Awake()
//    {
//        // Прив'язуємо кнопки
//        if (restartButton != null)
//            restartButton.onClick.AddListener(RestartLevel);

//        if (mainMenuButton != null)
//            mainMenuButton.onClick.AddListener(GoToMainMenu);
//    }

//    /// <summary>
//    /// Показує WinScreen та підтягує статистику з Бази Даних
//    /// </summary>
//    /// <param name="sessionTime">Час цієї спроби</param>
//    /// <param name="sessionAttempts">Спроби за цю сесію</param>
//    public void Show(float sessionTime, int sessionAttempts)
//    {
//        gameObject.SetActive(true);
//        Time.timeScale = 0f;

//        // 1. Отримуємо дані з бази про ЦЕЙ рівень
//        string currentLevel = SceneManager.GetActiveScene().name;
//        LevelStat dbStats = null;

//        if (DatabaseManager.Instance != null)
//        {
//            dbStats = DatabaseManager.Instance.GetLevelData(currentLevel);
//        }

//        // 2. Формуємо текст для часу
//        if (timeText != null)
//        {
//            // Якщо база є, показуємо: "Time: 12.5s (Total: 500s)"
//            if (dbStats != null)
//            {
//                timeText.text = $"Time: {sessionTime:F2} s\nTotal: {dbStats.TotalTime:F2} s";
//            }
//            else
//            {
//                timeText.text = $"Time: {sessionTime:F2} s";
//            }
//        }

//        // 3. Формуємо текст для спроб
//        if (attemptsText != null)
//        {
//            // Якщо база є, показуємо загальну кількість спроб з бази
//            if (dbStats != null)
//            {
//                attemptsText.text = $"Total Attempts: {dbStats.TotalAttempts}";
//            }
//            else
//            {
//                attemptsText.text = $"Attempts: {sessionAttempts}";
//            }
//        }
//    }

//    private void RestartLevel()
//    {
//        Time.timeScale = 1f;
//        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
//    }

//    private void GoToMainMenu()
//    {
//        Time.timeScale = 1f;
//        SceneManager.LoadScene("MainMenu"); // Переконайся, що сцена меню називається саме так
//    }
//}
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinScreenController : MonoBehaviour
{
    public TMP_Text timeText;
    public TMP_Text attemptsText;
    public Button restartButton;
    public Button mainMenuButton;

    private void Awake()
    {
        if (restartButton) restartButton.onClick.AddListener(RestartLevel);
        if (mainMenuButton) mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    public void Show(float sessionTime, int sessionAttempts)
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f; // Пауза гри

        if (timeText != null)
            timeText.text = $"TIME: {sessionTime:F2} s";

        if (attemptsText != null)
            attemptsText.text = $"ATTEMPTS: {sessionAttempts}";
    }

    private void RestartLevel()
    {
        Time.timeScale = 1f;
        // Перезавантаження сцени скине сесійну статистику (це правильно для нової гри)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // Назва сцени меню
    }
}