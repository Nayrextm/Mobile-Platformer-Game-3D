
//using UnityEngine;
//using SQLite4Unity3d;
//using System.IO;
//using System.Linq;

//public class DatabaseManager : MonoBehaviour
//{
//    public static DatabaseManager Instance { get; private set; }
//    private SQLiteConnection _connection;
//    private string dbName = "GeoDashStats.db";

//    void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);
//            InitializeDatabase();
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    private void InitializeDatabase()
//    {
//        // ---> ВИПРАВЛЕННЯ ТУТ <---
//        string dbPath = string.Empty;

//        if (Application.platform == RuntimePlatform.Android)
//        {
//            // На Android шлях виглядає інакше і має бути в persistentDataPath
//            dbPath = Path.Combine(Application.persistentDataPath, dbName);
//        }
//        else
//        {
//            // На Windows/Editor можна залишати в Assets або теж кидати в persistentDataPath
//            // Краще використовувати persistentDataPath завжди, щоб симулювати реальну поведінку
//            dbPath = Path.Combine(Application.persistentDataPath, dbName);
//        }

//        // Лог для перевірки, куди воно зберігає (знайдеш цей шлях у консолі)
//        Debug.Log($"📂 Шлях до бази даних: {dbPath}");

//        // Відкриваємо з'єднання
//        _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

//        // Створення таблиць (якщо їх немає)
//        _connection.CreateTable<LevelStat>();
//        _connection.CreateTable<CollectiblesStat>();
//        _connection.CreateTable<CoinState>();

//        // Створення гаманця (ініціалізація)
//        if (_connection.Table<CollectiblesStat>().Count() == 0)
//        {
//            _connection.Insert(new CollectiblesStat { Id = 1, TotalCoins = 0 });
//        }
//    }

//    // --- STAR COINS (АЛМАЗИ) ---
//    public void SaveStarCoins(string levelName, bool[] coinsCollectedInRun)
//    {
//        var stat = _connection.Table<LevelStat>().Where(x => x.LevelID == levelName).FirstOrDefault();
//        if (stat == null) stat = new LevelStat { LevelID = levelName };

//        // Зберігаємо тільки true (якщо вже зібрали раніше, не перезаписуємо на false)
//        if (coinsCollectedInRun[0]) stat.StarCoin1 = true;
//        if (coinsCollectedInRun[1]) stat.StarCoin2 = true;
//        if (coinsCollectedInRun[2]) stat.StarCoin3 = true;

//        _connection.InsertOrReplace(stat);
//    }

//    public bool IsStarCoinCollected(string levelName, int coinIndex)
//    {
//        var stat = _connection.Table<LevelStat>().Where(x => x.LevelID == levelName).FirstOrDefault();
//        if (stat == null) return false;

//        switch (coinIndex)
//        {
//            case 0: return stat.StarCoin1;
//            case 1: return stat.StarCoin2;
//            case 2: return stat.StarCoin3;
//            default: return false;
//        }
//    }

//    // --- ЗВИЧАЙНІ МОНЕТИ (ГАМАНЕЦЬ) ---
//    public void AddCoins(int amount)
//    {
//        var stat = _connection.Table<CollectiblesStat>().Where(x => x.Id == 1).FirstOrDefault();
//        if (stat != null)
//        {
//            stat.TotalCoins += amount;
//            _connection.Update(stat);
//        }
//    }

//    public int GetTotalCoins()
//    {
//        var stat = _connection.Table<CollectiblesStat>().Where(x => x.Id == 1).FirstOrDefault();
//        return stat != null ? stat.TotalCoins : 0;
//    }

//    // --- ПАМ'ЯТЬ КОНКРЕТНИХ МОНЕТ ---
//    public bool IsCoinCollected(string coinID)
//    {
//        var existing = _connection.Table<CoinState>().Where(x => x.UniqueID == coinID).FirstOrDefault();
//        return existing != null;
//    }

//    public void MarkCoinAsCollected(string coinID)
//    {
//        _connection.InsertOrReplace(new CoinState { UniqueID = coinID });
//    }

//    // --- ПРОГРЕС РІВНЯ ---
//    public void SaveProgress(string levelName, int newAttempts, float timeDelta)
//    {
//        var record = _connection.Table<LevelStat>().Where(x => x.LevelID == levelName).FirstOrDefault();
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

//    public LevelStat GetLevelData(string levelName)
//    {
//        return _connection.Table<LevelStat>().Where(x => x.LevelID == levelName).FirstOrDefault();
//    }

//    // --- ВИДАЛЕННЯ ---
//    public void DeleteAllData()
//    {
//        _connection.DeleteAll<LevelStat>();
//        _connection.DeleteAll<CoinState>();

//        var stat = _connection.Table<CollectiblesStat>().Where(x => x.Id == 1).FirstOrDefault();
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
//        var allLevels = _connection.Table<LevelStat>().ToList();
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
//        var stat = _connection.Table<LevelStat>().Where(x => x.LevelID == levelName).FirstOrDefault();
//        if (stat == null)
//        {
//            stat = new LevelStat { LevelID = levelName, IsCompleted = true };
//            _connection.Insert(stat);
//        }
//        else
//        {
//            stat.IsCompleted = true;
//            _connection.Update(stat);
//        }
//    }

//    public bool IsLevelCompleted(string levelName)
//    {
//        var stat = _connection.Table<LevelStat>().Where(x => x.LevelID == levelName).FirstOrDefault();
//        return stat != null && stat.IsCompleted;
//    }
//}
using UnityEngine;
using SQLite4Unity3d;
using System.IO;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }
    private SQLiteConnection _connection;
    private readonly string _dbName = "GeoDashStats.db";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Application.targetFrameRate = 60;

            InitializeDatabase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeDatabase()
    {
        string dbPath = string.Empty;

#if UNITY_EDITOR
        
        string folderPath = Path.Combine(Application.dataPath, "Database");

       
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        dbPath = Path.Combine(folderPath, _dbName);
#else
        
        dbPath = Path.Combine(Application.persistentDataPath, _dbName);
#endif

        Debug.Log($"Шлях до бази даних: {dbPath}");

        _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

        _connection.CreateTable<LevelStat>();
        _connection.CreateTable<CollectiblesStat>();
        _connection.CreateTable<CoinState>();

        var wallet = GetWalletStat();
        if (wallet == null)
        {
            _connection.Insert(new CollectiblesStat { Id = 1, TotalCoins = 0 });
        }
    }

    private CollectiblesStat GetWalletStat()
    {
        return _connection.Table<CollectiblesStat>().Where(x => x.Id == 1).FirstOrDefault();
    }

    public LevelStat GetLevelData(string levelName)
    {
        return _connection.Find<LevelStat>(levelName);
    }

    public void SaveStarCoins(string levelName, bool[] coinsCollectedInRun)
    {
        var stat = GetLevelData(levelName);

        bool isNewRecord = false;
        if (stat == null)
        {
            stat = new LevelStat { LevelID = levelName };
            isNewRecord = true;
        }

        bool needsUpdate = false;

        if (coinsCollectedInRun[0] && !stat.StarCoin1) { stat.StarCoin1 = true; needsUpdate = true; }
        if (coinsCollectedInRun[1] && !stat.StarCoin2) { stat.StarCoin2 = true; needsUpdate = true; }
        if (coinsCollectedInRun[2] && !stat.StarCoin3) { stat.StarCoin3 = true; needsUpdate = true; }

        if (isNewRecord)
        {
            _connection.Insert(stat);
        }
        else if (needsUpdate)
        {
            _connection.Update(stat);
        }
    }

    public bool IsStarCoinCollected(string levelName, int coinIndex)
    {
        var stat = GetLevelData(levelName);
        if (stat == null) return false;

        return coinIndex switch
        {
            0 => stat.StarCoin1,
            1 => stat.StarCoin2,
            2 => stat.StarCoin3,
            _ => false,
        };
    }

    public void AddCoins(int amount)
    {
        var stat = GetWalletStat();
        if (stat != null)
        {
            stat.TotalCoins += amount;
            _connection.Update(stat);
        }
    }

    public int GetTotalCoins()
    {
        var stat = GetWalletStat();
        return stat != null ? stat.TotalCoins : 0;
    }

    public bool IsCoinCollected(string coinID)
    {
        return _connection.Find<CoinState>(coinID) != null;
    }

    public void MarkCoinAsCollected(string coinID)
    {
        _connection.InsertOrReplace(new CoinState { UniqueID = coinID });
    }

    public void SaveProgress(string levelName, int newAttempts, float timeDelta)
    {
        var record = GetLevelData(levelName);
        if (record != null)
        {
            record.TotalAttempts += newAttempts;
            record.TotalTime += timeDelta;
            _connection.Update(record);
        }
        else
        {
            var newRecord = new LevelStat
            {
                LevelID = levelName,
                TotalAttempts = newAttempts,
                TotalTime = timeDelta
            };
            _connection.Insert(newRecord);
        }
    }

    public void DeleteAllData()
    {
        _connection.DeleteAll<LevelStat>();
        _connection.DeleteAll<CoinState>();

        var stat = GetWalletStat();
        if (stat != null)
        {
            stat.TotalCoins = 0;
            _connection.Update(stat);
        }
        Debug.Log("Всі дані видалено!");
    }

    public int GetTotalCollectedStarCoins()
    {
        int totalCount = 0;
        var allLevels = _connection.Table<LevelStat>();

        foreach (var level in allLevels)
        {
            if (level.StarCoin1) totalCount++;
            if (level.StarCoin2) totalCount++;
            if (level.StarCoin3) totalCount++;
        }
        return totalCount;
    }

    public void MarkLevelComplete(string levelName)
    {
        var stat = GetLevelData(levelName);
        if (stat == null)
        {
            stat = new LevelStat { LevelID = levelName, IsCompleted = true };
            _connection.Insert(stat);
        }
        else if (!stat.IsCompleted)
        {
            stat.IsCompleted = true;
            _connection.Update(stat);
        }
    }

    public bool IsLevelCompleted(string levelName)
    {
        var stat = GetLevelData(levelName);
        return stat != null && stat.IsCompleted;
    }
}