
//using UnityEngine;
//using TMPro;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;

//public class WinScreenController : MonoBehaviour
//{
//    public TMP_Text timeText;
//    public TMP_Text attemptsText;
//    public Button restartButton;
//    public Button mainMenuButton;

//    private void Awake()
//    {
//        if (restartButton) restartButton.onClick.AddListener(RestartLevel);
//        if (mainMenuButton) mainMenuButton.onClick.AddListener(GoToMainMenu);
//    }

//    public void Show(float sessionTime, int sessionAttempts)
//    {
//        gameObject.SetActive(true);
//        Time.timeScale = 0f; // Пауза гри

//        if (timeText != null)
//            timeText.text = $"TIME: {sessionTime:F2} s";

//        if (attemptsText != null)
//            attemptsText.text = $"ATTEMPTS: {sessionAttempts}";
//    }

//    private void RestartLevel()
//    {
//        Time.timeScale = 1f;
//        // Перезавантаження сцени скине сесійну статистику (це правильно для нової гри)
//        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
//    }

//    private void GoToMainMenu()
//    {
//        Time.timeScale = 1f;
//        SceneManager.LoadScene("MainMenu"); // Назва сцени меню
//    }
//}
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinScreenController : MonoBehaviour
{
    [SerializeField] private TMP_Text _timeText;
    [SerializeField] private TMP_Text _attemptsText;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _mainMenuButton;

    private void Awake()
    {
        if (_restartButton) _restartButton.onClick.AddListener(RestartLevel);
        if (_mainMenuButton) _mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    private void OnDestroy()
    {
        if (_restartButton) _restartButton.onClick.RemoveListener(RestartLevel);
        if (_mainMenuButton) _mainMenuButton.onClick.RemoveListener(GoToMainMenu);
    }

    public void Show(float sessionTime, int sessionAttempts)
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;

        if (_timeText != null)
            _timeText.text = $"TIME: {sessionTime:F2} s";

        if (_attemptsText != null)
            _attemptsText.text = $"ATTEMPTS: {sessionAttempts}";
    }

    private void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}