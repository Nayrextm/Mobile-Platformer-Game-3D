using UnityEngine;
using SQLite4Unity3d;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }

    public bool IsReady { get; private set; }

    private SQLiteConnection _connection;
    private readonly string _dbName = "GeoDashStats.db";

    private HashSet<string> _collectedCoinsCache = new HashSet<string>();
    private CollectiblesStat _cachedWallet;
    private List<CoinState> _coinsToSaveToDisk = new List<CoinState>();

    private void Awake()
    {
        Debug.unityLogger.logEnabled = Debug.isDebugBuild;

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            StartCoroutine(InitializeDatabaseAsync());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator InitializeDatabaseAsync()
    {
        string dbPath = Path.Combine(Application.persistentDataPath, _dbName);

#if !UNITY_EDITOR
        // Якщо файлу на пристрої гравця ще немає - асинхронно дістаємо його
        if (!File.Exists(dbPath))
        {
            string sourcePath = Application.streamingAssetsPath + "/" + _dbName;

            if (sourcePath.Contains("://") || sourcePath.Contains(":///"))
            {
                using (UnityWebRequest request = UnityWebRequest.Get(sourcePath))
                {
                    yield return request.SendWebRequest(); 

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        try 
                        {
                            File.WriteAllBytes(dbPath, request.downloadHandler.data);
                            Debug.Log("БД успішно скопійована з APK (Асинхронно)!");
                        }
                        catch (System.Exception e) { Debug.LogError("Помилка запису: " + e.Message); }
                    }
                    else
                    {
                        Debug.LogError("Помилка копіювання БД на Android: " + request.error);
                    }
                }
            }
            else
            {
                try { if (File.Exists(sourcePath)) File.Copy(sourcePath, dbPath); }
                catch (System.Exception e) { Debug.LogError("Помилка копіювання: " + e.Message); }
            }
        }
#else
        string folderPath = Path.Combine(Application.dataPath, "Database");
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
        dbPath = Path.Combine(folderPath, _dbName);
#endif

        try
        {
            ConnectAndCache(dbPath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Критична помилка БД: " + e.Message);
            if (File.Exists(dbPath))
            {
                File.Delete(dbPath);
                ConnectAndCache(dbPath);
            }
        }

        yield break;
    }

    private void ConnectAndCache(string finalPath)
    {
        _connection = new SQLiteConnection(finalPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

        _connection.CreateTable<LevelStat>();
        _connection.CreateTable<CollectiblesStat>();
        _connection.CreateTable<CoinState>();
        _connection.CreateTable<UserSettings>();
        _connection.CreateTable<UnlockedItem>();

        LoadDataToCache();

        IsReady = true;
    }

    public int GetSelectedSkinID()
    {
        if (_connection == null) return 0;
        var settings = _connection.Table<UserSettings>().FirstOrDefault(x => x.Id == 1);
        return settings != null ? settings.SelectedSkinID : 0;
    }

    public void SaveSelectedSkinID(int skinID)
    {
        var settings = _connection.Table<UserSettings>().FirstOrDefault(x => x.Id == 1) ?? new UserSettings { Id = 1 };
        settings.SelectedSkinID = skinID;
        _connection.InsertOrReplace(settings);
    }

    public int GetSelectedColorIndex()
    {
        if (_connection == null) return 0;
        var settings = _connection.Table<UserSettings>().FirstOrDefault(x => x.Id == 1);
        return settings != null ? settings.SelectedColorIndex : 0;
    }

    public void SaveSelectedColorIndex(int colorIndex)
    {
        var settings = _connection.Table<UserSettings>().FirstOrDefault(x => x.Id == 1) ?? new UserSettings { Id = 1 };
        settings.SelectedColorIndex = colorIndex;
        _connection.InsertOrReplace(settings);
    }

    public int GetSelectedTrailColorIndex()
    {
        if (_connection == null) return 0;
        var settings = _connection.Table<UserSettings>().FirstOrDefault(x => x.Id == 1);
        return settings != null ? settings.SelectedTrailColorIndex : 0;
    }

    public void SaveSelectedTrailColorIndex(int colorIndex)
    {
        var settings = _connection.Table<UserSettings>().FirstOrDefault(x => x.Id == 1) ?? new UserSettings { Id = 1 };
        settings.SelectedTrailColorIndex = colorIndex;
        _connection.InsertOrReplace(settings);
    }

    private void LoadDataToCache()
    {
      
        _cachedWallet = _connection.Table<CollectiblesStat>().FirstOrDefault(x => x.Id == 1)
                        ?? new CollectiblesStat { Id = 1, TotalCoins = 0 };

        if (_connection.Table<CollectiblesStat>().Count() == 0) _connection.Insert(_cachedWallet);

       
        _collectedCoinsCache.Clear();
        foreach (var c in _connection.Table<CoinState>())
        {
            _collectedCoinsCache.Add(c.GlobalID);
        }
    }

    public LevelStat GetLevelData(string levelName)
    {
        return _connection.Find<LevelStat>(levelName);
    }

    public void DeleteAllData()
    {
        _connection.DeleteAll<LevelStat>();
        _connection.DeleteAll<CoinState>();

        _connection.DeleteAll<UnlockedItem>();

        _connection.DeleteAll<UserSettings>();
        _connection.Insert(new UserSettings
        {
            Id = 1,
            SelectedSkinID = 0,
            SelectedColorIndex = 0,
            SelectedTrailColorIndex = 0
        });

        if (_cachedWallet != null)
        {
            _cachedWallet.TotalCoins = 0;
            _connection.Update(_cachedWallet);
        }

        _collectedCoinsCache.Clear();
        _coinsToSaveToDisk.Clear();

        Debug.Log("Прогрес, покупки та інвентар успішно скинуто до заводських налаштувань!");
    }

    public bool IsCoinAlreadyCollectedInDB(string globalId) => _collectedCoinsCache.Contains(globalId);

    public void CollectCoinImmediate(string globalId, int value)
    {
        if (!_collectedCoinsCache.Contains(globalId))
        {
            _collectedCoinsCache.Add(globalId);
            _coinsToSaveToDisk.Add(new CoinState { GlobalID = globalId });
            _cachedWallet.TotalCoins += value;

            if (_coinsToSaveToDisk.Count >= 4) SaveAllPendingDataToDisk();
        }
    }

    public int GetTotalCoins() => _cachedWallet != null ? _cachedWallet.TotalCoins : 0;

    public int GetCollectedCoinsCount(string levelName)
    {
        int count = 0;
        string prefix = levelName + "_";

        foreach (string globalId in _collectedCoinsCache)
        {
            if (globalId.StartsWith(prefix))
            {
                count++;
            }
        }

        return count;
    }

    public bool IsItemUnlocked(string category, int id)
    {
        if (id == 0) return true; 

        return _connection.Table<UnlockedItem>()
                          .Count(x => x.ItemCategory == category && x.ItemID == id) > 0;
    }

    public bool TrySpendCoins(int amount)
    {
        if (_cachedWallet != null && _cachedWallet.TotalCoins >= amount)
        {
            _cachedWallet.TotalCoins -= amount;
            SaveAllPendingDataToDisk(); 
            return true;
        }
        return false;
    }

    public void UnlockItem(string category, int id)
    {
        if (!IsItemUnlocked(category, id))
        {
            _connection.Insert(new UnlockedItem { ItemCategory = category, ItemID = id });
        }
    }

    public int GetTotalCollectedStarCoins()
    {
        if (_connection == null) return 0;

        string query = "SELECT COALESCE(SUM(StarCoin1 + StarCoin2 + StarCoin3), 0) FROM LevelStat";

        return _connection.ExecuteScalar<int>(query);
    }

    public void SaveAllPendingDataToDisk()
    {
        if (_connection == null) return;
        if (_coinsToSaveToDisk.Count > 0 || _cachedWallet != null)
        {
            _connection.Update(_cachedWallet);
            if (_coinsToSaveToDisk.Count > 0)
            {
                _connection.InsertAll(_coinsToSaveToDisk);
                _coinsToSaveToDisk.Clear();
            }
        }
    }

    private void OnApplicationPause(bool isPaused) { if (isPaused) SaveAllPendingDataToDisk(); }
    private void OnApplicationQuit() => SaveAllPendingDataToDisk();

    public void SaveStarCoins(string lvl, bool[] stars)
    {
        var s = GetLevelData(lvl) ?? new LevelStat { LevelID = lvl };
        if (stars[0]) s.StarCoin1 = true;
        if (stars[1]) s.StarCoin2 = true;
        if (stars[2]) s.StarCoin3 = true;
        _connection.InsertOrReplace(s);
    }

    public bool IsStarCoinCollected(string lvl, int idx)
    {
        var s = GetLevelData(lvl);
        if (s == null) return false;
        return idx == 0 ? s.StarCoin1 : idx == 1 ? s.StarCoin2 : s.StarCoin3;
    }

    public void MarkLevelComplete(string lvl)
    {
        var s = GetLevelData(lvl) ?? new LevelStat { LevelID = lvl };
        s.IsCompleted = true;
        _connection.InsertOrReplace(s);
    }

    public void SaveProgress(string lvl, int att, double time)
    {
        var s = GetLevelData(lvl) ?? new LevelStat { LevelID = lvl };
        s.TotalAttempts += att;
        s.TotalTime += time;
        _connection.InsertOrReplace(s);
    }
}

public class UserSettings
{
    [SQLite4Unity3d.PrimaryKey]
    public int Id { get; set; }
    public int SelectedSkinID { get; set; }
    public int SelectedColorIndex { get; set; }
    public int SelectedTrailColorIndex { get; set; }
}

public class UnlockedItem
{
    [SQLite4Unity3d.PrimaryKey, SQLite4Unity3d.AutoIncrement]
    public int Id { get; set; }
    public string ItemCategory { get; set; } 
    public int ItemID { get; set; }          
}