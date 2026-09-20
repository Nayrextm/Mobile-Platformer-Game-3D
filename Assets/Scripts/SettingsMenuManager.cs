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