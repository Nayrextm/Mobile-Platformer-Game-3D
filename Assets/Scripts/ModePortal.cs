using UnityEngine;



public class ModePortal : MonoBehaviour

{

    // Випадаюче меню в Інспекторі

    public enum GameMode

    {

        Cube,   // Звичайний

        Ball,   // Гравітація

        Spider, // Павук

        Ghost   // ---> НОВЕ: Режим Привида

    }



    [Header("Налаштування")]

    public GameMode targetMode; // Обери режим тут

    public AudioClip portalSound; // Звук входу



    private void OnTriggerEnter(Collider other)

    {

        // 1. Спочатку шукаємо скрипт на самому об'єкті

        PlayerController player = other.GetComponent<PlayerController>();



        // 2. Якщо не знайшли, шукаємо в батьківському об'єкті

        // (Це корисно, якщо колайдер висить на дочірньому об'єкті Visual)

        if (player == null)

            player = other.GetComponentInParent<PlayerController>();



        if (player != null)

        {

            // Перемикаємо режим

            player.SetMode(targetMode.ToString());



            // Граємо звук

            if (portalSound != null)

            {

                AudioSource.PlayClipAtPoint(portalSound, transform.position);

            }

        }

    }

}