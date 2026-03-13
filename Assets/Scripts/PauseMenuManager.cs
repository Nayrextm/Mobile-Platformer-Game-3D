
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;

//public class PauseMenuManager : MonoBehaviour
//{
//    [Header("UI References")]
//    public GameObject pausePanel;
//    public Button restartButton;
//    public Button mainMenuButton;

//    [Header("Volume Sliders")]
//    public Slider gameMusicSlider;
//    public Slider sfxSlider; // Слайдер SFX

//    [Header("Audio")]
//    public MusicController musicController;

//    bool isPaused = false;

//    void Start()
//    {
//        if (pausePanel != null) pausePanel.SetActive(false);
//        if (restartButton != null) restartButton.onClick.AddListener(RestartLevel);
//        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(GoToMainMenu);

//        // 1. Налаштування слайдера МУЗИКИ
//        if (gameMusicSlider != null)
//        {
//            float savedGameVol = PlayerPrefs.GetFloat("GameMusicVolume", 1f);
//            gameMusicSlider.value = savedGameVol;
//            gameMusicSlider.onValueChanged.AddListener(OnGameMusicChanged);
//        }

//        // 2. Налаштування слайдера SFX
//        if (sfxSlider != null)
//        {
//            // Завантажуємо збережене значення (спільне з головним меню)
//            float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 1f);
//            sfxSlider.value = savedSFX;
//            sfxSlider.onValueChanged.AddListener(OnSFXChanged);
//        }
//    }

//    void OnGameMusicChanged(float value)
//    {
//        if (musicController != null) musicController.SetVolume(value);
//        PlayerPrefs.SetFloat("GameMusicVolume", value);
//        PlayerPrefs.Save();
//    }

//    // ---> ОНОВЛЕНО: Зміна SFX
//    void OnSFXChanged(float value)
//    {
//        // 1. Зберігаємо глобально (щоб в Головному Меню теж змінилося)
//        PlayerPrefs.SetFloat("SFXVolume", value);
//        PlayerPrefs.Save();

//        // 2. Шукаємо гравця на сцені і міняємо йому гучність прямо зараз
//        PlayerController player = FindObjectOfType<PlayerController>();
//        if (player != null)
//        {
//            player.SetSFXVolume(value);
//        }
//    }

//    // ... Методи PauseGame, ResumeGame, RestartLevel, GoToMainMenu без змін ...

//    void Update() { if (Input.GetKeyDown(KeyCode.Escape)) { if (isPaused) ResumeGame(); else PauseGame(); } }

//    void PauseGame()
//    {
//        if (pausePanel != null) pausePanel.SetActive(true);
//        Time.timeScale = 0f;
//        isPaused = true;
//        Cursor.visible = true;
//        if (musicController != null) musicController.PauseMusic();
//    }

//    public void ResumeGame()
//    {
//        if (pausePanel != null) pausePanel.SetActive(false);
//        Time.timeScale = 1f;
//        isPaused = false;
//        Cursor.visible = false;
//        if (musicController != null) musicController.ResumeMusic();
//    }

//    void RestartLevel()
//    {
//        Time.timeScale = 1f;
//        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
//    }

//    void GoToMainMenu()
//    {
//        Time.timeScale = 1f;
//        SceneManager.LoadScene("MainMenu");
//    }
//}
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _mainMenuButton;

    [Header("Volume Sliders")]
    [SerializeField] private Slider _gameMusicSlider;
    [SerializeField] private Slider _sfxSlider;

    [Header("Audio")]
    [SerializeField] private MusicController _musicController;

    private bool _isPaused = false;
    private PlayerController _player;

    private void Start()
    {
        _player = FindObjectOfType<PlayerController>();

        if (_pausePanel != null) _pausePanel.SetActive(false);

        if (_restartButton != null) _restartButton.onClick.AddListener(RestartLevel);
        if (_mainMenuButton != null) _mainMenuButton.onClick.AddListener(GoToMainMenu);

        if (_gameMusicSlider != null)
        {
            _gameMusicSlider.value = PlayerPrefs.GetFloat("GameMusicVolume", 1f);
            _gameMusicSlider.onValueChanged.AddListener(OnGameMusicChanged);
        }

        if (_sfxSlider != null)
        {
            _sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
            _sfxSlider.onValueChanged.AddListener(OnSFXChanged);
        }
    }

    private void OnDestroy()
    {
        if (_restartButton != null) _restartButton.onClick.RemoveListener(RestartLevel);
        if (_mainMenuButton != null) _mainMenuButton.onClick.RemoveListener(GoToMainMenu);
        if (_gameMusicSlider != null) _gameMusicSlider.onValueChanged.RemoveListener(OnGameMusicChanged);
        if (_sfxSlider != null) _sfxSlider.onValueChanged.RemoveListener(OnSFXChanged);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPaused) ResumeGame();
            else PauseGame();
        }
    }

    private void OnGameMusicChanged(float value)
    {
        if (_musicController != null) _musicController.SetVolume(value);
        PlayerPrefs.SetFloat("GameMusicVolume", value);
    }

    private void OnSFXChanged(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);

        if (_player != null)
        {
            _player.SetSFXVolume(value);
        }
    }

    public void SaveSettingsToDisk()
    {
        PlayerPrefs.Save();
    }

    private void PauseGame()
    {
        if (_pausePanel != null) _pausePanel.SetActive(true);
        Time.timeScale = 0f;
        _isPaused = true;
        Cursor.visible = true;
        if (_musicController != null) _musicController.PauseMusic();
    }

    public void ResumeGame()
    {
        SaveSettingsToDisk();

        if (_pausePanel != null) _pausePanel.SetActive(false);
        Time.timeScale = 1f;
        _isPaused = false;
        Cursor.visible = false;
        if (_musicController != null) _musicController.ResumeMusic();
    }

    private void RestartLevel()
    {
        SaveSettingsToDisk();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void GoToMainMenu()
    {
        SaveSettingsToDisk();
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
