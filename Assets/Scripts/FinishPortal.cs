
//using UnityEngine;

//public class FinishPortal : MonoBehaviour
//{
//    public ParticleSystem finishEffect;
//    public AudioSource finishSound;

//    private void OnTriggerEnter(Collider other)
//    {
//        // ѕерев≥р€Їмо, чи зайшов гравець
//        PlayerController player = other.GetComponent<PlayerController>();

//        if (player != null)
//        {
//            // √раЇмо ефекти
//            if (finishEffect) finishEffect.Play();
//            if (finishSound) finishSound.Play();

//            // ¬икликаЇмо ф≥н≥ш через Singleton (швидше ≥ над≥йн≥ше)
//            if (LevelManager.Instance != null)
//            {
//                LevelManager.Instance.LevelFinished(player);
//            }
//        }
//    }
//}
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FinishPortal : MonoBehaviour
{
    [Header("≈фекти")]
    [SerializeField] private ParticleSystem _finishEffect;
    [SerializeField] private AudioSource _finishSound;

    private bool _isFinished = false;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;

        if (_finishSound != null)
        {
            _finishSound.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isFinished) return;

        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                _isFinished = true;

                if (_finishEffect != null) _finishEffect.Play();
                if (_finishSound != null) _finishSound.Play();

                if (LevelManager.Instance != null)
                {
                    LevelManager.Instance.LevelFinished(player);
                }
            }
        }
    }
}
