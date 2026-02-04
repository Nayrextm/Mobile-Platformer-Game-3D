using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MusicController : MonoBehaviour
{
    private AudioSource audioSource;
    private float userVolume = 1f; // Гучність, яку виставив гравець для рівнів

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // Завантажуємо налаштування саме для ГРИ
        userVolume = PlayerPrefs.GetFloat("GameMusicVolume", 1f);
        audioSource.volume = userVolume;
    }

    void Start()
    {
        RestartMusic();
    }

    public void RestartMusic()
    {
        StopAllCoroutines();
        audioSource.Stop();
        audioSource.time = 0f;
        audioSource.volume = userVolume; // Ставимо збережену гучність
        audioSource.Play();
    }

    public void StopMusic()
    {
        StopAllCoroutines();
        audioSource.Stop();
    }

    // Для паузи
    public void PauseMusic()
    {
        if (audioSource.isPlaying) audioSource.Pause();
    }

    public void ResumeMusic()
    {
        audioSource.UnPause();
    }

    // Затихання при смерті
    public void FadeOutMusic()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        float fadeSpeed = 2.0f;
        while (audioSource.volume > 0)
        {
            audioSource.volume -= Time.unscaledDeltaTime * fadeSpeed;
            yield return null;
        }
        audioSource.Stop();
        audioSource.volume = userVolume; // Відновлюємо для наступного разу
    }
    public void SetVolume(float volume)
    {
        userVolume = volume;
        if (audioSource != null)
        {
            audioSource.volume = userVolume;
        }

        // Зберігаємо налаштування (використовуємо ключ для ігрової музики)
        PlayerPrefs.SetFloat("GameMusicVolume", userVolume);
        PlayerPrefs.Save();
    }
}

