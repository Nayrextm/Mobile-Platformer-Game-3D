//using UnityEngine;

//public class FinishPortal : MonoBehaviour
//{
//    public ParticleSystem finishEffect;
//    public AudioSource finishSound;

//    private void OnTriggerEnter(Collider other)
//    {
//        PlayerController player = other.GetComponent<PlayerController>();
//        if (player != null)
//        {
//            if (finishEffect) finishEffect.Play();
//            if (finishSound) finishSound.Play();

//            LevelManager lm = FindObjectOfType<LevelManager>();
//            if (lm != null)
//                lm.LevelFinished(player);
//        }
//    }
//}
using UnityEngine;

public class FinishPortal : MonoBehaviour
{
    public ParticleSystem finishEffect;
    public AudioSource finishSound;

    private void OnTriggerEnter(Collider other)
    {
        // Перевіряємо, чи зайшов гравець
        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null)
        {
            // Граємо ефекти
            if (finishEffect) finishEffect.Play();
            if (finishSound) finishSound.Play();

            // Викликаємо фініш через Singleton (швидше і надійніше)
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.LevelFinished(player);
            }
        }
    }
}
