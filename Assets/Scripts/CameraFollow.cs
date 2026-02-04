



//public class CameraFollow : MonoBehaviour

//{

//    public Transform target; // Сюди перетягни Player



//    [Header("Налаштування")]

//    public Vector3 offset = new Vector3(-8f, 3f, 0f);

//    public float smoothTime = 0.25f; // Чим більше число, тим "лінивіша" камера (спробуй 0.2 - 0.3)

//    public bool lockY = true;



//    private Vector3 velocity = Vector3.zero; // Технічна змінна для SmoothDamp



//    void LateUpdate()

//    {

//        if (target == null) return;



//        // 1. Визначаємо бажану позицію

//        Vector3 desiredPosition = target.position + offset;



//        // Якщо треба заблокувати висоту (щоб камера не стрибала разом з гравцем)

//        if (lockY)

//        {

//            desiredPosition.y = transform.position.y;

//        }



//        // 2. Використовуємо SmoothDamp замість Lerp

//        // Це прибирає різкість і додає інерцію

//        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);



//        // 3. Дивимось трохи перед гравцем

//        transform.LookAt(target.position + Vector3.up * 1.2f);

//    }



//    // Цей метод викликаємо при смерті, щоб камера не "летіла" назад, а телепортувалась

//    public void ResetCamera()

//    {

//        if (target != null)

//        {

//            Vector3 desiredPosition = target.position + offset;

//            if (lockY) desiredPosition.y = transform.position.y;



//            transform.position = desiredPosition;

//            velocity = Vector3.zero; // Скидаємо інерцію

//        }

//    }

//}
//using UnityEngine;

//public class CameraFollow : MonoBehaviour
//{
//    public Transform target;

//    [Header("Загальна плавність")]
//    public float smoothTime = 0.15f;
//    public float rotationSpeed = 5f;

//    [Header("Налаштування 2D")]
//    public Vector3 offset2D = new Vector3(-8f, 3f, -10f);
//    public Vector3 rotation2D = new Vector3(0f, 0f, 0f);
//    public bool lockY_2D = true;

//    [Header("Налаштування 3D")]
//    public Vector3 offset3D = new Vector3(-6f, 4f, 0f);
//    public Vector3 rotation3D = new Vector3(15f, 0f, 0f);

//    private bool is3DMode = false;
//    private Vector3 velocity = Vector3.zero;

//    // ---> НОВЕ: Для дзеркальної гравітації
//    private float targetGravityMult = 1f; // 1 = низ, -1 = верх
//    private float currentGravityMult = 1f;

//    void LateUpdate()
//    {
//        if (target == null) return;

//        // Плавно змінюємо множник гравітації (від 1 до -1)
//        currentGravityMult = Mathf.Lerp(currentGravityMult, targetGravityMult, Time.deltaTime * 5f);

//        Vector3 desiredPosition;
//        Quaternion targetRotation;

//        // Визначаємо базові налаштування (2D або 3D)
//        Vector3 baseOffset = is3DMode ? offset3D : offset2D;
//        Vector3 baseRotEuler = is3DMode ? rotation3D : rotation2D;

//        // ---> МАГІЯ ВІДДЗЕРКАЛЕННЯ <---
//        // 1. Інвертуємо висоту (Y) залежно від гравітації
//        Vector3 finalOffset = baseOffset;
//        finalOffset.y *= currentGravityMult;

//        // 2. Інвертуємо кут нахилу (Pitch), щоб дивитися знизу вгору
//        Vector3 finalRotEuler = baseRotEuler;
//        finalRotEuler.x *= currentGravityMult;

//        if (is3DMode)
//        {
//            // 3D РЕЖИМ
//            desiredPosition = target.position + finalOffset;
//            // Згладжуємо Z для плавності при зміні смуг
//            desiredPosition.z = Mathf.Lerp(transform.position.z, target.position.z + finalOffset.z, Time.deltaTime * 10f);

//            targetRotation = Quaternion.Euler(finalRotEuler);
//        }
//        else
//        {
//            // 2D РЕЖИМ
//            desiredPosition = target.position + finalOffset;

//            if (lockY_2D)
//            {
//                // Фіксуємо Y відносно гравітації
//                // Якщо гравітація звичайна -> offset.y, якщо перевернута -> -offset.y
//                desiredPosition.y = target.position.y + finalOffset.y;
//            }

//            desiredPosition.z = offset2D.z;
//            targetRotation = Quaternion.Euler(finalRotEuler);
//        }

//        // Рух
//        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
//        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
//    }

//    public void Set3DView(bool active)
//    {
//        is3DMode = active;
//    }

//    // ---> НОВИЙ МЕТОД: Викликаємо з PlayerController при зміні гравітації
//    public void SetGravityFlipped(bool isFlipped)
//    {
//        targetGravityMult = isFlipped ? -1f : 1f;
//    }

//    public void ResetCamera()
//    {
//        if (target != null)
//        {
//            is3DMode = false;
//            targetGravityMult = 1f; // Скидаємо гравітацію камери
//            currentGravityMult = 1f;

//            Vector3 startPos = target.position + offset2D;
//            startPos.z = offset2D.z;

//            transform.position = startPos;
//            transform.rotation = Quaternion.Euler(rotation2D);
//            velocity = Vector3.zero;
//        }
//    }
//}
//using UnityEngine;

//public class CameraFollow : MonoBehaviour
//{
//    public Transform target;

//    [Header("Загальна плавність")]
//    public float smoothTime = 0.15f;
//    public float rotationSpeed = 5f;

//    [Header("Налаштування перевороту")]
//    // Збільшив швидкість до 20 (було 5). 
//    // Можна ставити 50, якщо хочете майже миттєвий переворот.
//    public float gravityFlipSpeed = 20f;

//    [Header("Налаштування 2D")]
//    public Vector3 offset2D = new Vector3(-8f, 3f, -10f);
//    public Vector3 rotation2D = new Vector3(0f, 0f, 0f);
//    public bool lockY_2D = true;

//    [Header("Налаштування 3D")]
//    public Vector3 offset3D = new Vector3(-6f, 4f, 0f);
//    public Vector3 rotation3D = new Vector3(15f, 0f, 0f);

//    private bool is3DMode = false;
//    private Vector3 velocity = Vector3.zero;

//    private float targetGravityMult = 1f;
//    private float currentGravityMult = 1f;

//    void LateUpdate()
//    {
//        if (target == null) return;

//        // 1. Прискорений перехід гравітації
//        currentGravityMult = Mathf.MoveTowards(currentGravityMult, targetGravityMult, Time.deltaTime * gravityFlipSpeed);

//        Vector3 desiredPosition;
//        Quaternion targetRotation;

//        Vector3 baseOffset = is3DMode ? offset3D : offset2D;
//        Vector3 baseRotEuler = is3DMode ? rotation3D : rotation2D;

//        // 2. Інверсія
//        Vector3 finalOffset = baseOffset;
//        finalOffset.y *= currentGravityMult;

//        Vector3 finalRotEuler = baseRotEuler;
//        finalRotEuler.x *= currentGravityMult;

//        if (is3DMode)
//        {
//            desiredPosition = target.position + finalOffset;
//            desiredPosition.z = Mathf.Lerp(transform.position.z, target.position.z + finalOffset.z, Time.deltaTime * 10f);
//            targetRotation = Quaternion.Euler(finalRotEuler);
//        }
//        else
//        {
//            desiredPosition = target.position + finalOffset;

//            if (lockY_2D)
//            {
//                // Фіксуємо Y залежно від напрямку гравітації
//                desiredPosition.y = target.position.y + finalOffset.y;
//            }

//            desiredPosition.z = offset2D.z;
//            targetRotation = Quaternion.Euler(finalRotEuler);
//        }

//        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
//        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
//    }

//    public void Set3DView(bool active) => is3DMode = active;

//    public void SetGravityFlipped(bool isFlipped)
//    {
//        targetGravityMult = isFlipped ? -1f : 1f;
//    }

//    public void ResetCamera()
//    {
//        if (target != null)
//        {
//            is3DMode = false;
//            targetGravityMult = 1f;
//            currentGravityMult = 1f; // Миттєве скидання

//            Vector3 startPos = target.position + offset2D;
//            startPos.z = offset2D.z;

//            transform.position = startPos;
//            transform.rotation = Quaternion.Euler(rotation2D);
//            velocity = Vector3.zero;
//        }
//    }
//}
//using UnityEngine;

//public class CameraFollow : MonoBehaviour
//{
//    public Transform target;

//    [Header("Загальна плавність")]
//    public float smoothTime = 0.15f;
//    public float rotationSpeed = 5f;

//    [Header("Налаштування перевороту")]
//    public float gravityFlipSpeed = 20f;

//    [Header("Налаштування 2D")]
//    public Vector3 offset2D = new Vector3(-8f, 3f, -10f);
//    public Vector3 rotation2D = new Vector3(0f, 0f, 0f);
//    public bool lockY_2D = true;

//    [Header("Налаштування 3D")]
//    public Vector3 offset3D = new Vector3(-6f, 4f, 0f);
//    public Vector3 rotation3D = new Vector3(15f, 0f, 0f);

//    // НОВЕ: Жорстка прив'язка Z у 3D режимі (щоб не було дрейфу при телепорті)
//    public bool lockZIn3D = true;

//    private bool is3DMode = false;
//    private Vector3 velocity = Vector3.zero;

//    private float targetGravityMult = 1f;
//    private float currentGravityMult = 1f;

//    void LateUpdate()
//    {
//        if (target == null) return;

//        // 1. Прискорений перехід гравітації
//        currentGravityMult = Mathf.MoveTowards(currentGravityMult, targetGravityMult, Time.deltaTime * gravityFlipSpeed);

//        Vector3 desiredPosition;
//        Quaternion targetRotation;

//        Vector3 baseOffset = is3DMode ? offset3D : offset2D;
//        Vector3 baseRotEuler = is3DMode ? rotation3D : rotation2D;

//        // 2. Інверсія
//        Vector3 finalOffset = baseOffset;
//        finalOffset.y *= currentGravityMult;

//        Vector3 finalRotEuler = baseRotEuler;
//        finalRotEuler.x *= currentGravityMult;

//        if (is3DMode)
//        {
//            // === ВИПРАВЛЕННЯ ТУТ ===
//            // Ми більше не робимо Lerp для Z окремо, це ламало SmoothDamp

//            Vector3 targetPos = target.position + finalOffset;

//            if (lockZIn3D)
//            {
//                // Варіант 1: X і Y плавні, Z - миттєвий (ідеально для телепортів)
//                float smoothX = Mathf.SmoothDamp(transform.position.x, targetPos.x, ref velocity.x, smoothTime);
//                float smoothY = Mathf.SmoothDamp(transform.position.y, targetPos.y, ref velocity.y, smoothTime);

//                // Z беремо напряму від цілі
//                desiredPosition = new Vector3(smoothX, smoothY, targetPos.z);

//                // Оскільки ми порахували вручну, присвоюємо напряму
//                transform.position = desiredPosition;
//            }
//            else
//            {
//                // Варіант 2: Все плавно через SmoothDamp (може трохи відставати)
//                desiredPosition = targetPos;
//                transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
//            }

//            targetRotation = Quaternion.Euler(finalRotEuler);
//        }
//        else // 2D Mode
//        {
//            desiredPosition = target.position + finalOffset;

//            if (lockY_2D)
//            {
//                desiredPosition.y = target.position.y + finalOffset.y;
//            }

//            desiredPosition.z = offset2D.z;
//            targetRotation = Quaternion.Euler(finalRotEuler);

//            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
//        }

//        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
//    }

//    public void Set3DView(bool active) => is3DMode = active;

//    public void SetGravityFlipped(bool isFlipped)
//    {
//        targetGravityMult = isFlipped ? -1f : 1f;
//    }

//    public void ResetCamera()
//    {
//        if (target != null)
//        {
//            is3DMode = false;
//            targetGravityMult = 1f;
//            currentGravityMult = 1f;
//            Vector3 startPos = target.position + offset2D;
//            startPos.z = offset2D.z;
//            transform.position = startPos;
//            transform.rotation = Quaternion.Euler(rotation2D);
//            velocity = Vector3.zero;
//        }
//    }
//}
//using UnityEngine;

//public class CameraFollow : MonoBehaviour
//{
//    [Header("Ціль")]
//    public Transform target;

//    [Header("Налаштування 2D (Side View)")]
//    public Vector3 offset2D = new Vector3(0, 1, -15); // Відсунь подалі для ефекту 2D
//    public Vector3 rotation2D = new Vector3(0, 0, 0);
//    public float fov2D = 30f; // Менший кут робить картинку більш "плоскою" (як 2D)

//    [Header("Налаштування 3D (Runner View)")]
//    public Vector3 offset3D = new Vector3(0, 5, -8);
//    public Vector3 rotation3D = new Vector3(25, 0, 0);
//    public float fov3D = 60f; // Ширший кут для динаміки

//    [Header("Налаштування Плавнoсті")]
//    public float smoothTime = 0.15f; 
//    public float rotationSpeed = 5f;
//    public float fovSpeed = 5f;

//    // Внутрішні змінні
//    private Vector3 currentVelocity; 
//    private Vector3 currentOffset;
//    private bool is3DMode = false;
//    private Camera cam;
//    private bool isGravityFlipped = false;

//    void Start()
//    {
//        cam = GetComponent<Camera>();

//        // Примусово вимикаємо ортографію, бо ми вирішили її не юзати
//        cam.orthographic = false;

//        // Стартова ініціалізація
//        currentOffset = offset2D; 
//        is3DMode = false;
//    }

//    void LateUpdate()
//    {
//        if (target == null) return;

//        // 1. Визначаємо цільовий офсет
//        Vector3 targetOffsetVal = is3DMode ? offset3D : offset2D;

//        // Інверсія для гравітації (тільки в 2D режимі)
//        if (!is3DMode && isGravityFlipped)
//        {
//             targetOffsetVal.y = -targetOffsetVal.y;
//        }

//        // 2. Плавна зміна Офсету
//        currentOffset = Vector3.Lerp(currentOffset, targetOffsetVal, fovSpeed * Time.deltaTime);

//        // 3. Рух (SmoothDamp для позиції)
//        Vector3 targetPosition = target.position + currentOffset;
//        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);

//        // 4. Поворот
//        Quaternion targetRot;
//        if (is3DMode)
//        {
//            targetRot = Quaternion.Euler(rotation3D);
//        }
//        else
//        {
//            Vector3 currentRot2D = rotation2D;
//            // Переворот камери при зміні гравітації
//            if (isGravityFlipped) currentRot2D.z = 180;
//            targetRot = Quaternion.Euler(currentRot2D);
//        }
//        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);

//        // 5. Плавна зміна FOV (Тепер працює для обох режимів)
//        float targetFOV = is3DMode ? fov3D : fov2D;
//        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, fovSpeed * Time.deltaTime);
//    }

//    // === PUBLIC METHODS ===

//    public void Set3DView(bool enable3D)
//    {
//        is3DMode = enable3D;
//        currentVelocity = Vector3.zero; // Скидаємо інерцію для чіткого переходу
//    }

//    public void SetGravityFlipped(bool flipped)
//    {
//        isGravityFlipped = flipped;
//    }

//    public void ResetCamera()
//    {
//        is3DMode = false;
//        isGravityFlipped = false;
//        if (target != null)
//        {
//            transform.position = target.position + offset2D;
//            currentOffset = offset2D;
//        }
//    }
//}
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Ціль")]
    public Transform target;

    [Header("Налаштування 2D (Side View)")]
    public Vector3 offset2D = new Vector3(0, 1, -15);
    public Vector3 rotation2D = new Vector3(0, 0, 0);
    public float fov2D = 30f;

    [Header("Налаштування 3D (Runner View)")]
    public Vector3 offset3D = new Vector3(0, 4, -8); // Висота камери в 3D
    public Vector3 rotation3D = new Vector3(20, 0, 0); // Кут нахилу в 3D
    public float fov3D = 60f;

    [Header("Налаштування Плавності")]
    public float smoothTime = 0.1f;
    public float rotationSpeed = 5f;
    public float fovSpeed = 5f;

    // Внутрішні змінні
    private Vector3 currentVelocity;
    private Vector3 currentOffset;
    private bool is3DMode = false;
    private Camera cam;
    private bool isGravityFlipped = false;

    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = false;
        currentOffset = offset2D;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 1. Вибираємо базовий офсет (2D або 3D)
        Vector3 baseOffset = is3DMode ? offset3D : offset2D;
        Vector3 finalTargetOffset = baseOffset;

        // 2. ЛОГІКА ГРАВІТАЦІЇ (Виправлення видимості за стелею)
        if (isGravityFlipped)
        {
            // Інвертуємо висоту (Y). 
            // Якщо ми біжимо по стелі, камера опускається "в яму" (візуально це буде над головою перевернутого гравця)
            finalTargetOffset.y = -baseOffset.y;
        }

        // 3. Плавна зміна Офсету
        currentOffset = Vector3.Lerp(currentOffset, finalTargetOffset, fovSpeed * Time.deltaTime);

        // 4. Позиція
        Vector3 targetPosition = target.position + currentOffset;

        // (Опціонально) Якщо в 3D ранері треба тримати центр екрану:
        // if (is3DMode) targetPosition.x = 0; 

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);

        // 5. Поворот (Rotation)
        Quaternion targetRot;

        if (is3DMode)
        {
            Vector3 targetEuler = rotation3D;

            if (isGravityFlipped)
            {
                // Якщо гравітація перевернута в 3D:
                // 1. Інвертуємо нахил (щоб дивитись знизу-вверх відносно світу)
                targetEuler.x = -rotation3D.x;
                // 2. Перевертаємо камеру догори ногами (щоб стеля стала підлогою)
                targetEuler.z = 180f;
            }
            else
            {
                targetEuler.z = 0f;
            }

            targetRot = Quaternion.Euler(targetEuler);
        }
        else // 2D Mode
        {
            Vector3 targetEuler = rotation2D;
            // У 2D просто перевертаємо по Z
            if (isGravityFlipped) targetEuler.z = 180f;
            else targetEuler.z = 0f;

            targetRot = Quaternion.Euler(targetEuler);
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);

        // 6. FOV
        float targetFOV = is3DMode ? fov3D : fov2D;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, fovSpeed * Time.deltaTime);
    }

    public void Set3DView(bool enable3D)
    {
        is3DMode = enable3D;
    }

    public void SetGravityFlipped(bool flipped)
    {
        isGravityFlipped = flipped;
    }

    public void ResetCamera()
    {
        is3DMode = false;
        isGravityFlipped = false;
        if (target != null)
        {
            transform.position = target.position + offset2D;
            transform.rotation = Quaternion.Euler(rotation2D);
            currentOffset = offset2D;
        }
    }
}
