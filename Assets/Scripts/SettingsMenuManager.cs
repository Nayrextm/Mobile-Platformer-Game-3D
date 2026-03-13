//using UnityEngine;
//using UnityEngine.UI;

//public class SettingsMenuManager : MonoBehaviour
//{
//    [Header("Sliders")]
//    public Slider menuMusicSlider; // Слайдер для музики в меню
//    public Slider gameMusicSlider; // Слайдер для музики на рівнях
//    public Slider sfxSlider;       // Слайдер для ефектів (SFX)

//    [Header("References")]
//    public MainMenuMusic mainMenuMusic; // Посилання на скрипт музики меню (щоб чути зміни відразу)

//    void Start()
//    {
//        // 1. Ініціалізація слайдера МЕНЮ
//        if (menuMusicSlider != null)
//        {
//            float val = PlayerPrefs.GetFloat("MenuMusicVolume", 1f);
//            menuMusicSlider.value = val;
//            menuMusicSlider.onValueChanged.AddListener(OnMenuVolumeChanged);
//        }

//        // 2. Ініціалізація слайдера ГРИ
//        if (gameMusicSlider != null)
//        {
//            float val = PlayerPrefs.GetFloat("GameMusicVolume", 1f);
//            gameMusicSlider.value = val;
//            gameMusicSlider.onValueChanged.AddListener(OnGameVolumeChanged);
//        }

//        // 3. Ініціалізація слайдера SFX
//        if (sfxSlider != null)
//        {
//            float val = PlayerPrefs.GetFloat("SFXVolume", 1f);
//            sfxSlider.value = val;
//            sfxSlider.onValueChanged.AddListener(OnSFXChanged);
//        }
//    }

//    // Змінюємо гучність меню (зберігаємо + застосовуємо відразу)
//    void OnMenuVolumeChanged(float value)
//    {
//        PlayerPrefs.SetFloat("MenuMusicVolume", value);
//        PlayerPrefs.Save();

//        if (mainMenuMusic != null)
//        {
//            mainMenuMusic.SetVolume(value);
//        }
//    }

//    // Змінюємо гучність гри (просто зберігаємо, застосується при старті рівня)
//    void OnGameVolumeChanged(float value)
//    {
//        PlayerPrefs.SetFloat("GameMusicVolume", value);
//        PlayerPrefs.Save();
//    }

//    // Змінюємо гучність ефектів (просто зберігаємо)
//    void OnSFXChanged(float value)
//    {
//        PlayerPrefs.SetFloat("SFXVolume", value);
//        PlayerPrefs.Save();
//    }
//}
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuManager : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider _menuMusicSlider;
    [SerializeField] private Slider _gameMusicSlider;
    [SerializeField] private Slider _sfxSlider;

    [Header("References")]
    [SerializeField] private MusicController _mainMenuMusic;

    private void Start()
    {
        if (_menuMusicSlider != null)
        {
            _menuMusicSlider.value = PlayerPrefs.GetFloat("MenuMusicVolume", 1f);
            _menuMusicSlider.onValueChanged.AddListener(OnMenuVolumeChanged);
        }

        if (_gameMusicSlider != null)
        {
            _gameMusicSlider.value = PlayerPrefs.GetFloat("GameMusicVolume", 1f);
            _gameMusicSlider.onValueChanged.AddListener(OnGameVolumeChanged);
        }

        if (_sfxSlider != null)
        {
            _sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
            _sfxSlider.onValueChanged.AddListener(OnSFXChanged);
        }
    }

    private void OnDestroy()
    {
        if (_menuMusicSlider != null) _menuMusicSlider.onValueChanged.RemoveListener(OnMenuVolumeChanged);
        if (_gameMusicSlider != null) _gameMusicSlider.onValueChanged.RemoveListener(OnGameVolumeChanged);
        if (_sfxSlider != null) _sfxSlider.onValueChanged.RemoveListener(OnSFXChanged);
    }

    private void OnMenuVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("MenuMusicVolume", value);

        if (_mainMenuMusic != null)
        {
            _mainMenuMusic.SetVolume(value);
        }
    }

    private void OnGameVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("GameMusicVolume", value);
    }

    private void OnSFXChanged(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    
    public void SaveSettingsToDisk()
    {
        PlayerPrefs.Save();
        Debug.Log("Налаштування збережено на диск!");
    }
}