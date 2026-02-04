//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class StarCoin : MonoBehaviour
//{
//    [Header("Налаштування")]
//    [Tooltip("ID монети: 0 - перша, 1 - друга, 2 - третя")]
//    [Range(0, 2)]
//    public int coinID = 0;

//    [Header("Ефекти")]
//    public GameObject visualModel; // Сама моделька (щоб ховати її)
//    public ParticleSystem pickupEffect;
//    public AudioClip pickupSound;

//    private bool isCollected = false;

//    void Start()
//    {
//        // Перевіряємо в базі при старті: якщо вже зібрана - ховаємо об'єкт
//        if (DatabaseManager.Instance != null)
//        {
//            string currentLevel = SceneManager.GetActiveScene().name;
//            if (DatabaseManager.Instance.IsStarCoinCollected(currentLevel, coinID))
//            {
//                gameObject.SetActive(false); // Вимикаємо монету, бо ми її вже маємо
//            }
//        }
//    }

//    void OnTriggerEnter(Collider other)
//    {
//        if (isCollected) return;

//        if (other.CompareTag("Player"))
//        {
//            PickUp();
//        }
//    }

//    void PickUp()
//    {
//        isCollected = true;

//        // 1. Повідомляємо LevelManager, що ми підібрали монету В ЦЬОМУ ЗАБІГУ
//        // (LevelManager ми оновимо в наступному кроці)
//        LevelManager.Instance.CollectStarCoinTemp(coinID);

//        // 2. Ефекти
//        if (pickupEffect) Instantiate(pickupEffect, transform.position, Quaternion.identity);
//        if (pickupSound) AudioSource.PlayClipAtPoint(pickupSound, transform.position);

//        // 3. Ховаємо візуал, але не знищуємо об'єкт миттєво (щоб скрипт допрацював, якщо треба)
//        if (visualModel) visualModel.SetActive(false);
//        GetComponent<Collider>().enabled = false;
//    }
//}
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class StarCoin : MonoBehaviour
//{
//    [Header("Налаштування")]
//    [Tooltip("0, 1 або 2")]
//    public int coinID;

//    [Header("Компоненти")]
//    public GameObject visualModel; // Сюди перетягни 3D модельку алмазу (дочірній об'єкт)
//    public Collider myCollider;    // Сюди перетягни BoxCollider/SphereCollider

//    [Header("Ефекти")]
//    public ParticleSystem pickupEffect;
//    public AudioClip pickupSound;

//    private bool isPermanentlyCollected = false; // Чи зібрано вже в базі даних?
//    private bool isCollectedInRun = false;       // Чи зібрано в цій спробі?

//    private void Start()
//    {
//        // 1. Підписуємось на подію рестарту від LevelManager
//        if (LevelManager.Instance != null)
//        {
//            LevelManager.Instance.OnLevelReset += ResetCoin;
//        }

//        // 2. Перевіряємо базу даних при запуску
//        CheckDatabase();
//    }

//    private void OnDestroy()
//    {
//        // Важливо! Відписуємось, щоб не було помилок при виході з гри
//        if (LevelManager.Instance != null)
//        {
//            LevelManager.Instance.OnLevelReset -= ResetCoin;
//        }
//    }

//    // Цей метод викликається при старті та при кожному респавні гравця
//    private void ResetCoin()
//    {
//        CheckDatabase(); // Оновлюємо інфу з бази

//        if (isPermanentlyCollected)
//        {
//            // Якщо алмаз вже є в базі (пройшли рівень з ним раніше) -> Ховаємо назавжди
//            HideCoin();
//        }
//        else
//        {
//            // Якщо не в базі -> Показуємо знову!
//            ShowCoin();
//        }
//    }

//    private void CheckDatabase()
//    {
//        if (DatabaseManager.Instance != null)
//        {
//            string level = SceneManager.GetActiveScene().name;
//            isPermanentlyCollected = DatabaseManager.Instance.IsStarCoinCollected(level, coinID);
//        }
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        // Якщо вже підібрали або це не гравець - ігноруємо
//        if (isCollectedInRun || isPermanentlyCollected) return;

//        if (other.CompareTag("Player"))
//        {
//            Collect();
//        }
//    }

//    private void Collect()
//    {
//        isCollectedInRun = true;

//        // Повідомляємо менеджеру
//        if (LevelManager.Instance != null)
//        {
//            LevelManager.Instance.CollectCoinTemp(coinID);
//        }

//        // Ефекти
//        if (pickupEffect) Instantiate(pickupEffect, transform.position, Quaternion.identity);
//        if (pickupSound) AudioSource.PlayClipAtPoint(pickupSound, transform.position);

//        // Ховаємо монету (але не знищуємо об'єкт!)
//        HideCoin();
//    }

//    private void ShowCoin()
//    {
//        isCollectedInRun = false;
//        if (visualModel) visualModel.SetActive(true);
//        if (myCollider) myCollider.enabled = true;
//    }

//    private void HideCoin()
//    {
//        if (visualModel) visualModel.SetActive(false);
//        if (myCollider) myCollider.enabled = false;
//    }
//}
using UnityEngine;
using UnityEngine.SceneManagement;

public class StarCoin : MonoBehaviour
{
    [Header("Налаштування")]
    [Tooltip("0, 1 або 2")]
    public int coinID;


    public float bobSpeed = 2f;   // Швидкість коливання вверх-вниз
    public float bobHeight = 0.5f; // Наскільки високо піднімається
    private Vector3 startPos;
    public float rotateSpeed = 100f; // Швидкість обертання для краси

    [Header("Компоненти")]
    public GameObject visualModel; // Об'єкт з MeshRenderer
    public Collider myCollider;

    [Header("Матеріали")]
    public Material ghostMaterial; // <-- Перетягни сюди створений прозорий матеріал
    private Material normalMaterial; // Тут ми збережемо оригінальний колір
    private Renderer myRenderer;

    [Header("Ефекти")]
    public ParticleSystem pickupEffect;
    public AudioClip pickupSound;

    private bool isPermanentlyCollected = false; // Вже в базі?
    private bool isCollectedInRun = false;       // Взяли тільки що?

    private void Awake()
    {
        // Отримуємо рендерер, щоб міняти матеріали
        if (visualModel != null)
        {
            myRenderer = visualModel.GetComponent<Renderer>();
            if (myRenderer != null)
            {
                // Запам'ятовуємо оригінальний вигляд алмазу
                normalMaterial = myRenderer.material;
            }
        }
    }

    private void Start()
    {
        startPos = transform.position; // Запам'ятовуємо, де стояла монета
        // Підписка на події
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevelReset += ResetCoin;
        }
        ResetCoin(); // Перевірка при старті
    }

    private void OnDestroy()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevelReset -= ResetCoin;
        }
    }

    void Update()
    {
        // Просто крутимо монетку навколо своєї осі
        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);

        // 2. Левітація (Bobbing) - Синусоїда
        // newY змінюється від -0.5 до +0.5 плавно
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;

        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    // Викликається при старті гри і при смерті гравця
    private void ResetCoin()
    {
        isCollectedInRun = false;
        CheckDatabase();

        if (isPermanentlyCollected)
        {
            // ВАРІАНТ 2: Робимо привид
            MakeGhost();
        }
        else
        {
            // Робимо нормальний вигляд
            MakeNormal();
        }
    }

    private void CheckDatabase()
    {
        if (DatabaseManager.Instance != null)
        {
            string level = SceneManager.GetActiveScene().name;
            isPermanentlyCollected = DatabaseManager.Instance.IsStarCoinCollected(level, coinID);
        }
    }

    private void MakeGhost()
    {
        // Показуємо модель
        if (visualModel) visualModel.SetActive(true);

        // Але вимикаємо колайдер (щоб не можна було взяти знову)
        if (myCollider) myCollider.enabled = false;

        // Міняємо матеріал на прозорий
        if (myRenderer != null && ghostMaterial != null)
        {
            myRenderer.material = ghostMaterial;
        }
    }

    private void MakeNormal()
    {
        // Показуємо модель
        if (visualModel) visualModel.SetActive(true);

        // Вмикаємо колайдер
        if (myCollider) myCollider.enabled = true;

        // Повертаємо нормальний матеріал
        if (myRenderer != null && normalMaterial != null)
        {
            myRenderer.material = normalMaterial;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Якщо це привид або вже взяли в цьому забігу - ігноруємо
        if (isCollectedInRun || isPermanentlyCollected) return;

        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    private void Collect()
    {
        isCollectedInRun = true;

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.CollectCoinTemp(coinID);
        }

        if (pickupEffect) Instantiate(pickupEffect, transform.position, Quaternion.identity);

        // ---> ЗМІНА ТУТ: Читаємо гучність SFX <---
        if (pickupSound)
        {
            float volume = PlayerPrefs.GetFloat("SFXVolume", 1f); // За замовчуванням 1 (максимум)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, volume);
        }

        // Коли підбираємо під час гри - ховаємо повністю!
        if (visualModel) visualModel.SetActive(false);
        if (myCollider) myCollider.enabled = false;
    }
}