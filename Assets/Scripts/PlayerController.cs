
//using System.Collections;
//using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(AudioSource))]
//public class PlayerController : MonoBehaviour
//{
//    [Header("Movement")]
//    public float forwardSpeed = 10f;
//    public float jumpForce = 12f;
//    public int maxJumps = 1;

//    public float jumpBufferTime = 0.15f;
//    public float coyoteTime = 0.1f;

//    [Header("Ground Check")]
//    public LayerMask groundMask;
//    public float groundCheckDistance = 0.6f;
//    public Vector3 groundCheckOffset = Vector3.zero;

//    [Header("Effects")]
//    public ParticleSystem jumpParticles;
//    public ParticleSystem deathParticles;

//    [Header("Audio SFX")]
//    public AudioClip jumpSfx;
//    public AudioClip deathSfx;

//    private Rigidbody rb;
//    private Collider myCollider;
//    private AudioSource audioSource;
//    private LevelManager levelManager;

//    // ---> ДОДАНО ПРОПУЩЕНУ ЗМІННУ ТУТ <---
//    private float defaultSpeed;

//    private int jumpsLeft;
//    private bool isGrounded;
//    private bool isDead = false;
//    private Vector3 originalScale;

//    private float jumpBufferCounter;
//    private float coyoteTimeCounter;
//    private bool jumpRequested = false;

//    void Awake()
//    {
//        rb = GetComponent<Rigidbody>();
//        myCollider = GetComponent<Collider>();
//        audioSource = GetComponent<AudioSource>();
//        levelManager = FindObjectOfType<LevelManager>();

//        rb.interpolation = RigidbodyInterpolation.Interpolate;
//        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

//        if (audioSource != null)
//        {
//            float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);
//            audioSource.volume = sfxVol;
//        }

//        // Тут ми її запам'ятовуємо
//        defaultSpeed = forwardSpeed;
//        originalScale = transform.localScale;
//    }

//    void Update()
//    {
//        if (isDead) return;

//        if (isGrounded)
//        {
//            coyoteTimeCounter = coyoteTime;
//            jumpsLeft = maxJumps;
//        }
//        else
//        {
//            coyoteTimeCounter -= Time.deltaTime;
//        }

//        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) ||
//           (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
//        {
//            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
//            {
//                jumpBufferCounter = jumpBufferTime;
//            }
//        }
//        else
//        {
//            jumpBufferCounter -= Time.deltaTime;
//        }

//        if (jumpBufferCounter > 0f && (coyoteTimeCounter > 0f || jumpsLeft > 0))
//        {
//            jumpRequested = true;

//            if (coyoteTimeCounter <= 0f)
//            {
//                jumpsLeft--;
//            }

//            jumpBufferCounter = 0f;
//        }
//    }

//    void FixedUpdate()
//    {
//        if (isDead)
//        {
//            rb.velocity = Vector3.zero;
//            return;
//        }
//        Vector3 currentVel = rb.velocity;
//        currentVel.x = forwardSpeed;

//        if (jumpRequested)
//        {
//            currentVel.y = jumpForce;

//            if (jumpParticles) jumpParticles.Play();
//            if (audioSource && jumpSfx) audioSource.PlayOneShot(jumpSfx);

//            jumpRequested = false;
//            coyoteTimeCounter = 0f;
//        }

//        rb.velocity = currentVel;

//        CheckFrontCollision();
//        GroundCheck();
//    }

//    public void SetSFXVolume(float volume)
//    {
//        if (audioSource != null) audioSource.volume = volume;
//    }

//    void GroundCheck()
//    {
//        RaycastHit hit;
//        Vector3 origin = transform.position + groundCheckOffset;
//        isGrounded = Physics.Raycast(origin, Vector3.down, out hit, groundCheckDistance, groundMask);
//    }

//    void CheckFrontCollision()
//    {
//        if (isDead) return;

//        RaycastHit hit;
//        Vector3 origin = transform.position + Vector3.up * 0.5f;

//        if (Physics.Raycast(origin, Vector3.right, out hit, 0.6f))
//        {
//            if (!hit.collider.CompareTag("Ground"))
//            {
//                if (hit.collider.CompareTag("Obstacle") || hit.collider.CompareTag("Wall"))
//                    Die();
//            }
//        }
//    }

//    void OnCollisionEnter(Collision collision)
//    {
//        if (isDead) return;
//        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Wall"))
//            Die();
//    }

//    void OnTriggerEnter(Collider other)
//    {
//        if (isDead) return;
//        if (other.CompareTag("Obstacle") || other.CompareTag("Wall"))
//            Die();
//    }

//    void Die()
//    {
//        if (isDead) return;
//        isDead = true;

//        if (myCollider != null) myCollider.enabled = false;
//        rb.detectCollisions = false;
//        rb.isKinematic = true;
//        rb.velocity = Vector3.zero;

//        if (deathParticles) Instantiate(deathParticles, transform.position, Quaternion.identity);
//        if (audioSource && deathSfx) audioSource.PlayOneShot(deathSfx);

//        StartCoroutine(DeathAnimation());
//    }

//    IEnumerator DeathAnimation()
//    {
//        float t = 0f;
//        Vector3 startScale = transform.localScale;

//        while (t < 0.3f)
//        {
//            t += Time.unscaledDeltaTime;
//            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t / 0.3f);
//            yield return null;
//        }
//        transform.localScale = Vector3.zero;

//        if (levelManager) levelManager.PlayerDied(this);
//    }

//    public void RespawnAt(Transform spawnPoint)
//    {
//        // 1. Спочатку скидаємо стани, щоб не було повторних тригерів
//        isDead = false;
//        jumpRequested = false;
//        jumpBufferCounter = 0f;
//        coyoteTimeCounter = 0f;
//        forwardSpeed = defaultSpeed; // Повертаємо швидкість

//        // 2. ВИМИКАЄМО ФІЗИКУ ПОВНІСТЮ
//        // Це критично важливо! Ми робимо об'єкт "кінематичним", 
//        // щоб він перестав реагувати на зіткнення під час телепортації.
//        rb.isKinematic = true;
//        rb.velocity = Vector3.zero;
//        rb.detectCollisions = false; // Додатковий захист

//        // 3. ТЕЛЕПОРТАЦІЯ
//        transform.position = spawnPoint.position;
//        transform.rotation = spawnPoint.rotation;
//        transform.localScale = originalScale;

//        // 4. СИНХРОНІЗАЦІЯ (Магія Unity)
//        // Ця команда змушує фізичний рушій миттєво оновити позицію колайдерів,
//        // не чекаючи наступного кадру.
//        Physics.SyncTransforms();

//        // 5. ВМИКАЄМО ВСЕ НАЗАД
//        if (myCollider != null) myCollider.enabled = true;

//        rb.isKinematic = false;
//        rb.detectCollisions = true; // Вмикаємо зіткнення

//        // Скидаємо стрибки
//        jumpsLeft = maxJumps;
//    }

//    public void Win()
//    {
//        rb.velocity = Vector3.zero;
//        rb.isKinematic = true;
//        if (myCollider != null) myCollider.enabled = false;
//        enabled = false;
//    }

//    public void ResetJumpsFromPad()
//    {
//        jumpsLeft = maxJumps;
//    }
//}
//using System.Collections;
//using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(AudioSource))]
//[RequireComponent(typeof(LineRenderer))]
//public class PlayerController : MonoBehaviour
//{
//    [Header("Visuals")]
//    public Transform visualModel;
//    public float rotationSpeed = 360f;
//    public TrailRenderer trail;

//    [Header("Ghost Settings")]
//    public float ghostAlpha = 0.3f;

//    // Назви шарів
//    public string safeLayerName = "Ground";
//    public string deadlyLayerName = "Obstacle";
//    public string wallLayerName = "Wall"; // Шар стін (безпечний для всіх, прохідний для Привида)

//    private Renderer[] modelRenderers;
//    private bool isPhasing = false;

//    // ID шарів
//    private int playerLayer;
//    private int safeLayer;
//    private int deadlyLayer;
//    private int wallLayer;

//    [Header("Spider Settings")]
//    public float spiderBeamDuration = 0.1f;
//    private LineRenderer spiderLine;

//    [Header("Movement")]
//    public float forwardSpeed = 10f;
//    public float jumpForce = 12f;
//    public int maxJumps = 1;

//    public float jumpBufferTime = 0.15f;
//    public float coyoteTime = 0.1f;

//    [Header("Modes Settings")]
//    public float gravityForce = 30f;

//    private bool isGravityMode = false;
//    private bool isSpiderMode = false;
//    private bool isGhostMode = false;
//    private float gravityScale = 1f;

//    [Header("Ground Check")]
//    public LayerMask groundMask; // Маска об'єктів, від яких можна стрибати
//    public float groundCheckDistance = 0.6f;
//    public Vector3 groundCheckOffset = Vector3.zero;

//    [Header("Effects")]
//    public ParticleSystem jumpParticles;
//    public ParticleSystem deathParticles;
//    public ParticleSystem spiderTeleportParticles;
//    public ParticleSystem spiderLandParticles;

//    [Header("Audio SFX")]
//    public AudioClip jumpSfx;
//    public AudioClip deathSfx;
//    public AudioClip gravitySwitchSfx;

//    private Rigidbody rb;
//    private Collider myCollider;
//    private AudioSource audioSource;
//    private LevelManager levelManager;

//    private float defaultSpeed;
//    private int jumpsLeft;
//    private bool isGrounded;
//    private bool isDead = false;
//    private Vector3 originalScale;

//    private float jumpBufferCounter;
//    private float coyoteTimeCounter;
//    private bool jumpRequested = false;

//    void Awake()
//    {
//        rb = GetComponent<Rigidbody>();
//        myCollider = GetComponent<Collider>();
//        audioSource = GetComponent<AudioSource>();
//        levelManager = FindObjectOfType<LevelManager>();

//        spiderLine = GetComponent<LineRenderer>();
//        if (spiderLine != null) spiderLine.enabled = false;

//        rb.interpolation = RigidbodyInterpolation.Interpolate;
//        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
//        rb.useGravity = false;

//        if (audioSource != null)
//        {
//            float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);
//            audioSource.volume = sfxVol;
//        }

//        defaultSpeed = forwardSpeed;
//        originalScale = transform.localScale;

//        if (visualModel != null)
//            modelRenderers = visualModel.GetComponentsInChildren<Renderer>();

//        // ---> ІНІЦІАЛІЗАЦІЯ ШАРІВ
//        playerLayer = gameObject.layer;
//        safeLayer = LayerMask.NameToLayer(safeLayerName);
//        deadlyLayer = LayerMask.NameToLayer(deadlyLayerName);
//        wallLayer = LayerMask.NameToLayer(wallLayerName);

//        if (safeLayer == -1 || deadlyLayer == -1 || wallLayer == -1)
//        {
//            Debug.LogError("ПОМИЛКА: Перевір шари Ground, Obstacle, Wall в Unity!");
//        }

//        // ---> АВТОМАТИЧНЕ ВИПРАВЛЕННЯ ДЛЯ СТРИБКА <---
//        // Додаємо шар Wall до маски GroundMask, щоб скрипт "бачив" стіни як землю
//        if (wallLayer != -1)
//        {
//            groundMask |= (1 << wallLayer);
//        }
//    }

//    void Update()
//    {
//        if (isDead) return;

//        if (visualModel != null) visualModel.Rotate(Vector3.back * rotationSpeed * Time.deltaTime);

//        if (isGrounded) { coyoteTimeCounter = coyoteTime; jumpsLeft = maxJumps; }
//        else { coyoteTimeCounter -= Time.deltaTime; }

//        bool isHolding = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0) || (Input.touchCount > 0);
//        bool isPressedDown = Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) ||
//                             (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);

//        if (isGhostMode)
//        {
//            if (isHolding) SetPhasingState(true);
//            else SetPhasingState(false);
//        }
//        else
//        {
//            if (isPhasing) SetPhasingState(false);

//            if (isPressedDown)
//            {
//                if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
//                    jumpBufferCounter = jumpBufferTime;
//            }
//            else jumpBufferCounter -= Time.deltaTime;
//        }

//        if (!isGhostMode && jumpBufferCounter > 0f)
//        {
//            if (isSpiderMode && isGrounded) { PerformSpiderTeleport(); jumpBufferCounter = 0f; }
//            else if (isGravityMode && isGrounded) { FlipGravity(); jumpBufferCounter = 0f; }
//            else if (!isSpiderMode && !isGravityMode && (coyoteTimeCounter > 0f || jumpsLeft > 0))
//            {
//                jumpRequested = true;
//                if (coyoteTimeCounter <= 0f) jumpsLeft--;
//                jumpBufferCounter = 0f;
//            }
//        }
//    }

//    // ---> ФІЗИЧНИЙ ФАЗИНГ
//    void SetPhasingState(bool active)
//    {
//        if (isPhasing == active) return;
//        isPhasing = active;

//        // 1. Візуал
//        if (modelRenderers != null)
//        {
//            foreach (var r in modelRenderers)
//            {
//                foreach (var mat in r.materials)
//                {
//                    Color c = mat.color;
//                    c.a = active ? ghostAlpha : 1f;
//                    mat.color = c;
//                }
//            }
//        }

//        // 2. Фізика
//        // Ігноруємо Obstacle (проходимо крізь смерть)
//        if (deadlyLayer != -1) Physics.IgnoreLayerCollision(playerLayer, deadlyLayer, active);

//        // Ігноруємо Wall (пролітаємо наскрізь у режимі привида)
//        if (wallLayer != -1) Physics.IgnoreLayerCollision(playerLayer, wallLayer, active);
//    }

//    void FixedUpdate()
//    {
//        if (isDead) return;
//        Vector3 customGravity = Vector3.down * gravityForce * gravityScale;
//        rb.AddForce(customGravity, ForceMode.Acceleration);
//        Vector3 currentVel = rb.velocity;
//        currentVel.x = forwardSpeed;

//        if (jumpRequested)
//        {
//            currentVel.y = jumpForce * gravityScale;
//            if (jumpParticles) jumpParticles.Play();
//            if (audioSource && jumpSfx) audioSource.PlayOneShot(jumpSfx);
//            jumpRequested = false; coyoteTimeCounter = 0f;
//        }
//        rb.velocity = currentVel;
//        CheckFrontCollision(); GroundCheck();
//    }

//    void CheckFrontCollision()
//    {
//        if (isDead) return;

//        // У режимі фазингу ми ігноруємо стіни спереду
//        if (isGhostMode && isPhasing) return;

//        RaycastHit hit;
//        Vector3 origin = transform.position + (gravityScale > 0 ? Vector3.up : Vector3.down) * 0.5f;

//        if (Physics.Raycast(origin, Vector3.right, out hit, 0.6f))
//        {
//            int hitLayer = hit.collider.gameObject.layer;
//            string hitTag = hit.collider.tag;

//            // 1. Вмираємо від Obstacle (якщо не фазимо)
//            bool isObstacle = (deadlyLayer != -1 && hitLayer == deadlyLayer) || hitTag == "Obstacle";
//            if (isObstacle) Die();

//            // 2. Вмираємо від Wall (ніколи тут, бо Wall безпечний для лобового удару у звичайному режимі, він просто зупинить гравця)
//            // А якщо ми фазимо - ми сюди не дійдемо через return на початку.
//        }
//    }

//    void OnCollisionEnter(Collision collision)
//    {
//        if (isDead) return;
//        if (isGhostMode && isPhasing) return; // Ігноруємо колізії при фазингу

//        int hitLayer = collision.gameObject.layer;
//        string hitTag = collision.gameObject.tag;

//        // Вмираємо ТІЛЬКИ від Obstacle
//        bool isObstacle = (deadlyLayer != -1 && hitLayer == deadlyLayer) || hitTag == "Obstacle";

//        if (isObstacle)
//        {
//            foreach (ContactPoint contact in collision.contacts)
//            {
//                if (gravityScale > 0 && contact.normal.y > 0.7f) return;
//                if (gravityScale < 0 && contact.normal.y < -0.7f) return;
//            }
//            Die();
//        }
//        // Wall тут безпечний
//    }

//    void OnTriggerEnter(Collider other)
//    {
//        if (isDead) return;
//        if (isGhostMode && isPhasing) return;

//        int hitLayer = other.gameObject.layer;

//        // Вмираємо від Obstacle
//        bool isObstacle = (deadlyLayer != -1 && hitLayer == deadlyLayer) || other.CompareTag("Obstacle");
//        if (isObstacle) Die();
//    }

//    // ... (Методи Die, Animations, Respawn без змін) ...

//    void Die()
//    {
//        if (isDead) return; isDead = true;
//        if (myCollider != null) myCollider.enabled = false; rb.detectCollisions = false; rb.velocity = Vector3.zero; rb.angularVelocity = Vector3.zero; rb.isKinematic = true;
//        if (trail != null) trail.emitting = false;
//        if (deathParticles) Instantiate(deathParticles, transform.position, Quaternion.identity);
//        if (audioSource && deathSfx) audioSource.PlayOneShot(deathSfx);
//        StartCoroutine(DeathAnimation());
//    }

//    IEnumerator DeathAnimation()
//    {
//        float t = 0f; Vector3 startScale = visualModel != null ? visualModel.localScale : transform.localScale;
//        while (t < 0.3f) { t += Time.unscaledDeltaTime; Vector3 newScale = Vector3.Lerp(startScale, Vector3.zero, t / 0.3f); if (visualModel != null) visualModel.localScale = newScale; else transform.localScale = newScale; yield return null; }
//        if (visualModel != null) visualModel.localScale = Vector3.zero; else transform.localScale = Vector3.zero; if (levelManager) levelManager.PlayerDied(this);
//    }

//    public void RespawnAt(Transform spawnPoint)
//    {
//        if (spawnPoint == null) return; isDead = false; jumpRequested = false; jumpBufferCounter = 0f; coyoteTimeCounter = 0f; forwardSpeed = defaultSpeed;
//        SetMode("Cube"); gravityScale = 1f; rb.isKinematic = true; rb.detectCollisions = false; transform.position = spawnPoint.position; transform.rotation = spawnPoint.rotation;
//        if (visualModel != null) visualModel.localScale = Vector3.one; else transform.localScale = originalScale;
//        if (trail != null) { trail.Clear(); trail.emitting = true; }
//        if (spiderLine != null) spiderLine.enabled = false;
//        Physics.SyncTransforms(); if (myCollider != null) myCollider.enabled = true; rb.isKinematic = false; rb.detectCollisions = true; rb.velocity = Vector3.zero; rb.angularVelocity = Vector3.zero; jumpsLeft = maxJumps;
//    }

//    void PerformSpiderTeleport()
//    {
//        Vector3 searchDirection = gravityScale > 0 ? Vector3.up : Vector3.down;
//        RaycastHit hit;
//        if (Physics.Raycast(transform.position, searchDirection, out hit, Mathf.Infinity, groundMask))
//        {
//            if (audioSource && gravitySwitchSfx) audioSource.PlayOneShot(gravitySwitchSfx);
//            if (spiderTeleportParticles) Instantiate(spiderTeleportParticles, transform.position, Quaternion.identity);
//            Vector3 startPos = transform.position;
//            float playerHeightOffset = 0.5f;
//            Vector3 targetPosition = hit.point + (searchDirection * -1 * playerHeightOffset);
//            Vector3 finalPos = new Vector3(transform.position.x, targetPosition.y, transform.position.z);
//            rb.interpolation = RigidbodyInterpolation.None; transform.position = finalPos; Physics.SyncTransforms(); rb.interpolation = RigidbodyInterpolation.Interpolate;
//            if (trail != null) trail.Clear();
//            if (spiderLandParticles) Instantiate(spiderLandParticles, finalPos, Quaternion.LookRotation(hit.normal));
//            StartCoroutine(DrawSpiderBeam(startPos, finalPos));
//            FlipGravity(); rb.velocity = new Vector3(rb.velocity.x, 0, 0);
//        }
//    }

//    IEnumerator DrawSpiderBeam(Vector3 start, Vector3 end)
//    {
//        if (spiderLine != null) { spiderLine.enabled = true; spiderLine.SetPosition(0, start); spiderLine.SetPosition(1, end); yield return new WaitForSeconds(spiderBeamDuration); spiderLine.enabled = false; }
//    }

//    public void FlipGravity() { gravityScale *= -1; }

//    public void SetMode(string modeName)
//    {
//        isGravityMode = false; isSpiderMode = false; isGhostMode = false; SetPhasingState(false);
//        if (visualModel != null) visualModel.localScale = Vector3.one;
//        if (modeName == "Cube") gravityScale = 1f; else if (modeName == "Ball") isGravityMode = true; else if (modeName == "Spider") isSpiderMode = true; else if (modeName == "Ghost") isGhostMode = true;
//    }

//    public void SetSFXVolume(float volume) { if (audioSource != null) audioSource.volume = volume; }

//    void GroundCheck()
//    {
//        RaycastHit hit; Vector3 origin = transform.position + groundCheckOffset;
//        Vector3 checkDirection = gravityScale > 0 ? Vector3.down : Vector3.up;

//        // groundMask використовується тут. У Awake ми додали до нього Wall.
//        isGrounded = Physics.Raycast(origin, checkDirection, out hit, groundCheckDistance, groundMask);
//    }

//    public void Win() { rb.velocity = Vector3.zero; rb.isKinematic = true; if (myCollider != null) myCollider.enabled = false; enabled = false; }
//    public void ResetJumpsFromPad() { jumpsLeft = maxJumps; }
//}
//using System.Collections;
//using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(AudioSource))]
//[RequireComponent(typeof(LineRenderer))]
//public class PlayerController : MonoBehaviour
//{
//    [Header("Visuals")]
//    public Transform visualModel;
//    public float rotationSpeed = 360f;
//    public TrailRenderer trail;

//    [Header("Ghost Settings")]
//    public float ghostAlpha = 0.3f;
//    public string safeLayerName = "Ground";
//    public string deadlyLayerName = "Obstacle";
//    public string wallLayerName = "Wall";

//    private Renderer[] modelRenderers;
//    private bool isPhasing = false;

//    // ID шарів
//    private int playerLayer;
//    private int safeLayer;
//    private int deadlyLayer;
//    private int wallLayer;

//    [Header("Zipline Settings")]
//    public float ziplineBaseSpeed = 15f;
//    public float ziplineSnapSpeed = 20f; // Наскільки швидко примагнічувати до лінії
//    private bool isZiplineMode = false;
//    private Transform currentZipline;
//    private float currentZiplineSpeedMod = 1f;

//    [Header("Spider Settings")]
//    public float spiderBeamDuration = 0.1f;
//    private LineRenderer spiderLine;

//    [Header("Movement")]
//    public float forwardSpeed = 10f;
//    public float jumpForce = 12f;
//    public int maxJumps = 1;

//    public float jumpBufferTime = 0.15f;
//    public float coyoteTime = 0.1f;

//    [Header("Modes Settings")]
//    public float gravityForce = 30f;

//    private bool isGravityMode = false;
//    private bool isSpiderMode = false;
//    private bool isGhostMode = false;
//    private float gravityScale = 1f;

//    [Header("Ground Check")]
//    public LayerMask groundMask;
//    public float groundCheckDistance = 0.6f;
//    public Vector3 groundCheckOffset = Vector3.zero;

//    [Header("Effects")]
//    public ParticleSystem jumpParticles;
//    public ParticleSystem deathParticles;
//    public ParticleSystem spiderTeleportParticles;
//    public ParticleSystem spiderLandParticles;

//    [Header("Audio SFX")]
//    public AudioClip jumpSfx;
//    public AudioClip deathSfx;
//    public AudioClip gravitySwitchSfx;

//    private Rigidbody rb;
//    private Collider myCollider;
//    private AudioSource audioSource;
//    private LevelManager levelManager;

//    // ---> НОВЕ: Посилання на камеру
//    private CameraFollow cameraFollow;

//    private float defaultSpeed;
//    private int jumpsLeft;
//    private bool isGrounded;
//    private bool isDead = false;

//    private Vector3 originalScale;

//    private float jumpBufferCounter;
//    private float coyoteTimeCounter;
//    private bool jumpRequested = false;

//    void Awake()
//    {
//        rb = GetComponent<Rigidbody>();
//        myCollider = GetComponent<Collider>();
//        audioSource = GetComponent<AudioSource>();
//        levelManager = FindObjectOfType<LevelManager>();

//        // ---> НОВЕ: Знаходимо камеру
//        cameraFollow = FindObjectOfType<CameraFollow>();

//        spiderLine = GetComponent<LineRenderer>();
//        if (spiderLine != null) spiderLine.enabled = false;

//        rb.interpolation = RigidbodyInterpolation.Interpolate;
//        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
//        rb.useGravity = false;

//        if (audioSource != null)
//        {
//            float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);
//            audioSource.volume = sfxVol;
//        }

//        defaultSpeed = forwardSpeed;
//        originalScale = transform.localScale;

//        if (visualModel != null)
//            modelRenderers = visualModel.GetComponentsInChildren<Renderer>();

//        // Init Layers
//        playerLayer = gameObject.layer;
//        safeLayer = LayerMask.NameToLayer(safeLayerName);
//        deadlyLayer = LayerMask.NameToLayer(deadlyLayerName);
//        wallLayer = LayerMask.NameToLayer(wallLayerName);

//        if (wallLayer != -1) groundMask |= (1 << wallLayer);
//    }

//    void Update()
//    {
//        if (isDead) return;

//        // Обертання моделі
//        if (visualModel != null && !isZiplineMode)
//            visualModel.Rotate(Vector3.back * rotationSpeed * Time.deltaTime);

//        // Відновлення стрибків
//        if (isGrounded || isZiplineMode)
//        {
//            coyoteTimeCounter = coyoteTime;
//            jumpsLeft = maxJumps;
//        }
//        else { coyoteTimeCounter -= Time.deltaTime; }

//        bool isHolding = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0) || (Input.touchCount > 0);
//        bool isPressedDown = Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) ||
//                             (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);

//        // --- INPUT HANDLING ---
//        if (isGhostMode)
//        {
//            if (isHolding) SetPhasingState(true);
//            else SetPhasingState(false);
//        }
//        else
//        {
//            if (isPhasing) SetPhasingState(false);

//            if (isPressedDown)
//            {
//                if (UnityEngine.EventSystems.EventSystem.current == null || !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
//                    jumpBufferCounter = jumpBufferTime;
//            }
//            else jumpBufferCounter -= Time.deltaTime;
//        }

//        // --- ВИХІД З ЗІПЛАЙНУ ---
//        if (isZiplineMode && !isHolding)
//        {
//            ExitZipline(false);
//        }

//        // --- JUMP LOGIC ---
//        if (!isGhostMode && !isZiplineMode && jumpBufferCounter > 0f)
//        {
//            if (isSpiderMode && isGrounded) { PerformSpiderTeleport(); jumpBufferCounter = 0f; }
//            else if (isGravityMode && isGrounded) { FlipGravity(); jumpBufferCounter = 0f; }
//            else if (!isSpiderMode && !isGravityMode && (coyoteTimeCounter > 0f || jumpsLeft > 0))
//            {
//                jumpRequested = true;
//                if (coyoteTimeCounter <= 0f) jumpsLeft--;
//                jumpBufferCounter = 0f;
//            }
//        }
//    }

//    void FixedUpdate()
//    {
//        if (isDead) return;

//        // ============================================
//        // ---> ЛОГІКА ЗІПЛАЙНУ <---
//        // ============================================
//        if (isZiplineMode && currentZipline != null)
//        {
//            Vector3 zipDirection = currentZipline.right;
//            Vector3 targetVelocity = zipDirection * (ziplineBaseSpeed * currentZiplineSpeedMod);
//            rb.velocity = targetVelocity;

//            Vector3 zipOrigin = currentZipline.position;
//            Vector3 playerDelta = transform.position - zipOrigin;
//            Vector3 projectedDelta = Vector3.Project(playerDelta, zipDirection);
//            Vector3 idealPosition = zipOrigin + projectedDelta;
//            Vector3 smoothedPos = Vector3.Lerp(transform.position, idealPosition, Time.fixedDeltaTime * ziplineSnapSpeed);

//            rb.MovePosition(smoothedPos);

//            if (visualModel != null)
//            {
//                Quaternion targetRot = Quaternion.LookRotation(zipDirection);
//                visualModel.rotation = Quaternion.Lerp(visualModel.rotation, targetRot, Time.fixedDeltaTime * 10f);
//            }

//            return;
//        }
//        // ============================================

//        // ---> STANDARD PHYSICS
//        Vector3 customGravity = Vector3.down * gravityForce * gravityScale;
//        rb.AddForce(customGravity, ForceMode.Acceleration);

//        Vector3 currentVel = rb.velocity;
//        currentVel.x = forwardSpeed;

//        if (jumpRequested)
//        {
//            currentVel.y = jumpForce * gravityScale;
//            if (jumpParticles) jumpParticles.Play();
//            if (audioSource && jumpSfx) audioSource.PlayOneShot(jumpSfx);
//            jumpRequested = false;
//            coyoteTimeCounter = 0f;
//        }
//        rb.velocity = currentVel;

//        CheckFrontCollision();
//        GroundCheck();
//    }

//    // ---> ЛОГІКА ТРИГЕРІВ <---

//    void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Zipline"))
//        {
//            bool isHolding = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0) || (Input.touchCount > 0);
//            if (isHolding)
//            {
//                EnterZipline(other.transform);
//            }
//        }
//        else
//        {
//            HandleTriggerDeath(other);
//        }
//    }

//    void OnTriggerStay(Collider other)
//    {
//        if (other.CompareTag("Zipline") && !isZiplineMode)
//        {
//            bool isHolding = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0) || (Input.touchCount > 0);
//            if (isHolding) EnterZipline(other.transform);
//        }
//    }

//    void OnTriggerExit(Collider other)
//    {
//        if (isZiplineMode && other.CompareTag("Zipline") && other.transform == currentZipline)
//        {
//            ExitZipline(false);
//        }
//    }

//    // ---> МЕТОДИ КЕРУВАННЯ ZIPLINE

//    void EnterZipline(Transform zipTransform)
//    {
//        isZiplineMode = true;
//        currentZipline = zipTransform;
//        jumpBufferCounter = 0f;

//        ZiplineObject zipObj = zipTransform.GetComponent<ZiplineObject>();
//        currentZiplineSpeedMod = (zipObj != null) ? zipObj.speedMultiplier : 1f;
//        rb.velocity = zipTransform.right * (ziplineBaseSpeed * currentZiplineSpeedMod);

//        // ---> Я ПРИБРАВ ЦЕЙ РЯДОК <---
//        // Тепер, якщо ви були перевернуті (павук), камера залишиться перевернутою і на зіплайні.
//        // if (cameraFollow) cameraFollow.SetGravityFlipped(false); 
//    }

//    void ExitZipline(bool jumpOut)
//    {
//        isZiplineMode = false;
//        currentZipline = null;

//        if (visualModel != null) visualModel.rotation = Quaternion.identity;

//        if (!jumpOut)
//        {
//            Vector3 exitVel = rb.velocity;
//            exitVel.x = forwardSpeed;
//            rb.velocity = exitVel;
//        }
//    }

//    // ... (HandleTriggerDeath, CheckFrontCollision, OnCollisionEnter, SetPhasingState, Die, DeathAnimation - БЕЗ ЗМІН) ...
//    void HandleTriggerDeath(Collider other) { if (isDead) return; if (isGhostMode && isPhasing) { if (other.CompareTag("Obstacle") || other.gameObject.layer == deadlyLayer || other.gameObject.layer == wallLayer) return; } int hitLayer = other.gameObject.layer; bool isObstacle = (deadlyLayer != -1 && hitLayer == deadlyLayer) || other.CompareTag("Obstacle"); if (isObstacle) Die(); }
//    void CheckFrontCollision() { if (isDead || (isGhostMode && isPhasing) || isZiplineMode) return; RaycastHit hit; Vector3 origin = transform.position + (gravityScale > 0 ? Vector3.up : Vector3.down) * 0.5f; if (Physics.Raycast(origin, Vector3.right, out hit, 0.6f)) { int hitLayer = hit.collider.gameObject.layer; bool isObstacle = (deadlyLayer != -1 && hitLayer == deadlyLayer) || hit.collider.tag == "Obstacle"; if (isObstacle) Die(); } }
//    void OnCollisionEnter(Collision collision) { if (isDead || (isGhostMode && isPhasing) || isZiplineMode) return; int hitLayer = collision.gameObject.layer; bool isObstacle = (deadlyLayer != -1 && hitLayer == deadlyLayer) || collision.gameObject.CompareTag("Obstacle"); if (isObstacle) { foreach (ContactPoint contact in collision.contacts) { if (gravityScale > 0 && contact.normal.y > 0.7f) return; if (gravityScale < 0 && contact.normal.y < -0.7f) return; } Die(); } }
//    void SetPhasingState(bool active) { if (isPhasing == active) return; isPhasing = active; if (modelRenderers != null) foreach (var r in modelRenderers) foreach (var mat in r.materials) { Color c = mat.color; c.a = active ? ghostAlpha : 1f; mat.color = c; } if (deadlyLayer != -1) Physics.IgnoreLayerCollision(playerLayer, deadlyLayer, active); if (wallLayer != -1) Physics.IgnoreLayerCollision(playerLayer, wallLayer, active); }
//    public void Die() { if (isDead) return; isDead = true; isZiplineMode = false; if (myCollider != null) myCollider.enabled = false; rb.detectCollisions = false; rb.velocity = Vector3.zero; rb.angularVelocity = Vector3.zero; rb.isKinematic = true; if (trail != null) trail.emitting = false; if (deathParticles) Instantiate(deathParticles, transform.position, Quaternion.identity); if (audioSource && deathSfx) audioSource.PlayOneShot(deathSfx); StartCoroutine(DeathAnimation()); }
//    IEnumerator DeathAnimation() { float t = 0f; Vector3 startScale = visualModel != null ? visualModel.localScale : transform.localScale; while (t < 0.3f) { t += Time.unscaledDeltaTime; Vector3 newScale = Vector3.Lerp(startScale, Vector3.zero, t / 0.3f); if (visualModel != null) visualModel.localScale = newScale; else transform.localScale = newScale; yield return null; } if (visualModel != null) visualModel.localScale = Vector3.zero; else transform.localScale = Vector3.zero; if (levelManager) levelManager.PlayerDied(this); }

//    public void RespawnAt(Transform spawnPoint)
//    {
//        //if (spawnPoint == null) return; isDead = false; jumpRequested = false; jumpBufferCounter = 0f; coyoteTimeCounter = 0f; forwardSpeed = defaultSpeed;
//        //SetMode("Cube"); gravityScale = 1f; rb.isKinematic = true; rb.detectCollisions = false; transform.position = spawnPoint.position; transform.rotation = spawnPoint.rotation;
//        //if (visualModel != null) visualModel.localScale = Vector3.one; else transform.localScale = originalScale;
//        //if (trail != null) { trail.Clear(); trail.emitting = true; }
//        //if (spiderLine != null) spiderLine.enabled = false;
//        //Physics.SyncTransforms(); if (myCollider != null) myCollider.enabled = true; rb.isKinematic = false; rb.detectCollisions = true; rb.velocity = Vector3.zero; rb.angularVelocity = Vector3.zero; jumpsLeft = maxJumps; isZiplineMode = false;

//        //// ---> НОВЕ: Скидаємо камеру при респавні
//        //if (cameraFollow) cameraFollow.SetGravityFlipped(false);
//        if (spawnPoint == null) return;


//        // 1. Скидаємо базові змінні
//        isDead = false;
//        jumpRequested = false;
//        jumpBufferCounter = 0f;
//        coyoteTimeCounter = 0f;
//        forwardSpeed = defaultSpeed;

//        // 2. Вимикаємо фізику на момент телепортації
//        if (myCollider != null) myCollider.enabled = false;
//        rb.isKinematic = true;
//        rb.detectCollisions = false;

//        // 3. Телепортація на точку спавну
//        transform.position = spawnPoint.position;
//        transform.rotation = spawnPoint.rotation;

//        // Скидання візуалу
//        if (visualModel != null)
//        {
//            visualModel.localScale = Vector3.one;
//            visualModel.localRotation = Quaternion.identity;
//        }
//        else
//        {
//            transform.localScale = originalScale;
//        }

//        if (trail != null) { trail.Clear(); trail.emitting = true; }
//        if (spiderLine != null) spiderLine.enabled = false;

//        // Оновлюємо фізичний рушій
//        Physics.SyncTransforms();

//        // =========================================================
//        // ---> ГОЛОВНЕ ВИПРАВЛЕННЯ ТУТ <---
//        // =========================================================

//        // 1. Знаходимо і ВИМИКАЄМО LaneRunner3D
//        // Якщо цього не зробити, він продовжить керувати персонажем
//        LaneRunner3D runnerScript = GetComponent<LaneRunner3D>();
//        if (runnerScript != null)
//        {
//            runnerScript.enabled = false;
//        }

//        // 2. Вмикаємо цей скрипт (PlayerController)
//        this.enabled = true;

//        // 3. Скидаємо камеру (вимикаємо 3D режим і перевороти)
//        if (cameraFollow)
//        {
//            cameraFollow.SetGravityFlipped(false);
//            cameraFollow.Set3DView(false); // <--- Додайте цей метод у CameraFollow, якщо його немає, або просто ігноруйте, якщо камера сама перемикається
//        }

//        // 4. Скидаємо блокування осей фізики (щоб повернути стандартний рух)
//        rb.constraints = RigidbodyConstraints.FreezeRotation;

//        // 5. Вмикаємо фізику назад
//        if (myCollider != null) myCollider.enabled = true;
//        rb.isKinematic = false;
//        rb.detectCollisions = true;

//        // 6. Обнуляємо всю швидкість
//        rb.velocity = Vector3.zero;
//        rb.angularVelocity = Vector3.zero;

//        // 7. Скидаємо режими
//        SetMode("Cube");
//        gravityScale = 1f;
//        jumpsLeft = maxJumps;
//        isZiplineMode = false;

//        // 8. Страховка: Вирівнювання по Z (щоб точно був по центру)
//        Vector3 flatPos = transform.position;
//        flatPos.z = 0f;
//        transform.position = flatPos;
//    }

//    void PerformSpiderTeleport()
//    {
//        Vector3 searchDirection = gravityScale > 0 ? Vector3.up : Vector3.down; RaycastHit hit;
//        if (Physics.Raycast(transform.position, searchDirection, out hit, Mathf.Infinity, groundMask))
//        {
//            if (audioSource && gravitySwitchSfx) audioSource.PlayOneShot(gravitySwitchSfx);
//            if (spiderTeleportParticles) Instantiate(spiderTeleportParticles, transform.position, Quaternion.identity);
//            Vector3 startPos = transform.position; float playerHeightOffset = 0.5f;
//            Vector3 targetPosition = hit.point + (searchDirection * -1 * playerHeightOffset);
//            Vector3 finalPos = new Vector3(transform.position.x, targetPosition.y, transform.position.z);
//            rb.interpolation = RigidbodyInterpolation.None; transform.position = finalPos; Physics.SyncTransforms(); rb.interpolation = RigidbodyInterpolation.Interpolate;
//            if (trail != null) trail.Clear(); if (spiderLandParticles) Instantiate(spiderLandParticles, finalPos, Quaternion.LookRotation(hit.normal));
//            StartCoroutine(DrawSpiderBeam(startPos, finalPos));

//            FlipGravity(); // Цей метод тепер викличе камеру

//            rb.velocity = new Vector3(rb.velocity.x, 0, 0);
//        }
//    }

//    IEnumerator DrawSpiderBeam(Vector3 start, Vector3 end) { if (spiderLine != null) { spiderLine.enabled = true; spiderLine.SetPosition(0, start); spiderLine.SetPosition(1, end); yield return new WaitForSeconds(spiderBeamDuration); spiderLine.enabled = false; } }

//    // ---> НОВЕ: Оновлений метод FlipGravity
//    public void FlipGravity()
//    {
//        gravityScale *= -1;

//        // Додано виклик камери:
//        if (cameraFollow != null)
//            cameraFollow.SetGravityFlipped(gravityScale < 0);
//    }

//    public void SetMode(string modeName)
//    {
//        isGravityMode = false; isSpiderMode = false; isGhostMode = false; SetPhasingState(false);
//        if (visualModel != null) visualModel.localScale = Vector3.one;
//        if (modeName == "Cube")
//        {
//            gravityScale = 1f;
//            // ---> НОВЕ: Скидаємо камеру, якщо повернулися в режим Куба
//            if (cameraFollow) cameraFollow.SetGravityFlipped(false);
//        }
//        else if (modeName == "Ball") isGravityMode = true; else if (modeName == "Spider") isSpiderMode = true; else if (modeName == "Ghost") isGhostMode = true;
//    }

//    public void SetSFXVolume(float volume) { if (audioSource != null) audioSource.volume = volume; }
//    void GroundCheck() { RaycastHit hit; Vector3 origin = transform.position + groundCheckOffset; Vector3 checkDirection = gravityScale > 0 ? Vector3.down : Vector3.up; isGrounded = Physics.Raycast(origin, checkDirection, out hit, groundCheckDistance, groundMask); }
//    public void Win() { rb.velocity = Vector3.zero; rb.isKinematic = true; if (myCollider != null) myCollider.enabled = false; enabled = false; }
//    public void ResetJumpsFromPad() { jumpsLeft = maxJumps; }
//}
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(LineRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("Visuals")]
    public Transform visualModel;
    public float rotationSpeed = 360f;
    public TrailRenderer trail;

    [Header("Ghost Settings")]
    public float ghostAlpha = 0.3f;
    public string safeLayerName = "Ground";
    public string deadlyLayerName = "Obstacle";
    public string wallLayerName = "Wall";

    private Renderer[] modelRenderers;
    private bool isPhasing = false;

    // ID шарів
    private int playerLayer;
    private int safeLayer;
    private int deadlyLayer;
    private int wallLayer;

    [Header("Zipline Settings")]
    public float ziplineBaseSpeed = 15f;
    public float ziplineSnapSpeed = 20f; // Наскільки швидко примагнічувати до лінії
    private bool isZiplineMode = false;
    private Transform currentZipline;
    private float currentZiplineSpeedMod = 1f;

    [Header("Spider Settings")]
    public float spiderBeamDuration = 0.1f;
    private LineRenderer spiderLine;

    [Header("Movement")]
    public float forwardSpeed = 10f;
    public float jumpForce = 12f;
    public int maxJumps = 1;

    public float jumpBufferTime = 0.15f;
    public float coyoteTime = 0.1f;

    [Header("Modes Settings")]
    public float gravityForce = 30f;

    private bool isGravityMode = false;
    private bool isSpiderMode = false;
    private bool isGhostMode = false;
    private float gravityScale = 1f;

    [Header("Ground Check")]
    public LayerMask groundMask;
    public float groundCheckDistance = 0.6f;
    public Vector3 groundCheckOffset = Vector3.zero;

    [Header("Effects")]
    public ParticleSystem jumpParticles;
    public ParticleSystem deathParticles;
    public ParticleSystem spiderTeleportParticles;
    public ParticleSystem spiderLandParticles;

    [Header("Audio SFX")]
    public AudioClip jumpSfx;
    public AudioClip deathSfx;
    public AudioClip gravitySwitchSfx;

    private Rigidbody rb;
    private Collider myCollider;
    private AudioSource audioSource;
    private LevelManager levelManager;

    // ---> НОВЕ: Посилання на камеру
    private CameraFollow cameraFollow;

    private float defaultSpeed;
    private int jumpsLeft;
    private bool isGrounded;
    private bool isDead = false;

    private Vector3 originalScale;

    private float jumpBufferCounter;
    private float coyoteTimeCounter;
    private bool jumpRequested = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();
        levelManager = FindObjectOfType<LevelManager>();

        // ---> НОВЕ: Знаходимо камеру
        cameraFollow = FindObjectOfType<CameraFollow>();

        spiderLine = GetComponent<LineRenderer>();
        if (spiderLine != null) spiderLine.enabled = false;

        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.useGravity = false;

        if (audioSource != null)
        {
            float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);
            audioSource.volume = sfxVol;
        }

        defaultSpeed = forwardSpeed;
        originalScale = transform.localScale;

        if (visualModel != null)
            modelRenderers = visualModel.GetComponentsInChildren<Renderer>();

        // Init Layers
        playerLayer = gameObject.layer;
        safeLayer = LayerMask.NameToLayer(safeLayerName);
        deadlyLayer = LayerMask.NameToLayer(deadlyLayerName);
        wallLayer = LayerMask.NameToLayer(wallLayerName);

        if (wallLayer != -1) groundMask |= (1 << wallLayer);
    }

    void Update()
    {
        if (isDead) return;

        // ---> ЗАКОМЕНТОВАНО: Обертання моделі (щоб не котилася) <---
        // Обертання моделі
        // if (visualModel != null && !isZiplineMode)
        //    visualModel.Rotate(Vector3.back * rotationSpeed * Time.deltaTime);

        // Відновлення стрибків
        if (isGrounded || isZiplineMode)
        {
            coyoteTimeCounter = coyoteTime;
            jumpsLeft = maxJumps;
        }
        else { coyoteTimeCounter -= Time.deltaTime; }

        bool isHolding = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0) || (Input.touchCount > 0);
        bool isPressedDown = Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) ||
                             (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);

        // --- INPUT HANDLING ---
        if (isGhostMode)
        {
            if (isHolding) SetPhasingState(true);
            else SetPhasingState(false);
        }
        else
        {
            if (isPhasing) SetPhasingState(false);

            if (isPressedDown)
            {
                if (UnityEngine.EventSystems.EventSystem.current == null || !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                    jumpBufferCounter = jumpBufferTime;
            }
            else jumpBufferCounter -= Time.deltaTime;
        }

        // --- ВИХІД З ЗІПЛАЙНУ ---
        if (isZiplineMode && !isHolding)
        {
            ExitZipline(false);
        }

        // --- JUMP LOGIC ---
        if (!isGhostMode && !isZiplineMode && jumpBufferCounter > 0f)
        {
            if (isSpiderMode && isGrounded) { PerformSpiderTeleport(); jumpBufferCounter = 0f; }
            else if (isGravityMode && isGrounded) { FlipGravity(); jumpBufferCounter = 0f; }
            else if (!isSpiderMode && !isGravityMode && (coyoteTimeCounter > 0f || jumpsLeft > 0))
            {
                jumpRequested = true;
                if (coyoteTimeCounter <= 0f) jumpsLeft--;
                jumpBufferCounter = 0f;
            }
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;

        // ============================================
        // ---> ЛОГІКА ЗІПЛАЙНУ <---
        // ============================================
        if (isZiplineMode && currentZipline != null)
        {
            Vector3 zipDirection = currentZipline.right;
            Vector3 targetVelocity = zipDirection * (ziplineBaseSpeed * currentZiplineSpeedMod);
            rb.velocity = targetVelocity;

            Vector3 zipOrigin = currentZipline.position;
            Vector3 playerDelta = transform.position - zipOrigin;
            Vector3 projectedDelta = Vector3.Project(playerDelta, zipDirection);
            Vector3 idealPosition = zipOrigin + projectedDelta;
            Vector3 smoothedPos = Vector3.Lerp(transform.position, idealPosition, Time.fixedDeltaTime * ziplineSnapSpeed);

            rb.MovePosition(smoothedPos);

            if (visualModel != null)
            {
                Quaternion targetRot = Quaternion.LookRotation(zipDirection);
                visualModel.rotation = Quaternion.Lerp(visualModel.rotation, targetRot, Time.fixedDeltaTime * 10f);
            }

            return;
        }
        // ============================================

        // ---> STANDARD PHYSICS
        Vector3 customGravity = Vector3.down * gravityForce * gravityScale;
        rb.AddForce(customGravity, ForceMode.Acceleration);

        Vector3 currentVel = rb.velocity;
        currentVel.x = forwardSpeed;

        if (jumpRequested)
        {
            currentVel.y = jumpForce * gravityScale;
            if (jumpParticles) jumpParticles.Play();
            if (audioSource && jumpSfx) audioSource.PlayOneShot(jumpSfx);
            jumpRequested = false;
            coyoteTimeCounter = 0f;
        }
        rb.velocity = currentVel;

        CheckFrontCollision();
        GroundCheck();
    }

    // ---> ЛОГІКА ТРИГЕРІВ <---

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Zipline"))
        {
            bool isHolding = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0) || (Input.touchCount > 0);
            if (isHolding)
            {
                EnterZipline(other.transform);
            }
        }
        else
        {
            HandleTriggerDeath(other);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Zipline") && !isZiplineMode)
        {
            bool isHolding = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0) || (Input.touchCount > 0);
            if (isHolding) EnterZipline(other.transform);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (isZiplineMode && other.CompareTag("Zipline") && other.transform == currentZipline)
        {
            ExitZipline(false);
        }
    }

    // ---> МЕТОДИ КЕРУВАННЯ ZIPLINE

    void EnterZipline(Transform zipTransform)
    {
        isZiplineMode = true;
        currentZipline = zipTransform;
        jumpBufferCounter = 0f;

        ZiplineObject zipObj = zipTransform.GetComponent<ZiplineObject>();
        currentZiplineSpeedMod = (zipObj != null) ? zipObj.speedMultiplier : 1f;
        rb.velocity = zipTransform.right * (ziplineBaseSpeed * currentZiplineSpeedMod);

        // ---> Я ПРИБРАВ ЦЕЙ РЯДОК <---
        // Тепер, якщо ви були перевернуті (павук), камера залишиться перевернутою і на зіплайні.
        // if (cameraFollow) cameraFollow.SetGravityFlipped(false); 
    }

    void ExitZipline(bool jumpOut)
    {
        isZiplineMode = false;
        currentZipline = null;

        if (visualModel != null) visualModel.rotation = Quaternion.identity;

        if (!jumpOut)
        {
            Vector3 exitVel = rb.velocity;
            exitVel.x = forwardSpeed;
            rb.velocity = exitVel;
        }
    }

    void HandleTriggerDeath(Collider other)
    {
        if (isDead) return;
        if (isGhostMode && isPhasing)
        {
            if (other.CompareTag("Obstacle") || other.gameObject.layer == deadlyLayer || other.gameObject.layer == wallLayer) return;
        }
        int hitLayer = other.gameObject.layer;
        bool isObstacle = (deadlyLayer != -1 && hitLayer == deadlyLayer) || other.CompareTag("Obstacle");
        if (isObstacle) Die();
    }

    void CheckFrontCollision()
    {
        if (isDead || (isGhostMode && isPhasing) || isZiplineMode) return;
        RaycastHit hit;
        Vector3 origin = transform.position + (gravityScale > 0 ? Vector3.up : Vector3.down) * 0.5f;
        if (Physics.Raycast(origin, Vector3.right, out hit, 0.6f))
        {
            int hitLayer = hit.collider.gameObject.layer;
            bool isObstacle = (deadlyLayer != -1 && hitLayer == deadlyLayer) || hit.collider.tag == "Obstacle";
            if (isObstacle) Die();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isDead || (isGhostMode && isPhasing) || isZiplineMode) return;
        int hitLayer = collision.gameObject.layer;
        bool isObstacle = (deadlyLayer != -1 && hitLayer == deadlyLayer) || collision.gameObject.CompareTag("Obstacle");
        if (isObstacle)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (gravityScale > 0 && contact.normal.y > 0.7f) return;
                if (gravityScale < 0 && contact.normal.y < -0.7f) return;
            }
            Die();
        }
    }

    void SetPhasingState(bool active)
    {
        if (isPhasing == active) return;
        isPhasing = active;
        if (modelRenderers != null)
            foreach (var r in modelRenderers)
                foreach (var mat in r.materials)
                {
                    Color c = mat.color;
                    c.a = active ? ghostAlpha : 1f;
                    mat.color = c;
                }
        if (deadlyLayer != -1) Physics.IgnoreLayerCollision(playerLayer, deadlyLayer, active);
        if (wallLayer != -1) Physics.IgnoreLayerCollision(playerLayer, wallLayer, active);
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        isZiplineMode = false;
        if (myCollider != null) myCollider.enabled = false;
        rb.detectCollisions = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        if (trail != null) trail.emitting = false;
        if (deathParticles) Instantiate(deathParticles, transform.position, Quaternion.identity);
        if (audioSource && deathSfx) audioSource.PlayOneShot(deathSfx);
        StartCoroutine(DeathAnimation());
    }

    IEnumerator DeathAnimation()
    {
        float t = 0f;
        Vector3 startScale = visualModel != null ? visualModel.localScale : transform.localScale;
        while (t < 0.3f)
        {
            t += Time.unscaledDeltaTime;
            Vector3 newScale = Vector3.Lerp(startScale, Vector3.zero, t / 0.3f);
            if (visualModel != null) visualModel.localScale = newScale; else transform.localScale = newScale;
            yield return null;
        }
        if (visualModel != null) visualModel.localScale = Vector3.zero; else transform.localScale = Vector3.zero;
        if (levelManager) levelManager.PlayerDied(this);
    }

    public void RespawnAt(Transform spawnPoint)
    {
        //if (spawnPoint == null) return; isDead = false; jumpRequested = false; jumpBufferCounter = 0f; coyoteTimeCounter = 0f; forwardSpeed = defaultSpeed;
        //SetMode("Cube"); gravityScale = 1f; rb.isKinematic = true; rb.detectCollisions = false; transform.position = spawnPoint.position; transform.rotation = spawnPoint.rotation;
        //if (visualModel != null) visualModel.localScale = Vector3.one; else transform.localScale = originalScale;
        //if (trail != null) { trail.Clear(); trail.emitting = true; }
        //if (spiderLine != null) spiderLine.enabled = false;
        //Physics.SyncTransforms(); if (myCollider != null) myCollider.enabled = true; rb.isKinematic = false; rb.detectCollisions = true; rb.velocity = Vector3.zero; rb.angularVelocity = Vector3.zero; jumpsLeft = maxJumps; isZiplineMode = false;

        //// ---> НОВЕ: Скидаємо камеру при респавні
        //if (cameraFollow) cameraFollow.SetGravityFlipped(false);
        if (spawnPoint == null) return;

        // 1. Скидаємо базові змінні
        isDead = false;
        jumpRequested = false;
        jumpBufferCounter = 0f;
        coyoteTimeCounter = 0f;
        forwardSpeed = defaultSpeed;

        // 2. Вимикаємо фізику на момент телепортації
        if (myCollider != null) myCollider.enabled = false;
        rb.isKinematic = true;
        rb.detectCollisions = false;

        // 3. Телепортація на точку спавну
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        // Скидання візуалу
        if (visualModel != null)
        {
            visualModel.localScale = Vector3.one;
            visualModel.localRotation = Quaternion.identity;
        }
        else
        {
            transform.localScale = originalScale;
        }

        if (trail != null) { trail.Clear(); trail.emitting = true; }
        if (spiderLine != null) spiderLine.enabled = false;

        // Оновлюємо фізичний рушій
        Physics.SyncTransforms();

        // =========================================================
        // ---> ГОЛОВНЕ ВИПРАВЛЕННЯ ТУТ <---
        // =========================================================

        // 1. Знаходимо і ВИМИКАЄМО LaneRunner3D
        // Якщо цього не зробити, він продовжить керувати персонажем
        LaneRunner3D runnerScript = GetComponent<LaneRunner3D>();
        if (runnerScript != null)
        {
            runnerScript.enabled = false;
        }

        // 2. Вмикаємо цей скрипт (PlayerController)
        this.enabled = true;

        // 3. Скидаємо камеру (вимикаємо 3D режим і перевороти)
        if (cameraFollow)
        {
            cameraFollow.SetGravityFlipped(false);
            cameraFollow.Set3DView(false); // <--- Додайте цей метод у CameraFollow, якщо його немає, або просто ігноруйте, якщо камера сама перемикається
        }

        // 4. Скидаємо блокування осей фізики (щоб повернути стандартний рух)
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // 5. Вмикаємо фізику назад
        if (myCollider != null) myCollider.enabled = true;
        rb.isKinematic = false;
        rb.detectCollisions = true;

        // 6. Обнуляємо всю швидкість
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // 7. Скидаємо режими
        SetMode("Cube");
        gravityScale = 1f;
        jumpsLeft = maxJumps;
        isZiplineMode = false;

        // 8. Страховка: Вирівнювання по Z (щоб точно був по центру)
        Vector3 flatPos = transform.position;
        flatPos.z = 0f;
        transform.position = flatPos;
    }

    void PerformSpiderTeleport()
    {
        Vector3 searchDirection = gravityScale > 0 ? Vector3.up : Vector3.down; RaycastHit hit;
        if (Physics.Raycast(transform.position, searchDirection, out hit, Mathf.Infinity, groundMask))
        {
            if (audioSource && gravitySwitchSfx) audioSource.PlayOneShot(gravitySwitchSfx);
            if (spiderTeleportParticles) Instantiate(spiderTeleportParticles, transform.position, Quaternion.identity);
            Vector3 startPos = transform.position; float playerHeightOffset = 0.5f;
            Vector3 targetPosition = hit.point + (searchDirection * -1 * playerHeightOffset);
            Vector3 finalPos = new Vector3(transform.position.x, targetPosition.y, transform.position.z);
            rb.interpolation = RigidbodyInterpolation.None; transform.position = finalPos; Physics.SyncTransforms(); rb.interpolation = RigidbodyInterpolation.Interpolate;
            if (trail != null) trail.Clear(); if (spiderLandParticles) Instantiate(spiderLandParticles, finalPos, Quaternion.LookRotation(hit.normal));
            StartCoroutine(DrawSpiderBeam(startPos, finalPos));

            FlipGravity(); // Цей метод тепер викличе камеру

            rb.velocity = new Vector3(rb.velocity.x, 0, 0);
        }
    }

    IEnumerator DrawSpiderBeam(Vector3 start, Vector3 end) { if (spiderLine != null) { spiderLine.enabled = true; spiderLine.SetPosition(0, start); spiderLine.SetPosition(1, end); yield return new WaitForSeconds(spiderBeamDuration); spiderLine.enabled = false; } }

    // ---> НОВЕ: Оновлений метод FlipGravity
    public void FlipGravity()
    {
        gravityScale *= -1;

        // Додано виклик камери:
        if (cameraFollow != null)
            cameraFollow.SetGravityFlipped(gravityScale < 0);
    }

    public void SetMode(string modeName)
    {
        isGravityMode = false; isSpiderMode = false; isGhostMode = false; SetPhasingState(false);
        if (visualModel != null) visualModel.localScale = Vector3.one;
        if (modeName == "Cube")
        {
            gravityScale = 1f;
            // ---> НОВЕ: Скидаємо камеру, якщо повернулися в режим Куба
            if (cameraFollow) cameraFollow.SetGravityFlipped(false);
        }
        else if (modeName == "Ball") isGravityMode = true; else if (modeName == "Spider") isSpiderMode = true; else if (modeName == "Ghost") isGhostMode = true;
    }

    public void SetSFXVolume(float volume) { if (audioSource != null) audioSource.volume = volume; }
    void GroundCheck() { RaycastHit hit; Vector3 origin = transform.position + groundCheckOffset; Vector3 checkDirection = gravityScale > 0 ? Vector3.down : Vector3.up; isGrounded = Physics.Raycast(origin, checkDirection, out hit, groundCheckDistance, groundMask); }
    public void Win() { rb.velocity = Vector3.zero; rb.isKinematic = true; if (myCollider != null) myCollider.enabled = false; enabled = false; }
    public void ResetJumpsFromPad() { jumpsLeft = maxJumps; }
}