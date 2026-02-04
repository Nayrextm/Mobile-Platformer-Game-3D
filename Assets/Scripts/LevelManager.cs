//using System.Collections;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class LevelManager : MonoBehaviour
//{
//    public Transform spawnPoint;
//    public float restartDelay = 0.6f;
//    public WinScreenController winScreen;

//    // СЕСІЙНА СТАТИСТИКА (Скидається, коли виходиш в меню)
//    [HideInInspector] public int sessionAttempts = 1;
//    private float sessionStartTime;
//    private float lastAttemptStartTime; // Щоб рахувати час конкретної спроби для бази

//    private void Start()
//    {
//        // Фіксуємо початок сесії (коли зайшли з меню)
//        sessionStartTime = Time.time;
//        lastAttemptStartTime = Time.time;
//        sessionAttempts = 1;
//    }

//    // Цей метод зберігає прогрес в базу (додає +1 спробу і час цієї спроби)
//    private void SaveAttemptToDB()
//    {
//        if (DatabaseManager.Instance != null)
//        {
//            // Рахуємо скільки тривала САМЕ ЦЯ коротка спроба (від спавну до смерті)
//            float duration = Time.time - lastAttemptStartTime;

//            string levelName = SceneManager.GetActiveScene().name;
//            DatabaseManager.Instance.SaveProgress(levelName, duration);
//        }
//    }

//    public void PlayerDied(PlayerController player)
//    {
//        // 1. Зберігаємо цю невдалу спробу в ГЛОБАЛЬНУ базу даних
//        SaveAttemptToDB();

//        // 2. Починаємо процедуру респауну
//        StartCoroutine(HandlePlayerDeath(player));
//    }

//    IEnumerator HandlePlayerDeath(PlayerController player)
//    {
//        yield return new WaitForSeconds(restartDelay);

//        if (player != null && spawnPoint != null)
//        {
//            // Респаун гравця
//            player.RespawnAt(spawnPoint);

//            // Оновлюємо СЕСІЙНУ статистику
//            sessionAttempts++;
//            lastAttemptStartTime = Time.time; // Скидаємо таймер для нової спроби
//        }
//        else
//        {
//            // Якщо раптом використовуєш перезавантаження сцени
//            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
//        }
//    }

//    public void LevelFinished(PlayerController player)
//    {
//        // Зберігаємо останню успішну спробу в базу
//        SaveAttemptToDB();

//        player.Win();

//        if (winScreen != null)
//        {
//            // Для WinScreen показуємо СЕСІЙНИЙ час (скільки сидів на рівні з моменту заходу)
//            float totalSessionTime = Time.time - sessionStartTime;

//            // Передаємо сесійні дані (скільки мучився саме зараз)
//            winScreen.Show(totalSessionTime, sessionAttempts);
//        }
//    }
//}
//using System.Collections;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class LevelManager : MonoBehaviour
//{
//    [Header("Налаштування")]
//    public Transform spawnPoint;
//    public float restartDelay = 0.6f;
//    public WinScreenController winScreen;

//    // ---> ДОДАНО: Посилання на камеру
//    public CameraFollow cameraScript;

//    // ---> ДОДАНО: Посилання на музичний контролер
//    public MusicController musicController;

//    // СЕСІЙНА СТАТИСТИКА
//    [HideInInspector] public int sessionAttempts = 0;
//    private float sessionStartTime;

//    // ТЕХНІЧНА ЗМІННА
//    private float lastSaveTime;

//    private void Start()
//    {
//        sessionAttempts = 1;
//        sessionStartTime = Time.time;
//        lastSaveTime = Time.time;

//        // На старті теж вирівнюємо камеру, щоб не було ривка при вході
//        if (cameraScript != null) cameraScript.ResetCamera();

//        // ---> ДОДАНО: Гарантовано запускаємо музику при старті
//        if (musicController != null) musicController.RestartMusic();
//    }

//    private void SaveToDatabase()
//    {
//        if (DatabaseManager.Instance == null) return;
//        float deltaTime = Time.time - lastSaveTime;
//        lastSaveTime = Time.time;
//        string levelName = SceneManager.GetActiveScene().name;
//        DatabaseManager.Instance.SaveProgress(levelName, deltaTime);
//    }

//    public void PlayerDied(PlayerController player)
//    {
//        SaveToDatabase();
//        sessionAttempts++;

//        // ---> ДОДАНО: Музика стихає при смерті
//        if (musicController != null) musicController.FadeOutMusic();

//        StartCoroutine(HandlePlayerDeath(player));
//    }

//    IEnumerator HandlePlayerDeath(PlayerController player)
//    {
//        yield return new WaitForSeconds(restartDelay);

//        if (player != null && spawnPoint != null)
//        {
//            // 1. Повертаємо гравця
//            player.RespawnAt(spawnPoint);

//            // 2. ---> ДОДАНО: Миттєво повертаємо камеру
//            if (cameraScript != null)
//            {
//                cameraScript.ResetCamera();
//            }

//            // ---> ДОДАНО: Музика починається з початку разом з респауном
//            if (musicController != null) musicController.RestartMusic();
//        }
//        else
//        {
//            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
//        }
//    }

//    public void LevelFinished(PlayerController player)
//    {
//        SaveToDatabase();

//        // ---> ДОДАНО: Зупиняємо музику повністю при фініші
//        if (musicController != null) musicController.StopMusic();

//        player.Win();

//        if (winScreen != null)
//        {
//            float totalSessionTime = Time.time - sessionStartTime;
//            winScreen.Show(totalSessionTime, sessionAttempts);
//        }
//    }
//}
//using System.Collections;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class LevelManager : MonoBehaviour
//{
//    [Header("Налаштування")]
//    public Transform spawnPoint;
//    public float restartDelay = 0.6f;
//    public WinScreenController winScreen;

//    public CameraFollow cameraScript;
//    public MusicController musicController;

//    [HideInInspector] public int sessionAttempts = 0;
//    private float sessionStartTime;
//    private float lastSaveTime;

//    // Захист від повторного виклику смерті
//    private bool isRestarting = false;

//    private void Start()
//    {
//        sessionAttempts = 1;
//        sessionStartTime = Time.time;
//        lastSaveTime = Time.time;
//        isRestarting = false; // Скидаємо прапорець

//        if (cameraScript != null) cameraScript.ResetCamera();
//        if (musicController != null) musicController.RestartMusic();
//    }

//    private void SaveToDatabase()
//    {
//        if (DatabaseManager.Instance == null) return;
//        float deltaTime = Time.time - lastSaveTime;
//        lastSaveTime = Time.time;
//        string levelName = SceneManager.GetActiveScene().name;
//        // Тут використовується версія з 3 параметрами, яка вже працює
//        DatabaseManager.Instance.SaveProgress(levelName, 0, deltaTime);
//    }

//    public void PlayerDied(PlayerController player)
//    {
//        // ---> БЛОКУВАННЯ: Якщо рестарт вже йде, виходимо
//        if (isRestarting) return;
//        isRestarting = true;

//        SaveToDatabase();

//        // Окремий запис спроби в базу (1 спроба, 0 часу - час вже зберігся вище)
//        if (DatabaseManager.Instance != null)
//        {
//            string levelName = SceneManager.GetActiveScene().name;
//            DatabaseManager.Instance.SaveProgress(levelName, 1, 0);
//        }

//        sessionAttempts++;

//        if (musicController != null) musicController.FadeOutMusic();

//        StartCoroutine(HandlePlayerDeath(player));
//    }

//    IEnumerator HandlePlayerDeath(PlayerController player)
//    {
//        yield return new WaitForSeconds(restartDelay);

//        if (player != null && spawnPoint != null)
//        {
//            player.RespawnAt(spawnPoint);

//            if (cameraScript != null) cameraScript.ResetCamera();
//            if (musicController != null) musicController.RestartMusic();


//            // ---> ДОДАЙ ЦЕЙ РЯДОК <---
//            // Чекаємо кінця кадру, щоб фізика точно оновилася після телепортації
//            yield return new WaitForEndOfFrame();
//            // ---> РОЗБЛОКУВАННЯ: Дозволяємо помирати знову
//            isRestarting = false;
//        }
//        else
//        {
//            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
//        }
//    }

//    public void LevelFinished(PlayerController player)
//    {
//        if (isRestarting) return; // Якщо влетів у фініш трупом
//        isRestarting = true;

//        SaveToDatabase();
//        if (musicController != null) musicController.StopMusic();

//        player.Win();

//        if (winScreen != null)
//        {
//            float totalSessionTime = Time.time - sessionStartTime;
//            winScreen.Show(totalSessionTime, sessionAttempts);
//        }
//    }
//}
//using System.Collections;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class LevelManager : MonoBehaviour
//{
//    // 1. РОБИМО SINGLETON (Щоб StarCoin міг знайти цей скрипт)
//    public static LevelManager Instance;

//    [Header("Налаштування")]
//    public Transform spawnPoint;
//    public float restartDelay = 0.6f;
//    public WinScreenController winScreen;

//    public CameraFollow cameraScript;
//    public MusicController musicController;

//    [HideInInspector] public int sessionAttempts = 0;
//    private float sessionStartTime;
//    private float lastSaveTime;

//    private bool isRestarting = false;

//    // 2. ТИМЧАСОВА ПАМ'ЯТЬ ДЛЯ МОНЕТ (Поки гравець не пройшов рівень)
//    private bool[] starCoinsInRun = new bool[3];

//    private void Awake()
//    {
//        // Ініціалізація Singleton
//        if (Instance == null) Instance = this;
//        else Destroy(gameObject);
//    }

//    private void Start()
//    {
//        sessionAttempts = 1;
//        sessionStartTime = Time.time;
//        lastSaveTime = Time.time;
//        isRestarting = false;

//        // Скидаємо монети на початку
//        starCoinsInRun = new bool[] { false, false, false };

//        if (cameraScript != null) cameraScript.ResetCamera();
//        if (musicController != null) musicController.RestartMusic();
//    }

//    // 3. МЕТОД ДЛЯ ПІДБОРУ (Викликається зі скрипта StarCoin.cs)
//    public void CollectStarCoinTemp(int coinIndex)
//    {
//        if (coinIndex >= 0 && coinIndex < 3)
//        {
//            starCoinsInRun[coinIndex] = true;
//            Debug.Log($"Алмаз #{coinIndex + 1} підібрано (Тимчасово)!");
//        }
//    }

//    private void SaveToDatabase()
//    {
//        if (DatabaseManager.Instance == null) return;
//        float deltaTime = Time.time - lastSaveTime;
//        lastSaveTime = Time.time;
//        string levelName = SceneManager.GetActiveScene().name;
//        DatabaseManager.Instance.SaveProgress(levelName, 0, deltaTime);
//    }

//    public void PlayerDied(PlayerController player)
//    {
//        if (isRestarting) return;
//        isRestarting = true;

//        // 4. ВАЖЛИВО: СКИДАЄМО МОНЕТИ ПРИ СМЕРТІ
//        // Гравець помер — значить не доніс алмази до фінішу.
//        starCoinsInRun = new bool[] { false, false, false };

//        SaveToDatabase();

//        if (DatabaseManager.Instance != null)
//        {
//            string levelName = SceneManager.GetActiveScene().name;
//            DatabaseManager.Instance.SaveProgress(levelName, 1, 0);
//        }

//        sessionAttempts++;

//        if (musicController != null) musicController.FadeOutMusic();

//        StartCoroutine(HandlePlayerDeath(player));
//    }

//    IEnumerator HandlePlayerDeath(PlayerController player)
//    {
//        yield return new WaitForSeconds(restartDelay);

//        if (player != null && spawnPoint != null)
//        {
//            player.RespawnAt(spawnPoint);

//            if (cameraScript != null) cameraScript.ResetCamera();
//            if (musicController != null) musicController.RestartMusic();

//            yield return new WaitForEndOfFrame();
//            isRestarting = false;
//        }
//        else
//        {
//            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
//        }
//    }

//    public void LevelFinished(PlayerController player)
//    {
//        if (isRestarting) return;
//        isRestarting = true;

//        // 5. ЗБЕРІГАЄМО АЛМАЗИ В БАЗУ ТІЛЬКИ ТУТ (ПРИ ПЕРЕМОЗІ)
//        if (DatabaseManager.Instance != null)
//        {
//            string levelName = SceneManager.GetActiveScene().name;
//            DatabaseManager.Instance.SaveStarCoins(levelName, starCoinsInRun);
//        }

//        SaveToDatabase();
//        if (musicController != null) musicController.StopMusic();

//        player.Win();

//        if (winScreen != null)
//        {
//            float totalSessionTime = Time.time - sessionStartTime;
//            winScreen.Show(totalSessionTime, sessionAttempts);
//        }
//    }
//}
using System; // Потрібно для Action
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Налаштування")]
    public Transform spawnPoint;
    public float restartDelay = 0.6f;
    public WinScreenController winScreen;
    public CameraFollow cameraScript;
    public MusicController musicController;

    // ---> ПОДІЯ РЕСТАРТУ (Щоб монети знали, коли з'явитися знову)
    public event Action OnLevelReset;

    // Тимчасова "кишеня" для алмазів у поточному житті
    private bool[] tempCoins = new bool[3];

    private bool isRestarting = false;
    private int sessionAttempts = 1;
    private float sessionStartTime;
    private float lastSaveTime;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        sessionStartTime = Time.time;
        lastSaveTime = Time.time;
        // Скидаємо кишеню
        tempCoins = new bool[] { false, false, false };

        if (cameraScript != null) cameraScript.ResetCamera();
        if (musicController != null) musicController.RestartMusic();
    }

    // 1. КОЛИ ПІДІБРАЛИ МОНЕТУ
    public void CollectCoinTemp(int coinID)
    {
        if (coinID >= 0 && coinID < 3)
        {
            tempCoins[coinID] = true;
            Debug.Log($"Алмаз {coinID + 1} в кишені (поки що)!");
        }
    }

    // 2. КОЛИ ПОМЕРЛИ
    public void PlayerDied(PlayerController player)
    {
        if (isRestarting) return;
        isRestarting = true;

        // Очищаємо кишеню (втратили все, що назбирали)
        tempCoins = new bool[] { false, false, false };

        // Зберігаємо статистику смерті
        SaveAttemptsToDB();

        sessionAttempts++;
        if (musicController != null) musicController.FadeOutMusic();

        StartCoroutine(HandlePlayerDeath(player));
    }

    IEnumerator HandlePlayerDeath(PlayerController player)
    {
        yield return new WaitForSeconds(restartDelay);

        if (player != null && spawnPoint != null)
        {
            player.RespawnAt(spawnPoint);

            // ---> ГОЛОВНИЙ МОМЕНТ: Відправляємо сигнал "Рестарт" усім об'єктам
            // Усі монети підпишуться на цю подію і відновляться
            OnLevelReset?.Invoke();

            if (cameraScript != null) cameraScript.ResetCamera();
            if (musicController != null) musicController.RestartMusic();

            yield return new WaitForEndOfFrame();
            isRestarting = false;
        }
        else
        {
            // Якщо щось пішло не так - перезавантажуємо сцену
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // 3. КОЛИ ВИГРАЛИ
    public void LevelFinished(PlayerController player)
    {
        if (isRestarting) return;
        isRestarting = true;

        if (DatabaseManager.Instance != null)
        {
            string levelName = SceneManager.GetActiveScene().name;

            // Зберігаємо алмази
            DatabaseManager.Instance.SaveStarCoins(levelName, tempCoins);

            // ---> НОВЕ: Зберігаємо статус "Пройдено"
            DatabaseManager.Instance.MarkLevelComplete(levelName);
        }

        SaveAttemptsToDB();
        if (musicController != null) musicController.StopMusic();

        player.Win();
        if (winScreen != null)
            winScreen.Show(Time.time - sessionStartTime, sessionAttempts);
    }

    private void SaveAttemptsToDB()
    {
        if (DatabaseManager.Instance == null) return;
        float deltaTime = Time.time - lastSaveTime;
        lastSaveTime = Time.time;
        string levelName = SceneManager.GetActiveScene().name;
        // Зберігаємо час. Якщо це смерть - додаємо 1 спробу, якщо просто сейв - 0.
        int attemptsToAdd = isRestarting ? 1 : 0;
        DatabaseManager.Instance.SaveProgress(levelName, attemptsToAdd, deltaTime);
    }


}