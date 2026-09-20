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
    [SerializeField] private UIPanelFader _panelFader;

    [Header("Зоряні Монети")]
    [SerializeField] private Image[] _starIcons;

    [Header("Спрайти")]
    [SerializeField] private Sprite _collectedSprite;
    [SerializeField] private Sprite _missingSprite;

    private void Awake()
    {
        if (_restartButton) _restartButton.onClick.AddListener(RestartLevel);
        if (_mainMenuButton) _mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    public void Show(float sessionTime, int sessionAttempts)
    {
        if (_panelFader != null) _panelFader.Show();
        else gameObject.SetActive(true);

        Time.timeScale = 0f;

        if (_timeText != null) _timeText.text = $"TIME: {sessionTime:F2} s";
        if (_attemptsText != null) _attemptsText.text = $"ATTEMPTS: {sessionAttempts}";

        UpdateStarsDisplay();
    }

    private void UpdateStarsDisplay()
    {
        if (DatabaseManager.Instance == null) return;
        string scene = SceneManager.GetActiveScene().name;

        for (int i = 0; i < _starIcons.Length; i++)
        {
            if (_starIcons[i] == null) continue;
            
            bool collected = DatabaseManager.Instance.IsStarCoinCollected(scene, i);

            _starIcons[i].sprite = collected ? _collectedSprite : _missingSprite;

            _starIcons[i].color = Color.white;
        }
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