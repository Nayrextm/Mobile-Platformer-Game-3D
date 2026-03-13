//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class MainMenuController : MonoBehaviour
//{
//    [Header("Panels")]
//    public GameObject settingsPanel;

//    [Header("Audio")]
//    public AudioSource sfxAudioSource; // Джерело для кліку
//    public AudioClip clickSound;

//    void Start()
//    {

//        if (settingsPanel != null) settingsPanel.SetActive(false);

//        Application.targetFrameRate = 60;
//        QualitySettings.vSyncCount = 0;
//    }

//    // Метод для програвання звуку кліку з правильною гучністю
//    private void PlayClickSound()
//    {
//        if (sfxAudioSource != null && clickSound != null)
//        {
//            // Завжди читаємо актуальне значення SFX перед програванням
//            float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);
//            sfxAudioSource.volume = sfxVol;
//            sfxAudioSource.PlayOneShot(clickSound);
//        }
//    }

//    public void LoadLevelScene()
//    {
//        PlayClickSound();
//        SceneManager.LoadScene("LevelTest"); // Назва твого рівня
//    }

//    public void OpenSettings()
//    {
//        PlayClickSound();
//        if (settingsPanel != null) settingsPanel.SetActive(true);
//    }

//    public void CloseSettings()
//    {
//        PlayClickSound();
//        if (settingsPanel != null) settingsPanel.SetActive(false);
//    }

//    public void QuitGame()
//    {
//        PlayClickSound();
//        Application.Quit();
//#if UNITY_EDITOR
//        UnityEditor.EditorApplication.isPlaying = false;
//#endif
//    }
//}
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject _settingsPanel;

    [Header("Audio")]
    [SerializeField] private AudioSource _sfxAudioSource;
    [SerializeField] private AudioClip _clickSound;

    private void Start()
    {
        if (_settingsPanel != null) _settingsPanel.SetActive(false);

      
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }

    private void PlayClickSound()
    {
        if (_sfxAudioSource != null && _clickSound != null)
        {
            _sfxAudioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            _sfxAudioSource.PlayOneShot(_clickSound);
        }
    }

    public void LoadLevelScene()
    {
        PlayClickSound();
        SceneManager.LoadScene("LevelTest");
    }

    public void OpenSettings()
    {
        PlayClickSound();
        if (_settingsPanel != null) _settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        PlayClickSound();
        if (_settingsPanel != null) _settingsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        PlayClickSound();
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}