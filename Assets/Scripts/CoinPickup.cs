//using UnityEngine;

//public class CoinPickup : MonoBehaviour
//{
//    [Header("Налаштування")]
//    public int coinValue = 1; // Скільки монет дає цей об'єкт
//    public float rotateSpeed = 100f; // Швидкість обертання для краси

//    [Header("Ефекти")]
//    public GameObject pickupEffect; // Префаб ефекту (іскри/спалах)
//    public AudioClip pickupSound;   // Звук підбору

//    private bool isCollected = false; // Захист від подвійного спрацювання

//    void Update()
//    {
//        // Просто крутимо монетку навколо своєї осі
//        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
//    }

//    void OnTriggerEnter(Collider other)
//    {
//        // Перевіряємо, чи зайшов у зону гравець (переконайтесь, що у гравця тег "Player")
//        if (other.CompareTag("Player") && !isCollected)
//        {
//            Collect();
//        }
//    }

//    void Collect()
//    {
//        isCollected = true;

//        // 1. Зберігаємо в базу даних (Глобальна статистика)
//        if (DatabaseManager.Instance != null)
//        {
//            DatabaseManager.Instance.AddCoins(coinValue);
//        }
//        else
//        {
//            Debug.LogError("Не знайдено DatabaseManager! Переконайтеся, що він є на сцені.");
//        }

//        // 2. Оновлюємо UI (якщо він є на сцені)
//        if (CoinUI.Instance != null)
//        {
//            CoinUI.Instance.UpdateDisplay();
//        }

//        // 3. Візуальні та звукові ефекти
//        if (pickupEffect != null)
//        {
//            // Створюємо ефект в точці монети
//            Instantiate(pickupEffect, transform.position, Quaternion.identity);
//        }

//        if (pickupSound != null)
//        {
//            // Створюємо звук в точці монети (AudioSource.PlayClipAtPoint працює навіть після знищення об'єкта)
//            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
//        }

//        // 4. Знищуємо саму монетку
//        Destroy(gameObject);
//    }
//}
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoinPickup : MonoBehaviour
{
    [Header("Налаштування")]
    public int coinValue = 1; // Скільки грошей дає

    [Header("Візуалізація (Левітація та Обертання)")]
    public float rotateSpeed = 100f; // Швидкість обертання
    public float bobSpeed = 2f;      // Швидкість руху вверх-вниз
    public float bobHeight = 0.5f;   // Амплітуда руху (висота)

    private Vector3 startPos;        // Початкова позиція (центр коливання)

    [Header("Ефекти")]
    public GameObject pickupEffect;
    public AudioClip pickupSound;

    private bool isCollected = false;
    private string myID; // Унікальний паспорт монети для бази даних

    void Start()
    {
        // 1. Запам'ятовуємо позицію для левітації
        startPos = transform.position;

        // 2. ГЕНЕРУЄМО УНІКАЛЬНИЙ ID (НазваРівня + Координати)
        // Використовуємо F2 для точності (щоб 10.5 не стало 10.500001)
        string posString = $"{startPos.x:F2}_{startPos.y:F2}_{startPos.z:F2}";
        myID = $"{SceneManager.GetActiveScene().name}_{posString}";

        // 3. ПЕРЕВІРЯЄМО БАЗУ
        if (DatabaseManager.Instance != null)
        {
            // Якщо така монета вже є в базі (ми її збирали раніше)
            if (DatabaseManager.Instance.IsCoinCollected(myID))
            {
                // Вимикаємо її одразу, гравець її не побачить
                gameObject.SetActive(false);
                isCollected = true;
            }
        }
    }

    void Update()
    {
        if (isCollected) return;

        // --- ВІЗУАЛЬНА ЛОГІКА (Як у StarCoin) ---

        // 1. Обертання навколо осі Z
        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);

        // 2. Левітація (Bobbing) - Синусоїда
        // newY плавно змінюється від (startPos.y - bobHeight) до (startPos.y + bobHeight)
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;

        // Застосовуємо нову позицію
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            Collect();
        }
    }

    void Collect()
    {
        isCollected = true;

        if (DatabaseManager.Instance != null)
        {
            // 1. Додаємо гроші в гаманець
            DatabaseManager.Instance.AddCoins(coinValue);

            // 2. Запам'ятовуємо цю монету як зібрану (щоб вона зникла назавжди до Ресету)
            DatabaseManager.Instance.MarkCoinAsCollected(myID);
        }

        // Оновлюємо UI
        if (CoinUI.Instance != null) CoinUI.Instance.UpdateDisplay();

        // Ефекти
        if (pickupEffect != null) Instantiate(pickupEffect, transform.position, Quaternion.identity);

        // ---> ЗМІНА ТУТ: Читаємо гучність SFX <---
        if (pickupSound != null)
        {
            float volume = PlayerPrefs.GetFloat("SFXVolume", 1f); // Читаємо з налаштувань
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, volume);
        }
        // Ховаємо
        gameObject.SetActive(false);
    }
}