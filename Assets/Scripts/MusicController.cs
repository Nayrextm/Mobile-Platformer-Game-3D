//using UnityEngine;
//using System.Collections;

//[RequireComponent(typeof(AudioSource))]
//public class MusicController : MonoBehaviour
//{
//    private AudioSource audioSource;
//    private float userVolume = 1f; // Гучність, яку виставив гравець для рівнів

//    void Awake()
//    {
//        audioSource = GetComponent<AudioSource>();

//        // Завантажуємо налаштування саме для ГРИ
//        userVolume = PlayerPrefs.GetFloat("GameMusicVolume", 1f);
//        audioSource.volume = userVolume;
//    }

//    void Start()
//    {
//        RestartMusic();
//    }

//    public void RestartMusic()
//    {
//        StopAllCoroutines();
//        audioSource.Stop();
//        audioSource.time = 0f;
//        audioSource.volume = userVolume; // Ставимо збережену гучність
//        audioSource.Play();
//    }

//    public void StopMusic()
//    {
//        StopAllCoroutines();
//        audioSource.Stop();
//    }

//    // Для паузи
//    public void PauseMusic()
//    {
//        if (audioSource.isPlaying) audioSource.Pause();
//    }

//    public void ResumeMusic()
//    {
//        audioSource.UnPause();
//    }

//    // Затихання при смерті
//    public void FadeOutMusic()
//    {
//        StopAllCoroutines();
//        StartCoroutine(FadeOutCoroutine());
//    }

//    private IEnumerator FadeOutCoroutine()
//    {
//        float fadeSpeed = 2.0f;
//        while (audioSource.volume > 0)
//        {
//            audioSource.volume -= Time.unscaledDeltaTime * fadeSpeed;
//            yield return null;
//        }
//        audioSource.Stop();
//        audioSource.volume = userVolume; // Відновлюємо для наступного разу
//    }
//    public void SetVolume(float volume)
//    {
//        userVolume = volume;
//        if (audioSource != null)
//        {
//            audioSource.volume = userVolume;
//        }

//        // Зберігаємо налаштування (використовуємо ключ для ігрової музики)
//        PlayerPrefs.SetFloat("GameMusicVolume", userVolume);
//        PlayerPrefs.Save();
//    }
//}
//using UnityEngine;
//using System.Collections;

//[RequireComponent(typeof(AudioSource))]
//public class MusicController : MonoBehaviour
//{
//    private AudioSource _audioSource;
//    private float _userVolume = 1f;

//    private void Awake()
//    {
//        _audioSource = GetComponent<AudioSource>();
//        _userVolume = PlayerPrefs.GetFloat("GameMusicVolume", 1f);
//        _audioSource.volume = _userVolume;
//    }

//    private void Start()
//    {
//        RestartMusic();
//    }

//    public void RestartMusic()
//    {
//        StopAllCoroutines();
//        _audioSource.Stop();
//        _audioSource.time = 0f;
//        _audioSource.volume = _userVolume;
//        _audioSource.Play();
//    }

//    public void StopMusic()
//    {
//        StopAllCoroutines();
//        _audioSource.Stop();
//    }

//    public void PauseMusic()
//    {
//        if (_audioSource.isPlaying) _audioSource.Pause();
//    }

//    public void ResumeMusic()
//    {
//        _audioSource.UnPause();
//    }

//    public void FadeOutMusic()
//    {
//        StopAllCoroutines();
//        StartCoroutine(FadeOutCoroutine());
//    }

//    private IEnumerator FadeOutCoroutine()
//    {
//        float fadeSpeed = 2.0f;
//        while (_audioSource.volume > 0)
//        {
//            _audioSource.volume -= Time.unscaledDeltaTime * fadeSpeed;
//            yield return null;
//        }
//        _audioSource.Stop();
//        _audioSource.volume = _userVolume;
//    }

//    public void SetVolume(float volume)
//    {
//        _userVolume = volume;
//        if (_audioSource != null)
//        {
//            _audioSource.volume = _userVolume;
//        }
//        PlayerPrefs.SetFloat("GameMusicVolume", _userVolume);
//    }
//}
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MusicController : MonoBehaviour
{
    public enum MusicContext
    {
        Gameplay,
        Menu
    }

    [Header("Де зараз грає музика?")]
    [SerializeField] private MusicContext _context = MusicContext.Gameplay;

    private AudioSource _audioSource;
    private float _userVolume = 1f;

  
    private string VolumeKey => _context == MusicContext.Menu ? "MenuMusicVolume" : "GameMusicVolume";

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _userVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        _audioSource.volume = _userVolume;
    }

    private void Start()
    {
        if (_context == MusicContext.Gameplay)
        {
           
            RestartMusic();
        }
        else
        {
          
            if (!_audioSource.isPlaying) _audioSource.Play();
        }
    }

    public void RestartMusic()
    {
        StopAllCoroutines();
        _audioSource.Stop();
        _audioSource.time = 0f;
        _audioSource.volume = _userVolume;
        _audioSource.Play();
    }

    public void StopMusic()
    {
        StopAllCoroutines();
        _audioSource.Stop();
    }

    public void PauseMusic()
    {
        if (_audioSource.isPlaying) _audioSource.Pause();
    }

    public void ResumeMusic()
    {
        _audioSource.UnPause();
    }

    public void FadeOutMusic()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        float fadeSpeed = 2.0f;
        while (_audioSource.volume > 0)
        {
            _audioSource.volume -= Time.unscaledDeltaTime * fadeSpeed;
            yield return null;
        }
        _audioSource.Stop();
        _audioSource.volume = _userVolume;
    }

    public void SetVolume(float volume)
    {
        _userVolume = volume;
        if (_audioSource != null)
        {
            _audioSource.volume = _userVolume;
        }

       
        PlayerPrefs.SetFloat(VolumeKey, _userVolume);
    }
}
