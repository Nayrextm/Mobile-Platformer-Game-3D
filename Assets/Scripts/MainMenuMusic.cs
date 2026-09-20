using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MainMenuMusic : MonoBehaviour
{
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        float vol = PlayerPrefs.GetFloat("MenuMusicVolume", 1f);
        audioSource.volume = vol;

        if (!audioSource.isPlaying) audioSource.Play();
    }

    // Викликається з SettingsMenuManager для миттєвої зміни
    public void SetVolume(float volume)
    {
        if (audioSource != null) audioSource.volume = volume;
    }
}