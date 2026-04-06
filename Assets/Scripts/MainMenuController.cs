
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class MainMenuController : MonoBehaviour
//{
//    [Header("Panels")]
//    [SerializeField] private GameObject _settingsPanel;

//    [Header("Audio")]
//    [SerializeField] private AudioSource _sfxAudioSource;
//    [SerializeField] private AudioClip _clickSound;

//    private void Start()
//    {
//        if (_settingsPanel != null) _settingsPanel.SetActive(false);


//        Application.targetFrameRate = 60;
//        QualitySettings.vSyncCount = 0;
//    }

//    private void PlayClickSound()
//    {
//        if (_sfxAudioSource != null && _clickSound != null)
//        {
//            _sfxAudioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
//            _sfxAudioSource.PlayOneShot(_clickSound);
//        }
//    }

//    public void LoadLevelScene()
//    {
//        PlayClickSound();
//        SceneManager.LoadScene("Synthwave");
//    }

//    public void OpenSettings()
//    {
//        PlayClickSound();
//        if (_settingsPanel != null) _settingsPanel.SetActive(true);
//    }

//    public void CloseSettings()
//    {
//        PlayClickSound();
//        if (_settingsPanel != null) _settingsPanel.SetActive(false);
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
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Level Loading")]
    [Tooltip("Назва сцени для завантаження (наприклад, Synthwave)")]
    [SerializeField] private string _levelToLoad = "Synthwave";

    [Header("UI Panels")]
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private UIPanelFader _settingsPanelFader;

    [Header("Audio")]
    [SerializeField] private AudioSource _sfxAudioSource;
    [SerializeField] private AudioClip _clickSound;

    
    private float _currentSfxVolume = 1f;

    private void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        if (_settingsPanelFader != null) _settingsPanelFader.gameObject.SetActive(false);

       
        UpdateSfxVolumeCache();
    }

    
    public void UpdateSfxVolumeCache()
    {
        _currentSfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    private void PlayClickSound()
    {
        if (_sfxAudioSource != null && _clickSound != null)
        {
            _sfxAudioSource.volume = _currentSfxVolume; 
            _sfxAudioSource.PlayOneShot(_clickSound);
        }
    }

    public void LoadLevelScene()
    {
        PlayClickSound();
        StartCoroutine(LoadLevelRoutine());
    }

    private IEnumerator LoadLevelRoutine()
    {
        if (_clickSound != null)
            yield return new WaitForSeconds(_clickSound.length);

        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_levelToLoad);

        
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    

    public void OpenSettings()
    {
        //PlayClickSound();

        if (_mainMenuPanel != null) _mainMenuPanel.SetActive(false);
        if (_settingsPanelFader != null) _settingsPanelFader.Show();
    }

    public void CloseSettings()
    {
        //PlayClickSound();

        
        UpdateSfxVolumeCache();

        if (_settingsPanelFader != null) _settingsPanelFader.Hide();
        if (_mainMenuPanel != null) _mainMenuPanel.SetActive(true);
    }

    

    public void QuitGame()
    {
        PlayClickSound();
        StartCoroutine(QuitGameRoutine());
    }

    private IEnumerator QuitGameRoutine()
    {
        if (_clickSound != null)
            yield return new WaitForSeconds(_clickSound.length);

        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}