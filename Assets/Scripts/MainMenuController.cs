
//using System.Collections;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class MainMenuController : MonoBehaviour
//{
//    [Header("UI Panels")]
//    [SerializeField] private GameObject _mainMenuPanel;
//    [SerializeField] private UIPanelFader _settingsPanelFader;

//    [Header("Audio")]
//    [SerializeField] private AudioSource _sfxAudioSource;
//    [SerializeField] private AudioClip _clickSound;

//    private float _currentSfxVolume = 1f;

//    private void Start()
//    {
//        Application.targetFrameRate = 60;
//        QualitySettings.vSyncCount = 0;

//        if (_settingsPanelFader != null) _settingsPanelFader.gameObject.SetActive(false);

//        UpdateSfxVolumeCache();
//    }

//    public void UpdateSfxVolumeCache()
//    {
//        _currentSfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
//    }

//    private void PlayClickSound()
//    {
//        if (_sfxAudioSource != null && _clickSound != null)
//        {
//            _sfxAudioSource.volume = _currentSfxVolume;
//            _sfxAudioSource.PlayOneShot(_clickSound);
//        }
//    }


//    public void LoadLevelScene(string levelName)
//    {
//        PlayClickSound();
//        StartCoroutine(LoadLevelRoutine(levelName));
//    }


//    private IEnumerator LoadLevelRoutine(string levelName)
//    {
//        if (_clickSound != null)
//            yield return new WaitForSeconds(_clickSound.length);

//        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelName);

//        while (!asyncLoad.isDone)
//        {
//            yield return null;
//        }
//    }

//    public void OpenSettings()
//    {
//        if (_mainMenuPanel != null) _mainMenuPanel.SetActive(false);
//        if (_settingsPanelFader != null) _settingsPanelFader.Show();
//    }

//    public void CloseSettings()
//    {
//        UpdateSfxVolumeCache();

//        if (_settingsPanelFader != null) _settingsPanelFader.Hide();
//        if (_mainMenuPanel != null) _mainMenuPanel.SetActive(true);
//    }

//    public void QuitGame()
//    {
//        PlayClickSound();
//        StartCoroutine(QuitGameRoutine());
//    }

//    private IEnumerator QuitGameRoutine()
//    {
//        if (_clickSound != null)
//            yield return new WaitForSeconds(_clickSound.length);

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
    [Header("Loading Setup")]
    [SerializeField] private UIPanelFader _loadingPanelFader; // Панель екрана завантаження

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

        // Ховаємо панелі на старті
        if (_settingsPanelFader != null) _settingsPanelFader.gameObject.SetActive(false);
        if (_mainMenuPanel != null) _mainMenuPanel.SetActive(false); // <--- Меню вимкнене, поки йде завантаження

        // Запускаємо професійну підготовку гри
        StartCoroutine(PreloadGameRoutine());
    }

    private IEnumerator PreloadGameRoutine()
    {
        // 1. Вмикаємо екран завантаження на максимум (без анімації, миттєво)
        if (_loadingPanelFader != null)
        {
            _loadingPanelFader.gameObject.SetActive(true);
            var canvasGroup = _loadingPanelFader.GetComponent<CanvasGroup>();
            if (canvasGroup != null) canvasGroup.alpha = 1f;
        }

        // 2. ОПТИМІЗАЦІЯ: Чекаємо, поки база даних повністю розпакується і завантажиться в кеш
        if (DatabaseManager.Instance != null)
        {
            while (!DatabaseManager.Instance.IsReady)
            {
                yield return null; // Пропускаємо кадри, поки база копіюється з APK
            }
        }

        // 3. Завантажуємо налаштування звуку та інші збереження
        UpdateSfxVolumeCache();

        // Тут також можна підвантажити: sfxAudioSource.clip, вибрані скіни м'яча, іконки і т.д.
        // Наприклад: SkinManager.Instance.ApplySelectedSkin(DatabaseManager.Instance.GetSelectedSkinID());

        // Невеликий штучний відступ (0.2-0.5 сек), щоб екран завантаження не блимнув занадто швидко, 
        // якщо база завантажилася миттєво. Це приємно для очей гравця.
        yield return new WaitForSeconds(0.3f);

        // 4. Вмикаємо головне меню у фоні
        if (_mainMenuPanel != null) _mainMenuPanel.SetActive(true);

        // 5. Плавно прибираємо екран завантаження за допомогою нашого DOTween fader-а
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