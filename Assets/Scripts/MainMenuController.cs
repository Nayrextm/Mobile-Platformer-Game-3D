using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Loading Setup")]
    [SerializeField] private UIPanelFader _loadingPanelFader;

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
        if (_mainMenuPanel != null) _mainMenuPanel.SetActive(false);

        StartCoroutine(PreloadGameRoutine());
    }

    private IEnumerator PreloadGameRoutine()
    {
        if (_loadingPanelFader != null)
        {
            _loadingPanelFader.gameObject.SetActive(true);
            var canvasGroup = _loadingPanelFader.GetComponent<CanvasGroup>();
            if (canvasGroup != null) canvasGroup.alpha = 1f;
        }

        if (DatabaseManager.Instance != null)
        {
            while (!DatabaseManager.Instance.IsReady)
            {
                yield return null;
            }
        }

        UpdateSfxVolumeCache();

        yield return new WaitForSeconds(0.3f);

        if (_mainMenuPanel != null) _mainMenuPanel.SetActive(true);

        if (_loadingPanelFader != null)
        {
            _loadingPanelFader.Hide();
        }
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

    public void LoadLevelScene(string levelName)
    {
        PlayClickSound();
        StartCoroutine(LoadLevelRoutine(levelName));
    }

    private IEnumerator LoadLevelRoutine(string levelName)
    {
        if (_clickSound != null)
            yield return new WaitForSeconds(_clickSound.length);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void OpenSettings()
    {
        if (_mainMenuPanel != null) _mainMenuPanel.SetActive(false);
        if (_settingsPanelFader != null) _settingsPanelFader.Show();
    }

    public void CloseSettings()
    {
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