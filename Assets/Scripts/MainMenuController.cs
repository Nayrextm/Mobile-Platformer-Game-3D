using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject settingsPanel;

    [Header("Audio")]
    public AudioSource sfxAudioSource; // Джерело для кліку
    public AudioClip clickSound;

    void Start()
    {

        if (settingsPanel != null) settingsPanel.SetActive(false);

        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }

    // Метод для програвання звуку кліку з правильною гучністю
    private void PlayClickSound()
    {
        if (sfxAudioSource != null && clickSound != null)
        {
            // Завжди читаємо актуальне значення SFX перед програванням
            float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);
            sfxAudioSource.volume = sfxVol;
            sfxAudioSource.PlayOneShot(clickSound);
        }
    }

    public void LoadLevelScene()
    {
        PlayClickSound();
        SceneManager.LoadScene("LevelTest"); // Назва твого рівня
    }

    public void OpenSettings()
    {
        PlayClickSound();
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        PlayClickSound();
        if (settingsPanel != null) settingsPanel.SetActive(false);
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