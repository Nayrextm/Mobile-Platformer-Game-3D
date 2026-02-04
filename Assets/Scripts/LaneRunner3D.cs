//using UnityEngine;

//public class LaneRunner3D : MonoBehaviour
//{
//    [Header("Налаштування руху")]
//    public float forwardSpeed = 15f;
//    public float laneDistance = 3.0f;

//    [Header("Ефекти")]
//    public ParticleSystem teleportEffect;

//    [Header("Налаштування фізики")]
//    public float extraGravity = 20f;

//    // Секретна послідовність руху: Центр(0) -> Право(1) -> Центр(0) -> Ліво(-1)
//    private int[] laneSequence = { 0, 1, 0, -1 };
//    private int sequenceIndex = 0;

//    private Rigidbody rb;
//    private bool isDead = false;
//    private PlayerController mainController;

//    void Awake()
//    {
//        rb = GetComponent<Rigidbody>();
//        mainController = GetComponent<PlayerController>();
//        enabled = false;
//    }

//    void OnEnable()
//    {
//        isDead = false;
//        sequenceIndex = 0;

//        // Скидаємо тільки швидкість по X та Z, Y (гравітацію) залишаємо як є
//        if (rb != null)
//        {
//            rb.isKinematic = false;
//            // Важливо: заморожуємо обертання, щоб персонаж не котився як м'яч
//            rb.constraints = RigidbodyConstraints.FreezeRotation;

//            // Вирівнюємо гравця на стартову лінію
//            SnapToLane(laneSequence[sequenceIndex]);
//        }
//    }

//    void OnDisable()
//    {
//        if (rb != null)
//        {
//            // Повертаємо вільну фізику для звичайного контролера
//            rb.constraints = RigidbodyConstraints.None;
//            // Або FreezeRotationX | FreezeRotationZ, якщо у вас 2D гра
//        }
//    }

//    void Update()
//    {
//        if (isDead) return;

//        // Зчитуємо ввід (стрибки по лініях) у Update (бо ввід може бути втрачений у FixedUpdate)
//        bool inputDetected = Input.GetButtonDown("Jump") || Input.GetMouseButtonDown(0);

//        if (inputDetected)
//        {
//            SwitchLane();
//        }
//    }

//    // Вся фізика руху перенесена сюди, щоб не було глюків з гравітацією
//    void FixedUpdate()
//    {
//        if (isDead) return;

//        // 1. Зберігаємо поточну вертикальну швидкість
//        float currentYVelocity = rb.velocity.y;

//        // 2. Додаємо штучну гравітацію (щоб не було ефекту "місяця")
//        // Це гарантує, що персонаж буде падати вниз, а не висіти
//        currentYVelocity -= extraGravity * Time.fixedDeltaTime;

//        // 3. Формуємо вектор швидкості
//        // X = стабільний біг
//        // Y = наша гравітація
//        // Z = 0 (бо ми це контролюємо через позицію)
//        Vector3 targetVelocity = new Vector3(forwardSpeed, currentYVelocity, 0);
//        rb.velocity = targetVelocity;

//        // 4. ПРИВ'ЯЗКА ДО ЛІНІЇ (Z)
//        // Ми беремо X та Y від фізики (rb.position), а Z жорстко задаємо
//        Vector3 currentPos = rb.position;
//        float targetZ = laneSequence[sequenceIndex] * laneDistance;

//        // Використовуємо Lerp для Z, щоб зміна лінії була трішки плавнішою, 
//        // або залишаємо жорстку прив'язку, якщо треба миттєво.
//        // Але тут важливо не перезаписувати Y з минулого кадру, а брати currentPos.y
//        Vector3 newPos = new Vector3(currentPos.x, currentPos.y, targetZ);

//        rb.MovePosition(newPos);
//    }

//    void SwitchLane()
//    {
//        sequenceIndex++;
//        if (sequenceIndex >= laneSequence.Length)
//        {
//            sequenceIndex = 0;
//        }

//        if (teleportEffect) teleportEffect.Play();
//    }

//    void SnapToLane(int laneIndex)
//    {
//        // Для миттєвого старту можна використати transform.position
//        Vector3 pos = transform.position;
//        pos.z = laneIndex * laneDistance;
//        transform.position = pos;
//    }

//    // === ЛОГІКА СМЕРТІ ===
//    void OnCollisionEnter(Collision collision) { CheckDamage(collision.gameObject); }
//    void OnTriggerEnter(Collider other) { CheckDamage(other.gameObject); }

//    void CheckDamage(GameObject obj)
//    {
//        if (isDead) return;

//        // Перешкоди вбивають
//        if (obj.CompareTag("Obstacle") || obj.layer == LayerMask.NameToLayer("Obstacle"))
//        {
//            HandleDeath();
//        }

//        // Портали ігноруємо тут, ними керує скрипт ModeSwitchPortal
//    }

//    void HandleDeath()
//    {
//        isDead = true;
//        rb.velocity = Vector3.zero; // Зупинка
//        rb.isKinematic = true;      // Вимкнення фізики, щоб не падав крізь світ (опціонально)

//        if (mainController != null) mainController.Die();
//    }
//}
//using UnityEngine;

//[RequireComponent(typeof(AudioSource))]
//public class LaneRunner3D : MonoBehaviour
//{
//    [Header("Налаштування руху")]
//    public float forwardSpeed = 15f;
//    public float laneDistance = 3.0f;
//    public float extraGravity = 30f; // Сильна гравітація для чіткості

//    [Header("Spider Effects")]
//    public GameObject ghostPrefab; // Сюди перетягни префаб "Привида"
//    public ParticleSystem teleportEffect;
//    public AudioClip teleportSound;

//    // Секретна послідовність руху
//    private int[] laneSequence = { 0, 1, 0, -1 };
//    private int sequenceIndex = 0;

//    private Rigidbody rb;
//    private bool isDead = false;
//    private PlayerController mainController;
//    private AudioSource audioSource;

//    void Awake()
//    {
//        rb = GetComponent<Rigidbody>();
//        mainController = GetComponent<PlayerController>();
//        audioSource = GetComponent<AudioSource>();
//        enabled = false;
//    }

//    void OnEnable()
//    {
//        isDead = false;
//        sequenceIndex = 0;

//        if (rb != null)
//        {
//            rb.isKinematic = false;
//            rb.constraints = RigidbodyConstraints.FreezeRotation;
//            SnapToLane(laneSequence[sequenceIndex]);
//        }
//    }

//    void OnDisable()
//    {
//        if (rb != null)
//        {
//            rb.constraints = RigidbodyConstraints.None;
//        }

//        // ДОДАЙ ЦЕ, якщо хочеш, щоб частинки миттєво зникали при виході з порталу
//        if (teleportEffect != null)
//        {
//            teleportEffect.Stop();
//            teleportEffect.Clear(); // Видаляє всі існуючі частинки миттєво
//        }
//    }

//    void Update()
//    {
//        if (isDead) return;

//        bool inputDetected = Input.GetButtonDown("Jump") || Input.GetMouseButtonDown(0);

//        if (inputDetected)
//        {
//            SwitchLane();
//        }
//    }

//    void FixedUpdate()
//    {
//        if (isDead) return;

//        // 1. Гравітація
//        float currentYVelocity = rb.velocity.y;
//        currentYVelocity -= extraGravity * Time.fixedDeltaTime;

//        // 2. Вектор швидкості
//        Vector3 targetVelocity = new Vector3(forwardSpeed, currentYVelocity, 0);
//        rb.velocity = targetVelocity;

//        // 3. ПРИВ'ЯЗКА ДО ЛІНІЇ (Телепортація по Z)
//        Vector3 currentPos = rb.position;
//        float targetZ = laneSequence[sequenceIndex] * laneDistance;
//        Vector3 newPos = new Vector3(currentPos.x, currentPos.y, targetZ);

//        rb.MovePosition(newPos);
//    }

//    //void SwitchLane()
//    //{
//    //    // 1. СТВОРЮЄМО ПРИВИДА НА СТАРОМУ МІСЦІ
//    //    if (ghostPrefab != null)
//    //    {
//    //        // Спавнимо копію в поточній позиції та повороті гравця
//    //        GameObject ghost = Instantiate(ghostPrefab, transform.position, transform.rotation);
//    //        // Якщо у гравця змінений масштаб, застосовуємо його до привида
//    //        ghost.transform.localScale = transform.localScale;
//    //    }

//    //    // 2. ЗМІНЮЄМО ЛІНІЮ (Логіка)
//    //    sequenceIndex++;
//    //    if (sequenceIndex >= laneSequence.Length)
//    //    {
//    //        sequenceIndex = 0;
//    //    }

//    //    // 3. ЕФЕКТИ В НОВОМУ МІСЦІ
//    //    // Оскільки фізика перемістить нас тільки в наступному FixedUpdate,
//    //    // для ефекту ми можемо трохи схитрувати і перемістити ефект вручну

//    //    if (teleportSound && audioSource)
//    //    {
//    //        audioSource.PlayOneShot(teleportSound); // Звук "Вжух"
//    //    }

//    //    if (teleportEffect)
//    //    {
//    //        // Опціонально: перемістити систему частинок на нову лінію миттєво
//    //        // teleportEffect.transform.position = ... (якщо вона не прикріплена до гравця)

//    //        teleportEffect.Play();
//    //    }
//    //}
//    void SwitchLane()
//    {
//        // 1. Зберігаємо СТАРУ позицію (для Привида)
//        Vector3 oldPosition = transform.position;

//        // 2. Рахуємо нову позицію
//        sequenceIndex++;
//        if (sequenceIndex >= laneSequence.Length) sequenceIndex = 0;

//        float targetZ = laneSequence[sequenceIndex] * laneDistance;
//        Vector3 newPosition = new Vector3(oldPosition.x, oldPosition.y, targetZ);

//        // 3. СТВОРЮЄМО ПРИВИДА (На старому місці)
//        if (ghostPrefab != null)
//        {
//            GameObject ghost = Instantiate(ghostPrefab, oldPosition, transform.rotation);
//            ghost.transform.localScale = transform.localScale;
//            Destroy(ghost, 1.0f);
//        }

//        // 4. ТЕЛЕПОРТУЄМО ГРАВЦЯ
//        transform.position = newPosition;
//        rb.position = newPosition;

//        // 5. ГРАЄМО ЗВУК
//        if (teleportSound && audioSource)
//        {
//            audioSource.PlayOneShot(teleportSound);
//        }

//        // 6. СТВОРЮЄМО ІСКРИ (INSTANTIATE)
//        // Ми створюємо нову копію ефекту прямо в новій точці гравця
//        if (teleportEffect != null)
//        {
//            // Створюємо (Instantiate) копію префабу в позиції гравця
//            ParticleSystem newEffect = Instantiate(teleportEffect, transform.position, Quaternion.identity);

//            // Якщо треба, щоб іскри були трохи вище (на рівні тіла)
//            // newEffect.transform.position += Vector3.up * 1.0f;

//            // Ця команда "випльовує" частинки з новоствореного об'єкта
//            newEffect.Emit(50);

//            // Або просто newEffect.Play(), якщо у префабі стоїть Play On Awake

//            // Важливо: знищуємо ефект через 2 секунди, щоб не засмічувати пам'ять
//            Destroy(newEffect.gameObject, 2.0f);
//        }
//    }

//    void SnapToLane(int laneIndex)
//    {
//        Vector3 pos = transform.position;
//        pos.z = laneIndex * laneDistance;
//        transform.position = pos;
//    }

//    // === ЛОГІКА СМЕРТІ ===
//    void OnCollisionEnter(Collision collision) { CheckDamage(collision.gameObject); }
//    void OnTriggerEnter(Collider other) { CheckDamage(other.gameObject); }

//    void CheckDamage(GameObject obj)
//    {
//        if (isDead) return;
//        if (obj.CompareTag("Obstacle") || obj.layer == LayerMask.NameToLayer("Obstacle"))
//        {
//            HandleDeath();
//        }
//    }

//    void HandleDeath()
//    {
//        isDead = true;
//        rb.velocity = Vector3.zero;
//        if (mainController != null) mainController.Die();
//    }


//}
using UnityEngine;
using UnityEngine.SceneManagement;

public class LaneRunner3D : MonoBehaviour
{
    [Header("Налаштування руху")]
    public float forwardSpeed = 15f;
    public float laneDistance = 3.0f; // Відстань між лініями
    public float extraGravity = 30f;  // Щоб не літав, як у космосі

    [Header("Ефекти")]
    public GameObject ghostPrefab;
    public ParticleSystem teleportEffect;
    public AudioClip teleportSound;

    // Послідовність: Центр -> Право -> Центр -> Ліво
    private int[] laneSequence = { 0, 1, 0, -1 };
    private int sequenceIndex = 0;

    private Rigidbody rb;
    private bool isDead = false;

    private PlayerController mainController;
    private AudioSource audioSource;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mainController = GetComponent<PlayerController>();
        audioSource = GetComponent<AudioSource>();

        // Скрипт вимкнений за замовчуванням (чекає порталу або чекпоінта)
        enabled = false;
    }

    // Цей метод спрацьовує, коли Portal або Checkpoint вмикають цей скрипт
    void OnEnable()
    {
        ResetRun();
    }

    void OnDisable()
    {
        // При виході з режиму повертаємо нормальну фізику
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        // Чистимо ефекти
        if (teleportEffect != null)
        {
            teleportEffect.Stop();
            teleportEffect.Clear();
        }
    }

    // Головний метод скидання параметрів
    public void ResetRun()
    {
        isDead = false;
        sequenceIndex = 0; // Завжди починаємо з центру

        if (rb != null)
        {
            rb.isKinematic = false; // Вмикаємо фізику
            rb.velocity = Vector3.zero;

            // Блокуємо обертання, щоб персонаж не котився
            rb.constraints = RigidbodyConstraints.FreezeRotation;

            // Миттєво ставимо на центральну лінію
            SnapToLane(laneSequence[sequenceIndex]);
        }
    }

    void Update()
    {
        // --- ЗАХИСТ ВІД ЗАВИСАННЯ ---
        // Якщо скрипт думає, що ми мертві, АЛЕ фізика увімкнена (LevelManager нас воскресив) -> Скидаємося!
        if (isDead)
        {
            if (rb != null && !rb.isKinematic)
            {
                ResetRun();
            }
            return;
        }
        // -----------------------------

        // Керування (Пробіл / Клік / Тап)
        if (Input.GetButtonDown("Jump") || Input.GetMouseButtonDown(0))
        {
            SwitchLane();
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;

        // 1. Штучна гравітація (щоб персонаж тримався землі)
        float currentYVelocity = rb.velocity.y;
        currentYVelocity -= extraGravity * Time.fixedDeltaTime;

        // 2. Рух вперед
        Vector3 targetVelocity = new Vector3(forwardSpeed, currentYVelocity, 0);
        rb.velocity = targetVelocity;

        // 3. Жорстка прив'язка до поточної лінії по осі Z
        Vector3 currentPos = rb.position;
        float targetZ = laneSequence[sequenceIndex] * laneDistance;

        // Використовуємо MovePosition для плавного, але точного руху
        Vector3 newPos = new Vector3(currentPos.x, currentPos.y, targetZ);
        rb.MovePosition(newPos);
    }

    void SwitchLane()
    {
        Vector3 oldPosition = transform.position;

        // Перемикаємо індекс
        sequenceIndex++;
        if (sequenceIndex >= laneSequence.Length) sequenceIndex = 0;

        float targetZ = laneSequence[sequenceIndex] * laneDistance;
        Vector3 newPosition = new Vector3(oldPosition.x, oldPosition.y, targetZ);

        // --- ЕФЕКТИ ---
        // 1. Привид (Ghost Effect)
        if (ghostPrefab != null)
        {
            GameObject ghost = Instantiate(ghostPrefab, oldPosition, transform.rotation);
            ghost.transform.localScale = transform.localScale;

            // Видаляємо зайві компоненти з привида
            Destroy(ghost.GetComponent<LaneRunner3D>());
            Destroy(ghost.GetComponent<Rigidbody>());
            Destroy(ghost.GetComponent<Collider>());

            Destroy(ghost, 0.5f);
        }

        // 2. Сама телепортація
        transform.position = newPosition;
        rb.position = newPosition;

        // 3. Звук та частки
        if (teleportSound && audioSource) audioSource.PlayOneShot(teleportSound);

        if (teleportEffect != null)
        {
            ParticleSystem newEffect = Instantiate(teleportEffect, transform.position, Quaternion.identity);
            newEffect.Emit(30);
            Destroy(newEffect.gameObject, 1.5f);
        }
    }

    void SnapToLane(int laneIndex)
    {
        Vector3 pos = transform.position;
        pos.z = laneIndex * laneDistance;
        transform.position = pos;
    }

    // === ОБРОБКА СМЕРТІ ===
    void OnCollisionEnter(Collision collision) { CheckDamage(collision.gameObject); }
    void OnTriggerEnter(Collider other) { CheckDamage(other.gameObject); }

    void CheckDamage(GameObject obj)
    {
        if (isDead) return;

        // Перевіряємо на перешкоди (Тег або Шар)
        if (obj.CompareTag("Obstacle") || obj.layer == LayerMask.NameToLayer("Obstacle"))
        {
            HandleDeath();
        }
    }

    void HandleDeath()
    {
        if (isDead) return;
        isDead = true;

        // Зупиняємо фізику
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;

        // Передаємо сигнал у головний контролер (щоб зарахувати смерть в БД)
        if (mainController != null)
        {
            mainController.Die();
        }
        else
        {
            // Аварійний перезапуск, якщо немає контролера
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}