using UnityEngine;
using UnityEngine.Events; // Потрібно для подій

public class UniversalPortal : MonoBehaviour
{
    [Header("Керування Скриптами")]
    // Сюди перетягни скрипт звичайної ходьби (якщо він є на гравцеві, інакше залиш пустим)
    public MonoBehaviour old2DMovementScript;

    [Header("Налаштування Камери")]
    // Сюди можна підв'язати метод зміни камери в інспекторі
    public UnityEvent onPortalEnter;

    private bool hasTriggered = false; // Захист від подвійного спрацьовування

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return; // Якщо вже спрацював — ігноруємо

        if (other.CompareTag("Player"))
        {
            // 1. Шукаємо новий скрипт руху по смугах
            LaneRunnerZ laneScript = other.GetComponent<LaneRunnerZ>();

            // 2. Вимикаємо старий скрипт (якщо призначений)
            if (old2DMovementScript != null)
            {
                old2DMovementScript.enabled = false;
            }
            // Або пробуємо знайти його автоматично, якщо не призначили вручну:
            else
            {
                var standardMove = other.GetComponent<PlayerController>(); // ЗАМІНИ НА СВОЮ НАЗВУ
                if (standardMove != null) standardMove.enabled = false;
            }

            // 3. Вмикаємо новий скрипт
            if (laneScript != null)
            {
                laneScript.enabled = true;
                laneScript.ActivateLaneMode(true);
            }

            // 4. Перемикаємо камеру
            // Виклик події, яку ми налаштуємо в Unity
            onPortalEnter.Invoke();

            hasTriggered = true;
            Debug.Log("Портал пройдено: 3D режим + Смуги Z активовано");
        }
    }
}