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
//    public Slider gameMusicSlider; // Слайдер для музики рівня
//    public Slider sfxSlider;       // Слайдер для ефектів (SFX)

//    [Header("Audio")]
//    public MusicController musicController; // Контролер музики на рівні

//    bool isPaused = false;

//    void Start()
//    {
//        if (pausePanel != null) pausePanel.SetActive(false);
//        if (restartButton != null) restartButton.onClick.AddListener(RestartLevel);
//        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(GoToMainMenu);

//        // 1. Налаштування слайдера МУЗИКИ РІВНЯ
//        if (gameMusicSlider != null)
//        {
//            // Завантажуємо ключ "GameMusicVolume" (не Menu!)
//            float savedGameVol = PlayerPrefs.GetFloat("GameMusicVolume", 1f);
//            gameMusicSlider.value = savedGameVol;
//            gameMusicSlider.onValueChanged.AddListener(OnGameMusicChanged);
//        }

//        // 2. Налаштування слайдера SFX
//        if (sfxSlider != null)
//        {
//            // Завантажуємо ключ "SFXVolume"
//            float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 1f);
//            sfxSlider.value = savedSFX;
//            sfxSlider.onValueChanged.AddListener(OnSFXChanged);
//        }
//    }

//    // Зміна гучності музики (відразу чути ефект)
//    void OnGameMusicChanged(float value)
//    {
//        // Оновлюємо гучність у контролері в реальному часі
//        if (musicController != null)
//        {
//            musicController.SetVolume(value);
//        }

//        // Зберігаємо, щоб на наступному рівні було так само
//        PlayerPrefs.SetFloat("GameMusicVolume", value);
//        PlayerPrefs.Save();
//    }

//    // Зміна гучності ефектів
//    void OnSFXChanged(float value)
//    {
//        // Просто зберігаємо. 
//        // Примітка: Гравець почує зміни при наступному респауні або завантаженні рівня,
//        // бо PlayerController зчитує це в Awake.
//        PlayerPrefs.SetFloat("SFXVolume", value);
//        PlayerPrefs.Save();
//    }

//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.Escape))
//        {
//            if (isPaused) ResumeGame();
//            else PauseGame();
//        }
//    }

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
    public GameObject pausePanel;
    public Button restartButton;
    public Button mainMenuButton;

    [Header("Volume Sliders")]
    public Slider gameMusicSlider;
    public Slider sfxSlider; // Слайдер SFX

    [Header("Audio")]
    public MusicController musicController;

    bool isPaused = false;

    void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (restartButton != null) restartButton.onClick.AddListener(RestartLevel);
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(GoToMainMenu);

        // 1. Налаштування слайдера МУЗИКИ
        if (gameMusicSlider != null)
        {
            float savedGameVol = PlayerPrefs.GetFloat("GameMusicVolume", 1f);
            gameMusicSlider.value = savedGameVol;
            gameMusicSlider.onValueChanged.AddListener(OnGameMusicChanged);
        }

        // 2. Налаштування слайдера SFX
        if (sfxSlider != null)
        {
            // Завантажуємо збережене значення (спільне з головним меню)
            float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 1f);
            sfxSlider.value = savedSFX;
            sfxSlider.onValueChanged.AddListener(OnSFXChanged);
        }
    }

    void OnGameMusicChanged(float value)
    {
        if (musicController != null) musicController.SetVolume(value);
        PlayerPrefs.SetFloat("GameMusicVolume", value);
        PlayerPrefs.Save();
    }

    // ---> ОНОВЛЕНО: Зміна SFX
    void OnSFXChanged(float value)
    {
        // 1. Зберігаємо глобально (щоб в Головному Меню теж змінилося)
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();

        // 2. Шукаємо гравця на сцені і міняємо йому гучність прямо зараз
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.SetSFXVolume(value);
        }
    }

    // ... Методи PauseGame, ResumeGame, RestartLevel, GoToMainMenu без змін ...

    void Update() { if (Input.GetKeyDown(KeyCode.Escape)) { if (isPaused) ResumeGame(); else PauseGame(); } }

    void PauseGame()
    {
        if (pausePanel != null) pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.visible = true;
        if (musicController != null) musicController.PauseMusic();
    }

    public void ResumeGame()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.visible = false;
        if (musicController != null) musicController.ResumeMusic();
    }

    void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
