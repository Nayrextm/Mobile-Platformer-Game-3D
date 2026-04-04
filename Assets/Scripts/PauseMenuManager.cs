
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;

//public class PauseMenuManager : MonoBehaviour
//{
//    [Header("UI References")]
//    [SerializeField] private GameObject _pausePanel;
//    [SerializeField] private Button _restartButton;
//    [SerializeField] private Button _mainMenuButton;

//    [SerializeField] private WinScreenController _winScreen;

//    [Header("Volume Sliders")]
//    [SerializeField] private Slider _gameMusicSlider;
//    [SerializeField] private Slider _sfxSlider;

//    [Header("Audio")]
//    [SerializeField] private MusicController _musicController;

//    private bool _isPaused = false;
//    private PlayerController _player;

//    private void Start()
//    {
//        _player = FindObjectOfType<PlayerController>();

//        if (_pausePanel != null) _pausePanel.SetActive(false);

//        if (_restartButton != null) _restartButton.onClick.AddListener(RestartLevel);
//        if (_mainMenuButton != null) _mainMenuButton.onClick.AddListener(GoToMainMenu);

//        if (_gameMusicSlider != null)
//        {
//            _gameMusicSlider.value = PlayerPrefs.GetFloat("GameMusicVolume", 1f);
//            _gameMusicSlider.onValueChanged.AddListener(OnGameMusicChanged);
//        }

//        if (_sfxSlider != null)
//        {
//            _sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
//            _sfxSlider.onValueChanged.AddListener(OnSFXChanged);
//        }
//    }

//    private void OnDestroy()
//    {
//        if (_restartButton != null) _restartButton.onClick.RemoveListener(RestartLevel);
//        if (_mainMenuButton != null) _mainMenuButton.onClick.RemoveListener(GoToMainMenu);
//        if (_gameMusicSlider != null) _gameMusicSlider.onValueChanged.RemoveListener(OnGameMusicChanged);
//        if (_sfxSlider != null) _sfxSlider.onValueChanged.RemoveListener(OnSFXChanged);
//    }

//    private void Update()
//    {

//        if (Input.GetKeyDown(KeyCode.Escape))
//        {
//            if (_winScreen != null && _winScreen.gameObject.activeInHierarchy) return;

//            if (_isPaused) ResumeGame();
//            else PauseGame();
//        }
//    }

//    private void OnGameMusicChanged(float value)
//    {
//        if (_musicController != null) _musicController.SetVolume(value);
//        PlayerPrefs.SetFloat("GameMusicVolume", value);
//    }

//    private void OnSFXChanged(float value)
//    {
//        PlayerPrefs.SetFloat("SFXVolume", value);

//        if (_player != null)
//        {
//            _player.SetSFXVolume(value);
//        }
//    }

//    public void SaveSettingsToDisk()
//    {
//        PlayerPrefs.Save();
//    }

//    private void PauseGame()
//    {
//        if (_pausePanel != null) _pausePanel.SetActive(true);
//        Time.timeScale = 0f;
//        _isPaused = true;
//        Cursor.visible = true;
//        if (_musicController != null) _musicController.PauseMusic();
//    }

//    public void ResumeGame()
//    {
//        SaveSettingsToDisk();

//        if (_pausePanel != null) _pausePanel.SetActive(false);
//        Time.timeScale = 1f;
//        _isPaused = false;
//        Cursor.visible = false;
//        if (_musicController != null) _musicController.ResumeMusic();
//    }

//    private void RestartLevel()
//    {
//        SaveSettingsToDisk();
//        Time.timeScale = 1f;
//        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
//    }

//    private void GoToMainMenu()
//    {
//        SaveSettingsToDisk();
//        Time.timeScale = 1f;
//        SceneManager.LoadScene("MainMenu");
//    }
//}
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;

//public class PauseMenuManager : MonoBehaviour
//{
//    [Header("UI References")]

//    [SerializeField] private UIPanelFader _pausePanelFader;
//    [SerializeField] private Button _restartButton;
//    [SerializeField] private Button _mainMenuButton;

//    [SerializeField] private WinScreenController _winScreen;

//    [Header("Volume Sliders")]
//    [SerializeField] private Slider _gameMusicSlider;
//    [SerializeField] private Slider _sfxSlider;

//    [Header("Audio")]
//    [SerializeField] private MusicController _musicController;

//    private bool _isPaused = false;
//    private PlayerController _player;

//    private void Start()
//    {
//        _player = FindObjectOfType<PlayerController>();


//        if (_pausePanelFader != null) _pausePanelFader.gameObject.SetActive(false);

//        if (_restartButton != null) _restartButton.onClick.AddListener(RestartLevel);
//        if (_mainMenuButton != null) _mainMenuButton.onClick.AddListener(GoToMainMenu);

//        if (_gameMusicSlider != null)
//        {
//            _gameMusicSlider.value = PlayerPrefs.GetFloat("GameMusicVolume", 1f);
//            _gameMusicSlider.onValueChanged.AddListener(OnGameMusicChanged);
//        }

//        if (_sfxSlider != null)
//        {
//            _sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
//            _sfxSlider.onValueChanged.AddListener(OnSFXChanged);
//        }
//    }

//    private void OnDestroy()
//    {
//        if (_restartButton != null) _restartButton.onClick.RemoveListener(RestartLevel);
//        if (_mainMenuButton != null) _mainMenuButton.onClick.RemoveListener(GoToMainMenu);
//        if (_gameMusicSlider != null) _gameMusicSlider.onValueChanged.RemoveListener(OnGameMusicChanged);
//        if (_sfxSlider != null) _sfxSlider.onValueChanged.RemoveListener(OnSFXChanged);
//    }

//    private void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.Escape))
//        {
//            if (_winScreen != null && _winScreen.gameObject.activeInHierarchy) return;

//            if (_isPaused) ResumeGame();
//            else PauseGame();
//        }
//    }

//    private void OnGameMusicChanged(float value)
//    {
//        if (_musicController != null) _musicController.SetVolume(value);
//        PlayerPrefs.SetFloat("GameMusicVolume", value);
//    }

//    private void OnSFXChanged(float value)
//    {
//        PlayerPrefs.SetFloat("SFXVolume", value);

//        if (_player != null)
//        {
//            _player.SetSFXVolume(value);
//        }
//    }

//    public void SaveSettingsToDisk()
//    {
//        PlayerPrefs.Save();
//    }

//    private void PauseGame()
//    {

//        if (_pausePanelFader != null) _pausePanelFader.Show();

//        Time.timeScale = 0f;
//        _isPaused = true;
//        Cursor.visible = true;
//        if (_musicController != null) _musicController.PauseMusic();
//    }

//    public void ResumeGame()
//    {
//        SaveSettingsToDisk();


//        if (_pausePanelFader != null) _pausePanelFader.Hide();

//        Time.timeScale = 1f;
//        _isPaused = false;
//        Cursor.visible = false;
//        if (_musicController != null) _musicController.ResumeMusic();
//    }

//    private void RestartLevel()
//    {
//        SaveSettingsToDisk();
//        Time.timeScale = 1f;
//        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
//    }

//    private void GoToMainMenu()
//    {
//        SaveSettingsToDisk();
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
    [SerializeField] private UIPanelFader _pausePanelFader;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private WinScreenController _winScreen;

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

        if (_pausePanelFader != null) _pausePanelFader.gameObject.SetActive(false);

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
            if (_winScreen != null && _winScreen.gameObject.activeInHierarchy) return;

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
        if (_player != null) _player.SetSFXVolume(value);
    }

    public void SaveSettingsToDisk()
    {
        PlayerPrefs.Save();

      
        if (DatabaseManager.Instance != null)
        {
            DatabaseManager.Instance.SaveAllPendingDataToDisk();
        }
    }

    private void PauseGame()
    {
        if (_pausePanelFader != null) _pausePanelFader.Show();
        Time.timeScale = 0f;
        _isPaused = true;
        Cursor.visible = true;
        if (_musicController != null) _musicController.PauseMusic();
    }

    public void ResumeGame()
    {
        SaveSettingsToDisk();
        if (_pausePanelFader != null) _pausePanelFader.Hide();
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