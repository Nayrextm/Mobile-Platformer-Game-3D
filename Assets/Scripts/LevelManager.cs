
//using System; // Потрібно для Action
//using System.Collections;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class LevelManager : MonoBehaviour
//{
//    public static LevelManager Instance;

//    [Header("Налаштування")]
//    public Transform spawnPoint;
//    public float restartDelay = 0.6f;
//    public WinScreenController winScreen;
//    public CameraFollow cameraScript;
//    public MusicController musicController;

//    // ---> ПОДІЯ РЕСТАРТУ (Щоб монети знали, коли з'явитися знову)
//    public event Action OnLevelReset;

//    // Тимчасова "кишеня" для алмазів у поточному житті
//    private bool[] tempCoins = new bool[3];

//    private bool isRestarting = false;
//    private int sessionAttempts = 1;
//    private float sessionStartTime;
//    private float lastSaveTime;

//    private void Awake()
//    {
//        if (Instance == null) Instance = this;
//        else Destroy(gameObject);
//    }

//    private void Start()
//    {
//        sessionStartTime = Time.time;
//        lastSaveTime = Time.time;
//        // Скидаємо кишеню
//        tempCoins = new bool[] { false, false, false };

//        if (cameraScript != null) cameraScript.ResetCamera();
//        if (musicController != null) musicController.RestartMusic();
//    }

//    // 1. КОЛИ ПІДІБРАЛИ МОНЕТУ
//    public void CollectCoinTemp(int coinID)
//    {
//        if (coinID >= 0 && coinID < 3)
//        {
//            tempCoins[coinID] = true;
//            Debug.Log($"Алмаз {coinID + 1} в кишені (поки що)!");
//        }
//    }

//    // 2. КОЛИ ПОМЕРЛИ
//    public void PlayerDied(PlayerController player)
//    {
//        if (isRestarting) return;
//        isRestarting = true;

//        // Очищаємо кишеню (втратили все, що назбирали)
//        tempCoins = new bool[] { false, false, false };

//        // Зберігаємо статистику смерті
//        SaveAttemptsToDB();

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

//            // ---> ГОЛОВНИЙ МОМЕНТ: Відправляємо сигнал "Рестарт" усім об'єктам
//            // Усі монети підпишуться на цю подію і відновляться
//            OnLevelReset?.Invoke();

//            if (cameraScript != null) cameraScript.ResetCamera();
//            if (musicController != null) musicController.RestartMusic();

//            yield return new WaitForEndOfFrame();
//            isRestarting = false;
//        }
//        else
//        {
//            // Якщо щось пішло не так - перезавантажуємо сцену
//            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
//        }
//    }

//    // 3. КОЛИ ВИГРАЛИ
//    public void LevelFinished(PlayerController player)
//    {
//        if (isRestarting) return;
//        isRestarting = true;

//        if (DatabaseManager.Instance != null)
//        {
//            string levelName = SceneManager.GetActiveScene().name;

//            // Зберігаємо алмази
//            DatabaseManager.Instance.SaveStarCoins(levelName, tempCoins);

//            // ---> НОВЕ: Зберігаємо статус "Пройдено"
//            DatabaseManager.Instance.MarkLevelComplete(levelName);
//        }

//        SaveAttemptsToDB();
//        if (musicController != null) musicController.StopMusic();

//        player.Win();
//        if (winScreen != null)
//            winScreen.Show(Time.time - sessionStartTime, sessionAttempts);
//    }

//    private void SaveAttemptsToDB()
//    {
//        if (DatabaseManager.Instance == null) return;
//        float deltaTime = Time.time - lastSaveTime;
//        lastSaveTime = Time.time;
//        string levelName = SceneManager.GetActiveScene().name;
//        // Зберігаємо час. Якщо це смерть - додаємо 1 спробу, якщо просто сейв - 0.
//        int attemptsToAdd = isRestarting ? 1 : 0;
//        DatabaseManager.Instance.SaveProgress(levelName, attemptsToAdd, deltaTime);
//    }


//}
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    
    public static LevelManager Instance { get; private set; }

    [Header("Налаштування")]
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private float _restartDelay = 0.6f;
    [SerializeField] private WinScreenController _winScreen;
    [SerializeField] private CameraFollow _cameraScript;
    [SerializeField] private MusicController _musicController;

    public event Action OnLevelReset;

    private readonly bool[] _tempCoins = new bool[3];

    private bool _isRestarting = false;
    private int _sessionAttempts = 1;
    private float _sessionStartTime;
    private float _lastSaveTime;

   
    private string _currentLevelName;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return; 
        }

       
        _currentLevelName = SceneManager.GetActiveScene().name;
    }

    private void Start()
    {
        _sessionStartTime = Time.time;
        _lastSaveTime = Time.time;

        ResetTempCoins();

        if (_cameraScript != null) _cameraScript.ResetCamera();
        if (_musicController != null) _musicController.RestartMusic();
    }

    public void CollectCoinTemp(int coinID)
    {
        if (coinID >= 0 && coinID < _tempCoins.Length)
        {
            _tempCoins[coinID] = true;
        }
    }

    public void PlayerDied(PlayerController player)
    {
        if (_isRestarting) return;
        _isRestarting = true;

        ResetTempCoins();
        SaveAttemptsToDB(true);

        _sessionAttempts++;
        if (_musicController != null) _musicController.FadeOutMusic();

        StartCoroutine(HandlePlayerDeath(player));
    }

    private IEnumerator HandlePlayerDeath(PlayerController player)
    {
        yield return new WaitForSeconds(_restartDelay);

        if (player != null && _spawnPoint != null)
        {
            player.RespawnAt(_spawnPoint);
            OnLevelReset?.Invoke();

            if (_cameraScript != null) _cameraScript.ResetCamera();
            if (_musicController != null) _musicController.RestartMusic();

            _isRestarting = false;
        }
        else
        {
            SceneManager.LoadScene(_currentLevelName);
        }
    }

    public void LevelFinished(PlayerController player)
    {
        if (_isRestarting) return;
        _isRestarting = true;

        if (DatabaseManager.Instance != null)
        {
            DatabaseManager.Instance.SaveStarCoins(_currentLevelName, _tempCoins);
            DatabaseManager.Instance.MarkLevelComplete(_currentLevelName);
        }

        SaveAttemptsToDB(false); 
        if (_musicController != null) _musicController.StopMusic();

        player.Win();
        if (_winScreen != null)
        {
            _winScreen.Show(Time.time - _sessionStartTime, _sessionAttempts);
        }
    }

    private void SaveAttemptsToDB(bool isDeath)
    {
        if (DatabaseManager.Instance == null) return;

        float deltaTime = Time.time - _lastSaveTime;
        _lastSaveTime = Time.time;

        int attemptsToAdd = isDeath ? 1 : 0;
        DatabaseManager.Instance.SaveProgress(_currentLevelName, attemptsToAdd, deltaTime);
    }

    private void ResetTempCoins()
    {
        
        Array.Clear(_tempCoins, 0, _tempCoins.Length);
    }
}