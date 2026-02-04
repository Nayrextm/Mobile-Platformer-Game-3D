using UnityEngine;

public class LaneRunnerZ : MonoBehaviour
{
    [Header("Налаштування Смуг")]
    public bool isLaneModeActive = false; // Чи активний режим 3 смуг
    public float laneDistance = 3.0f;     // Відстань між смугами (в метрах/юнітах)

    // 0 = Центр, -1 = Ліво, 1 = Право
    private int currentLaneIndex = 0;

    void Update()
    {
        // Якщо режим не активний, нічого не робимо (працює стандартна логіка)
        if (!isLaneModeActive) return;

        HandleInput();
    }

    void HandleInput()
    {
        // Рух вліво (A або Стрілка вліво)
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeLane(-1);
        }
        // Рух вправо (D або Стрілка вправо)
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeLane(1);
        }
    }

    void ChangeLane(int direction)
    {
        // Розраховуємо нову смугу
        int targetLane = currentLaneIndex + direction;

        // Обмежуємо значення, щоб не вийти за межі (-1, 0, 1)
        // Якщо хочеш, щоб не можна було вийти за межі:
        if (targetLane < -1 || targetLane > 1) return;

        currentLaneIndex = targetLane;

        // Виконуємо миттєву телепортацію (як у павука)
        UpdatePositionInstant();
    }

    void UpdatePositionInstant()
    {
        // Беремо поточну позицію
        Vector3 newPosition = transform.position;

        // Змінюємо тільки X (горизонталь). 
        // Якщо у тебе вертикальний рівень, змінюй newPosition.y
        newPosition.x = currentLaneIndex * laneDistance;

        // Застосовуємо телепортацію
        transform.position = newPosition;
    }

    // Метод для активації режиму з порталу
    public void ActivateLaneMode(bool activate)
    {
        isLaneModeActive = activate;

        // При вході в портал вирівнюємо гравця по центру (або найближчій смузі)
        if (activate)
        {
            currentLaneIndex = 0; // Скидаємо на центр
            UpdatePositionInstant();

            // Тут також можна викликати код для зміни камери на 3D, якщо він окремий
        }
    }
}