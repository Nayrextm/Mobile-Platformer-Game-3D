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
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        _currentLevelName = SceneManager.GetActiveScene().name;

        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
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
        if (coinID >= 0 && coinID < _tempCoins.Length) _tempCoins[coinID] = true;
    }

    public void PlayerDied(PlayerController player)
    {
        if (_isRestarting) return;
        _isRestarting = true;

       
        ResetTempCoins();

        
        SaveAttemptsToDB(true);
        if (DatabaseManager.Instance != null) DatabaseManager.Instance.SaveAllPendingDataToDisk();

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
            if (PoolManager.Instance != null) PoolManager.Instance.HideAllActiveEffects();

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
            
            DatabaseManager.Instance.SaveAllPendingDataToDisk();
        }

        SaveAttemptsToDB(false);

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

    private void ResetTempCoins() => Array.Clear(_tempCoins, 0, _tempCoins.Length);
}