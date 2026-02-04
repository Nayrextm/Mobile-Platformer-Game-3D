//using UnityEngine;

//public class ModeSwitchPortal : MonoBehaviour
//{
//    [Header("Налаштування")]
//    [Tooltip("True = Вхід у ранер (3D). False = Вихід у звичайний режим")]
//    public bool enterLaneMode = true;

//    private void OnTriggerEnter(Collider other)
//    {
//        // Перевіряємо, чи це гравець
//        if (other.CompareTag("Player"))
//        {
//            // Отримуємо посилання на компоненти
//            PlayerController standardMove = other.GetComponent<PlayerController>();
//            LaneRunner3D laneMove = other.GetComponent<LaneRunner3D>();
//            CameraFollow cam = FindObjectOfType<CameraFollow>();
//            Rigidbody rb = other.GetComponent<Rigidbody>();

//            if (enterLaneMode)
//            {
//                // === ВХІД У РЕЖИМ ЛІНІЙ ===

//                // 1. Вимикаємо звичайне керування
//                if (standardMove) standardMove.enabled = false;

//                // 2. Вмикаємо ранер
//                if (laneMove)
//                {
//                    laneMove.enabled = true;
//                    // Синхронізація швидкості (опціонально)
//                    if (standardMove) laneMove.forwardSpeed = standardMove.forwardSpeed;
//                }

//                // 3. Камера
//                if (cam) cam.Set3DView(true);

//                // === ФІКС ПОЛЬОТУ ===
//                // Якщо гравець летить вгору (rb.velocity.y > 0), ми це обнуляємо.
//                // Якщо він вже падає - хай падає.
//                if (rb != null && rb.velocity.y > 0)
//                {
//                    Vector3 fixedVel = rb.velocity;
//                    fixedVel.y = 0f; // Гасимо стрибок миттєво
//                    rb.velocity = fixedVel;
//                }
//            }
//            else
//            {
//                // === ВИХІД У ЗВИЧАЙНИЙ РЕЖИМ ===

//                // 1. Вимикаємо ранер
//                if (laneMove) laneMove.enabled = false;

//                // 2. СКИДАЄМО ІНЕРЦІЮ (Важливо!)
//                // Щоб гравець не продовжував летіти вперед зі швидкістю ранера
//                if (rb) rb.velocity = Vector3.zero;

//                // 3. Вмикаємо звичайне керування
//                if (standardMove) standardMove.enabled = true;

//                // 4. Камера (якщо треба повертати 2D)
//                 if (cam) cam.Set3DView(false); 
//            }
//        }
//    }
//}
using UnityEngine;

public class ModeSwitchPortal : MonoBehaviour
{
    [Header("Налаштування")]
    [Tooltip("Постав галочку, якщо це вхід у 3D тунель. Прибери, якщо це вихід у 2D.")]
    public bool enterLaneMode = true;

    private void OnTriggerEnter(Collider other)
    {
        // Реагуємо тільки на гравця
        if (other.CompareTag("Player"))
        {
           
            Rigidbody rb = other.GetComponent<Rigidbody>();

            // Кешуємо компоненти
            PlayerController standardMove = other.GetComponent<PlayerController>();
            LaneRunner3D laneMove = other.GetComponent<LaneRunner3D>();

            // Шукаємо камеру на сцені
            CameraFollow cam = FindObjectOfType<CameraFollow>();

            if (enterLaneMode)
            {
                // ============================================
                // ---> ВХІД У РЕЖИМ 3D (Lane Runner)
                // ============================================

                // 1. Вимикаємо 2D керування
                if (standardMove) standardMove.enabled = false;

                // 2. Вмикаємо 3D керування
                // (При включенні LaneRunner3D сам викличе OnEnable -> ResetRun)
                if (laneMove)
                {
                    laneMove.enabled = true;
                    // Опціонально: переносимо швидкість для плавності
                    if (standardMove) laneMove.forwardSpeed = standardMove.forwardSpeed;
                }

                // 3. Перемикаємо камеру
                if (cam) cam.Set3DView(true);

                // 4. Гасимо вертикальний стрибок, щоб гравець не перелетів портал
                if (rb != null && rb.velocity.y > 0)
                {
                    Vector3 fixedVel = rb.velocity;
                    fixedVel.y = 0f;
                    rb.velocity = fixedVel;
                }

                // 5. Вирівнюємо поворот (щоб дивився чітко вперед)
                other.transform.rotation = Quaternion.identity;
            }
            else
            {
                // ============================================
                // ---> ВИХІД У ЗВИЧАЙНИЙ РЕЖИМ (2D)
                // ============================================

                // 1. Вимикаємо 3D бігуна
                if (laneMove) laneMove.enabled = false;

                // 2. Повне скидання фізики
                if (rb)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    // Переконуємось, що фізика увімкнена
                    rb.isKinematic = false;
                }

                // 3. !!! КРИТИЧНО !!! Вирівнювання позиції
                // Якщо гравець був на лівій (-3) чи правій (3) лінії, 
                // повертаємо його в центр (0), інакше у 2D він буде висіти в повітрі
                Vector3 flatPos = other.transform.position;
                flatPos.z = 0f;
                other.transform.position = flatPos;

                // 4. Скидання обертання
                other.transform.rotation = Quaternion.identity;

                // 5. Вмикаємо 2D керування і скидаємо стани
                if (standardMove)
                {
                    standardMove.enabled = true;

                    // Важливо: скидаємо режим у "Куб", щоб прибрати перевернуту гравітацію 
                    // або стани корабля/павука, якщо вони були
                    standardMove.SetMode("Cube");

                    // Якщо у PlayerController є змінна для швидкості, відновлюємо її
                    // standardMove.forwardSpeed = standardMove.defaultSpeed; 
                }

                // 6. Камера назад у 2D
                if (cam) cam.Set3DView(false);
            }
        }
    }
}