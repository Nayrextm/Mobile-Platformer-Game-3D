
//using UnityEngine;
//using SQLite4Unity3d;
//using System.IO;

//public class DatabaseManager : MonoBehaviour
//{
//    public static DatabaseManager Instance { get; private set; }
//    private SQLiteConnection _connection;
//    private readonly string _dbName = "GeoDashStats.db";

//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);

//            Application.targetFrameRate = 60;

//            InitializeDatabase();
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    private void InitializeDatabase()
//    {
//        string dbPath = string.Empty;

//#if UNITY_EDITOR

//        string folderPath = Path.Combine(Application.dataPath, "Database");


//        if (!Directory.Exists(folderPath))
//        {
//            Directory.CreateDirectory(folderPath);
//        }
//        dbPath = Path.Combine(folderPath, _dbName);
//#else

//        dbPath = Path.Combine(Application.persistentDataPath, _dbName);
//#endif

//        Debug.Log($"Шлях до бази даних: {dbPath}");

//        _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

//        _connection.CreateTable<LevelStat>();
//        _connection.CreateTable<CollectiblesStat>();
//        _connection.CreateTable<CoinState>();

//        var wallet = GetWalletStat();
//        if (wallet == null)
//        {
//            _connection.Insert(new CollectiblesStat { Id = 1, TotalCoins = 0 });
//        }
//    }

//    private CollectiblesStat GetWalletStat()
//    {
//        return _connection.Table<CollectiblesStat>().Where(x => x.Id == 1).FirstOrDefault();
//    }

//    public LevelStat GetLevelData(string levelName)
//    {
//        return _connection.Find<LevelStat>(levelName);
//    }

//    public void SaveStarCoins(string levelName, bool[] coinsCollectedInRun)
//    {
//        var stat = GetLevelData(levelName);

//        bool isNewRecord = false;
//        if (stat == null)
//        {
//            stat = new LevelStat { LevelID = levelName };
//            isNewRecord = true;
//        }

//        bool needsUpdate = false;

//        if (coinsCollectedInRun[0] && !stat.StarCoin1) { stat.StarCoin1 = true; needsUpdate = true; }
//        if (coinsCollectedInRun[1] && !stat.StarCoin2) { stat.StarCoin2 = true; needsUpdate = true; }
//        if (coinsCollectedInRun[2] && !stat.StarCoin3) { stat.StarCoin3 = true; needsUpdate = true; }

//        if (isNewRecord)
//        {
//            _connection.Insert(stat);
//        }
//        else if (needsUpdate)
//        {
//            _connection.Update(stat);
//        }
//    }

//    public bool IsStarCoinCollected(string levelName, int coinIndex)
//    {
//        var stat = GetLevelData(levelName);
//        if (stat == null) return false;

//        return coinIndex switch
//        {
//            0 => stat.StarCoin1,
//            1 => stat.StarCoin2,
//            2 => stat.StarCoin3,
//            _ => false,
//        };
//    }

//    public void AddCoins(int amount)
//    {
//        var stat = GetWalletStat();
//        if (stat != null)
//        {
//            stat.TotalCoins += amount;
//            _connection.Update(stat);
//        }
//    }

//    public int GetTotalCoins()
//    {
//        var stat = GetWalletStat();
//        return stat != null ? stat.TotalCoins : 0;
//    }

//    public bool IsCoinCollected(string coinID)
//    {
//        return _connection.Find<CoinState>(coinID) != null;
//    }

//    public void MarkCoinAsCollected(string coinID)
//    {
//        _connection.InsertOrReplace(new CoinState { UniqueID = coinID });
//    }

//    public void SaveProgress(string levelName, int newAttempts, float timeDelta)
//    {
//        var record = GetLevelData(levelName);
//        if (record != null)
//        {
//            record.TotalAttempts += newAttempts;
//            record.TotalTime += timeDelta;
//            _connection.Update(record);
//        }
//        else
//        {
//            var newRecord = new LevelStat
//            {
//                LevelID = levelName,
//                TotalAttempts = newAttempts,
//                TotalTime = timeDelta
//            };
//            _connection.Insert(newRecord);
//        }
//    }

//    public void DeleteAllData()
//    {
//        _connection.DeleteAll<LevelStat>();
//        _connection.DeleteAll<CoinState>();

//        var stat = GetWalletStat();
//        if (stat != null)
//        {
//            stat.TotalCoins = 0;
//            _connection.Update(stat);
//        }
//        Debug.Log("Всі дані видалено!");
//    }

//    public int GetTotalCollectedStarCoins()
//    {
//        int totalCount = 0;
//        var allLevels = _connection.Table<LevelStat>();

//        foreach (var level in allLevels)
//        {
//            if (level.StarCoin1) totalCount++;
//            if (level.StarCoin2) totalCount++;
//            if (level.StarCoin3) totalCount++;
//        }
//        return totalCount;
//    }

//    public void MarkLevelComplete(string levelName)
//    {
//        var stat = GetLevelData(levelName);
//        if (stat == null)
//        {
//            stat = new LevelStat { LevelID = levelName, IsCompleted = true };
//            _connection.Insert(stat);
//        }
//        else if (!stat.IsCompleted)
//        {
//            stat.IsCompleted = true;
//            _connection.Update(stat);
//        }
//    }

//    public bool IsLevelCompleted(string levelName)
//    {
//        var stat = GetLevelData(levelName);
//        return stat != null && stat.IsCompleted;
//    }
//}

//Поганий варіант по оптимізації нижче! 

//using UnityEngine;
//using SQLite4Unity3d;
//using System.IO;
//using System.Collections.Generic;

//public class DatabaseManager : MonoBehaviour
//{
//    public static DatabaseManager Instance { get; private set; }
//    private SQLiteConnection _connection;
//    private readonly string _dbName = "GeoDashStats.db";

//    private HashSet<int> _collectedCoinsCache = new HashSet<int>();

//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);


//            Application.targetFrameRate = 60;

//            InitializeDatabase();
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    private void InitializeDatabase()
//    {
//        string dbPath = string.Empty;

//#if UNITY_EDITOR
//        string folderPath = Path.Combine(Application.dataPath, "Database");
//        if (!Directory.Exists(folderPath))
//        {
//            Directory.CreateDirectory(folderPath);
//        }
//        dbPath = Path.Combine(folderPath, _dbName);
//#else
//        dbPath = Path.Combine(Application.persistentDataPath, _dbName);
//#endif

//        Debug.Log($"Шлях до бази даних: {dbPath}");

//        _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);


//        _connection.CreateTable<LevelStat>();
//        _connection.CreateTable<CollectiblesStat>();
//        _connection.CreateTable<CoinState>();


//        var wallet = GetWalletStat();
//        if (wallet == null)
//        {
//            _connection.Insert(new CollectiblesStat { Id = 1, TotalCoins = 0 });
//        }


//        LoadCoinsIntoCache();
//    }


//    private void LoadCoinsIntoCache()
//    {
//        _collectedCoinsCache.Clear();
//        var allCollectedCoins = _connection.Table<CoinState>();
//        foreach (var coin in allCollectedCoins)
//        {
//            _collectedCoinsCache.Add(coin.UniqueID);
//        }
//    }

//    public bool IsCoinAlreadyCollectedInDB(int coinID)
//    {

//        return _collectedCoinsCache.Contains(coinID);
//    }

//    public void CollectCoinImmediate(int coinID, int coinValue)
//    {
//        if (!_collectedCoinsCache.Contains(coinID))
//        {

//            _collectedCoinsCache.Add(coinID);


//            _connection.InsertOrReplace(new CoinState { UniqueID = coinID });


//            AddCoins(coinValue);
//        }
//    }


//    private CollectiblesStat GetWalletStat()
//    {
//        return _connection.Table<CollectiblesStat>().Where(x => x.Id == 1).FirstOrDefault();
//    }

//    public void AddCoins(int amount)
//    {
//        var stat = GetWalletStat();
//        if (stat != null)
//        {
//            stat.TotalCoins += amount;
//            _connection.Update(stat);
//        }
//    }

//    public int GetTotalCoins()
//    {
//        var stat = GetWalletStat();
//        return stat != null ? stat.TotalCoins : 0;
//    }


//    public LevelStat GetLevelData(string levelName)
//    {
//        return _connection.Find<LevelStat>(levelName);
//    }

//    public void SaveStarCoins(string levelName, bool[] coinsCollectedInRun)
//    {
//        var stat = GetLevelData(levelName);
//        bool isNewRecord = false;

//        if (stat == null)
//        {
//            stat = new LevelStat { LevelID = levelName };
//            isNewRecord = true;
//        }

//        bool needsUpdate = false;

//        if (coinsCollectedInRun[0] && !stat.StarCoin1) { stat.StarCoin1 = true; needsUpdate = true; }
//        if (coinsCollectedInRun[1] && !stat.StarCoin2) { stat.StarCoin2 = true; needsUpdate = true; }
//        if (coinsCollectedInRun[2] && !stat.StarCoin3) { stat.StarCoin3 = true; needsUpdate = true; }

//        if (isNewRecord)
//        {
//            _connection.Insert(stat);
//        }
//        else if (needsUpdate)
//        {
//            _connection.Update(stat);
//        }
//    }

//    public bool IsStarCoinCollected(string levelName, int coinIndex)
//    {
//        var stat = GetLevelData(levelName);
//        if (stat == null) return false;

//        return coinIndex switch
//        {
//            0 => stat.StarCoin1,
//            1 => stat.StarCoin2,
//            2 => stat.StarCoin3,
//            _ => false,
//        };
//    }

//    public int GetTotalCollectedStarCoins()
//    {
//        int totalCount = 0;
//        var allLevels = _connection.Table<LevelStat>();

//        foreach (var level in allLevels)
//        {
//            if (level.StarCoin1) totalCount++;
//            if (level.StarCoin2) totalCount++;
//            if (level.StarCoin3) totalCount++;
//        }
//        return totalCount;
//    }


//    public void SaveProgress(string levelName, int newAttempts, float timeDelta)
//    {
//        var record = GetLevelData(levelName);
//        if (record != null)
//        {
//            record.TotalAttempts += newAttempts;
//            record.TotalTime += timeDelta;
//            _connection.Update(record);
//        }
//        else
//        {
//            var newRecord = new LevelStat
//            {
//                LevelID = levelName,
//                TotalAttempts = newAttempts,
//                TotalTime = timeDelta
//            };
//            _connection.Insert(newRecord);
//        }
//    }

//    public void MarkLevelComplete(string levelName)
//    {
//        var stat = GetLevelData(levelName);
//        if (stat == null)
//        {
//            stat = new LevelStat { LevelID = levelName, IsCompleted = true };
//            _connection.Insert(stat);
//        }
//        else if (!stat.IsCompleted)
//        {
//            stat.IsCompleted = true;
//            _connection.Update(stat);
//        }
//    }

//    public bool IsLevelCompleted(string levelName)
//    {
//        var stat = GetLevelData(levelName);
//        return stat != null && stat.IsCompleted;
//    }


//    public void DeleteAllData()
//    {
//        _connection.DeleteAll<LevelStat>();
//        _connection.DeleteAll<CoinState>();

//        var stat = GetWalletStat();
//        if (stat != null)
//        {
//            stat.TotalCoins = 0;
//            _connection.Update(stat);
//        }

//        _collectedCoinsCache.Clear(); 
//        Debug.Log("Всі дані видалено!");
//    }
//}
using UnityEngine;
using SQLite4Unity3d;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }
    private SQLiteConnection _connection;
    private readonly string _dbName = "GeoDashStats.db";


    //private HashSet<int> _collectedCoinsCache = new HashSet<int>();
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
            InitializeDatabase();
        }
        else { Destroy(gameObject); }
    }

    private void InitializeDatabase()
    {
        string dbPath = "";
#if UNITY_EDITOR
        string folderPath = Path.Combine(Application.dataPath, "Database");
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
        dbPath = Path.Combine(folderPath, _dbName);
#else
        dbPath = Path.Combine(Application.persistentDataPath, _dbName);
#endif
        _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        _connection.CreateTable<LevelStat>();
        _connection.CreateTable<CollectiblesStat>();
        _connection.CreateTable<CoinState>();

        _connection.CreateTable<UserSettings>();

        _connection.CreateTable<UnlockedItem>();

        LoadDataToCache();
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



    //5.0 update - public bool IsCoinAlreadyCollectedInDB(int id) => _collectedCoinsCache.Contains(id);

    //public void CollectCoinImmediate(int id, int value)
    //{
    //    if (!_collectedCoinsCache.Contains(id))
    //    {
    //        _collectedCoinsCache.Add(id);
    //        _coinsToSaveToDisk.Add(new CoinState { UniqueID = id });
    //        _cachedWallet.TotalCoins += value;


    //        if (_coinsToSaveToDisk.Count >= 4) SaveAllPendingDataToDisk();
    //    }
    //}
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

    //Старий 
    //public int GetTotalCollectedStarCoins()
    //{
    //    int count = 0;
    //    foreach (var l in _connection.Table<LevelStat>())
    //    {
    //        if (l.StarCoin1) count++;
    //        if (l.StarCoin2) count++;
    //        if (l.StarCoin3) count++;
    //    }
    //    return count;
    //}
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
    //Старий 
    //public void SaveProgress(string lvl, int att, float time)
    //{
    //    var s = GetLevelData(lvl) ?? new LevelStat { LevelID = lvl };
    //    s.TotalAttempts += att;
    //    s.TotalTime += time;
    //    _connection.InsertOrReplace(s);
    //}

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
    public string ItemCategory { get; set; } // Наприклад: "TrailColor", "Skin", "PlayerColor"
    public int ItemID { get; set; }          // Індекс або ID купленого предмета
}