//using UnityEngine;
//using SQLite4Unity3d;
//using System.IO;

//public class DatabaseManager : MonoBehaviour
//{
//    // Робимо Singleton, щоб мати доступ до бази з будь-якого скрипта
//    public static DatabaseManager Instance;

//    private SQLiteConnection _connection;
//    private string dbName = "GeoDashCoursework.db"; // Назва вашого файлу


//    void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject); // <--- ВАЖЛИВО! Це робить об'єкт "безсмертним" між сценами

//            // Далі твій код підключення...
//            string dbPath = Path.Combine(Application.persistentDataPath, dbName);
//            _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
//            _connection.CreateTable<LevelStat>();
//            Debug.Log("DB Connected: " + dbPath);
//        }
//        else
//        {
//            // Якщо такий менеджер вже прийшов з попередньої сцени - знищуємо дублікат
//            Destroy(gameObject);
//        }
//    }
//    //void Awake()
//    //{
//    //    // Налаштування Singleton
//    //    if (Instance == null) Instance = this;
//    //    else Destroy(gameObject);

//    //    // 1. Визначаємо шлях. persistentDataPath - це папка, куди Unity дозволяє писати файли.
//    //    string dbPath = Path.Combine(Application.persistentDataPath, dbName);

//    //    // 2. Створюємо підключення
//    //    _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

//    //    // 3. Створюємо таблицю (якщо вона вже є, цей рядок нічого не зіпсує)
//    //    _connection.CreateTable<LevelStat>();

//    //    Debug.Log("База даних активна: " + dbPath);
//    //}

//    // Головна функція: додати смерть і час
//    public void SaveProgress(string levelName, float timeSpent)
//    {
//        // Шукаємо, чи є вже такий рівень в базі
//        var existingRecord = _connection.Table<LevelStat>()
//                            .Where(x => x.LevelID == levelName)
//                            .FirstOrDefault();

//        if (existingRecord != null)
//        {
//            // Рівень вже був -> Оновлюємо дані
//            existingRecord.TotalAttempts += 1;      // +1 спроба
//            existingRecord.TotalTime += timeSpent;  // додаємо час

//            _connection.Update(existingRecord);     // Зберігаємо зміни
//            Debug.Log($"Оновлено: {existingRecord.LevelID}, Всього спроб: {existingRecord.TotalAttempts}");
//        }
//        else
//        {
//            // Рівень новий -> Створюємо запис
//            var newRecord = new LevelStat
//            {
//                LevelID = levelName,
//                TotalAttempts = 1,
//                TotalTime = timeSpent
//            };

//            _connection.Insert(newRecord);          // Вставляємо новий рядок
//            Debug.Log($"Створено новий запис для {levelName}");
//        }
//    }

//    // (Необов'язково) Отримати кількість спроб для відображення в меню
//    public int GetAttemptsCount(string levelName)
//    {
//        var record = _connection.Table<LevelStat>().Where(x => x.LevelID == levelName).FirstOrDefault();
//        return record != null ? record.TotalAttempts : 0;
//    }

//    // Додай цей метод в DatabaseManager.cs
//    public LevelStat GetLevelData(string levelName)
//    {
//        // Шукаємо запис в базі за назвою рівня
//        return _connection.Table<LevelStat>()
//                          .Where(x => x.LevelID == levelName)
//                          .FirstOrDefault();
//    }
//}
//using UnityEngine;
//using SQLite4Unity3d;
//using System.IO;
//using System.Linq;

//public class DatabaseManager : MonoBehaviour
//{
//    public static DatabaseManager Instance;
//    private SQLiteConnection _connection;
//    private string dbName = "GeoDashStats.db";

//    void Awake()
//    {
//        // Паттерн Singleton + DontDestroyOnLoad
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject); // Живе між сценами

//            string dbPath = Path.Combine(Application.persistentDataPath, dbName);
//            _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
//            _connection.CreateTable<LevelStat>();

//            Debug.Log("База даних підключена: " + dbPath);
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    // Додає "шматочок" часу і +1 спробу до глобальної статистики
//    public void SaveProgress(string levelName, float timeDelta)
//    {
//        var record = _connection.Table<LevelStat>().Where(x => x.LevelID == levelName).FirstOrDefault();

//        if (record != null)
//        {
//            record.TotalAttempts += 1;
//            record.TotalTime += timeDelta;
//            _connection.Update(record);
//        }
//        else
//        {
//            var newRecord = new LevelStat
//            {
//                LevelID = levelName,
//                TotalAttempts = 1,
//                TotalTime = timeDelta
//            };
//            _connection.Insert(newRecord);
//        }
//    }

//    // Отримати статистику (для меню)
//    public LevelStat GetLevelData(string levelName)
//    {
//        return _connection.Table<LevelStat>().Where(x => x.LevelID == levelName).FirstOrDefault();
//    }

//    // Додай це в DatabaseManager.cs
//    public void DeleteAllData()
//    {
//        // Це вбудована команда бібліотеки: видаляє всі рядки з таблиці LevelStat
//        _connection.DeleteAll<LevelStat>();

//        Debug.Log("Базу даних повністю очищено!");
//    }
//}
//using UnityEngine;
//using SQLite4Unity3d;
//using System.IO;
//using System.Linq;

//public class DatabaseManager : MonoBehaviour
//{
//    public static DatabaseManager Instance { get; private set; }
//    private SQLiteConnection _connection;
//    // Ім'я файлу бази даних
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

//    //private void InitializeDatabase()
//    //{
//    //    string dbPath = Path.Combine(Application.persistentDataPath, dbName);
//    //    _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
//    //    _connection.CreateTable<LevelStat>();
//    //    Debug.Log($"База даних підключена: {dbPath}");
//    //}
//    private void InitializeDatabase()
//    {
//        // ВИДАЛЕНО: Старий шлях, який веде в AppData користувача
//        // string dbPath = Path.Combine(Application.persistentDataPath, dbName);

//        // ДОДАНО: Новий шлях, який веде в папку з грою (Data поруч з .exe)
//        // Це дозволить носити базу разом з грою на флешці
//        string dbPath = Path.Combine(Application.dataPath, dbName);

//        _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
//        _connection.CreateTable<LevelStat>();

//        Debug.Log($"База даних (Portable) підключена за шляхом: {dbPath}");
//    }

//    // ---> ОСЬ ТУТ БУЛА ПРОБЛЕМА: Тепер метод приймає 3 аргументи
//    // newAttempts - скільки спроб додати (1 при смерті, 0 при перемозі)
//    public void SaveProgress(string levelName, int newAttempts, float timeDelta)
//    {
//        // Шукаємо, чи є вже запис про цей рівень
//        var record = _connection.Table<LevelStat>()
//            .Where(x => x.LevelID == levelName)
//            .FirstOrDefault();

//        if (record != null)
//        {
//            // Оновлюємо: додаємо нові спроби і час до старих
//            record.TotalAttempts += newAttempts;
//            record.TotalTime += timeDelta;
//            _connection.Update(record);
//        }
//        else
//        {
//            // Створюємо новий запис
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

//    public void DeleteAllData()
//    {
//        _connection.DeleteAll<LevelStat>();
//        Debug.Log("Базу даних повністю очищено!");
//    }
//}
//using UnityEngine;
//using SQLite4Unity3d;
//using System.IO;
//using System.Linq;

//public class DatabaseManager : MonoBehaviour
//{
//    public static DatabaseManager Instance { get; private set; }
//    private SQLiteConnection _connection;
//    // Ім'я файлу бази даних
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

//    //private void InitializeDatabase()
//    //{
//    //    string dbPath = Path.Combine(Application.persistentDataPath, dbName);
//    //    _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
//    //    _connection.CreateTable<LevelStat>();
//    //    Debug.Log($"База даних підключена: {dbPath}");
//    //}
//    private void InitializeDatabase()
//    {
//        // ВИДАЛЕНО: Старий шлях, який веде в AppData користувача
//        // string dbPath = Path.Combine(Application.persistentDataPath, dbName);

//        // ДОДАНО: Новий шлях, який веде в папку з грою (Data поруч з .exe)
//        // Це дозволить носити базу разом з грою на флешці
//        string dbPath = Path.Combine(Application.dataPath, dbName);

//        _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
//        _connection.CreateTable<LevelStat>();

//        Debug.Log($"База даних (Portable) підключена за шляхом: {dbPath}");
//    }

//    // ---> ОСЬ ТУТ БУЛА ПРОБЛЕМА: Тепер метод приймає 3 аргументи
//    // newAttempts - скільки спроб додати (1 при смерті, 0 при перемозі)
//    public void SaveProgress(string levelName, int newAttempts, float timeDelta)
//    {
//        // Шукаємо, чи є вже запис про цей рівень
//        var record = _connection.Table<LevelStat>()
//            .Where(x => x.LevelID == levelName)
//            .FirstOrDefault();

//        if (record != null)
//        {
//            // Оновлюємо: додаємо нові спроби і час до старих
//            record.TotalAttempts += newAttempts;
//            record.TotalTime += timeDelta;
//            _connection.Update(record);
//        }
//        else
//        {
//            // Створюємо новий запис
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

//    public void DeleteAllData()
//    {
//        _connection.DeleteAll<LevelStat>();
//        Debug.Log("Базу даних повністю очищено!");
//    }
//}
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
//        string dbPath = Path.Combine(Application.dataPath, dbName);

//        _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

//        // 1. Таблиця рівнів
//        _connection.CreateTable<LevelStat>();

//        // 2. Таблиця валют і колекційних предметів (НОВА НАЗВА)
//        _connection.CreateTable<CollectiblesStat>();

//        // Створюємо гаманець, якщо його ще немає
//        if (_connection.Table<CollectiblesStat>().Count() == 0)
//        {
//            var newWallet = new CollectiblesStat
//            {
//                Id = 1,
//                TotalCoins = 0
//            };
//            _connection.Insert(newWallet);
//        }

//        Debug.Log($"База даних підключена: {dbPath}");
//    }

//    // --- Методи для ВАЛЮТИ (Collectibles) ---

//    public void AddCoins(int amount)
//    {
//        // Шукаємо запис у новій таблиці
//        var stat = _connection.Table<CollectiblesStat>().Where(x => x.Id == 1).FirstOrDefault();

//        if (stat != null)
//        {
//            stat.TotalCoins += amount;
//            _connection.Update(stat);
//            // Debug.Log($"Монет збережено: {stat.TotalCoins}");
//        }
//    }

//    public int GetTotalCoins()
//    {
//        var stat = _connection.Table<CollectiblesStat>().Where(x => x.Id == 1).FirstOrDefault();
//        return stat != null ? stat.TotalCoins : 0;
//    }

//    // --- Методи для РІВНІВ (Без змін) ---
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

//    // --- ЛОГІКА STAR COINS ---

//    // Зберігає монети (викликається тільки при перемозі)
//    public void SaveStarCoins(string levelName, bool[] coinsCollectedInRun)
//    {
//        // Знаходимо запис рівня або створюємо новий
//        var stat = _connection.Table<LevelStat>().Where(x => x.LevelID == levelName).FirstOrDefault();
//        if (stat == null) stat = new LevelStat { LevelID = levelName };

//        // Логіка "АБО": якщо монета вже була true, вона залишається true.
//        // Якщо ми її щойно зібрали (coinsCollectedInRun == true), то записуємо true.
//        if (coinsCollectedInRun[0]) stat.StarCoin1 = true;
//        if (coinsCollectedInRun[1]) stat.StarCoin2 = true;
//        if (coinsCollectedInRun[2]) stat.StarCoin3 = true;

//        _connection.InsertOrReplace(stat);
//    }

//    // Перевіряє статус конкретної монети (0, 1 або 2)
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

//    public LevelStat GetLevelData(string levelName)
//    {
//        return _connection.Table<LevelStat>().Where(x => x.LevelID == levelName).FirstOrDefault();
//    }

//    public void DeleteAllData()
//    {
//        _connection.DeleteAll<LevelStat>();

//        // Обнуляємо валюту в новій таблиці
//        var stat = _connection.Table<CollectiblesStat>().Where(x => x.Id == 1).FirstOrDefault();
//        if (stat != null)
//        {
//            stat.TotalCoins = 0;
//            _connection.Update(stat);
//        }

//        Debug.Log("Всі дані очищено!");
//    }
//}
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
//        string dbPath = Path.Combine(Application.dataPath, dbName);
//        _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

//        // Створення таблиць
//        _connection.CreateTable<LevelStat>();
//        _connection.CreateTable<CollectiblesStat>();

//        // Створення гаманця
//        if (_connection.Table<CollectiblesStat>().Count() == 0)
//        {
//            _connection.Insert(new CollectiblesStat { Id = 1, TotalCoins = 0 });
//        }
//        Debug.Log($"База даних підключена: {dbPath}");
//    }

//    // --- STAR COINS (АЛМАЗИ) ---
//    public void SaveStarCoins(string levelName, bool[] coinsCollectedInRun)
//    {
//        var stat = _connection.Table<LevelStat>().Where(x => x.LevelID == levelName).FirstOrDefault();
//        if (stat == null) stat = new LevelStat { LevelID = levelName };

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

//    // --- ЗВИЧАЙНІ МОНЕТИ ---
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

//    // --- ВИДАЛЕННЯ (RESET) ---
//    public void DeleteAllData()
//    {
//        // 1. Цей рядок видаляє ВСЕ про рівні: Час, Спроби, І АЛМАЗИ ТЕЖ
//        _connection.DeleteAll<LevelStat>();

//        // 2. Цей блок обнуляє звичайні монети
//        var stat = _connection.Table<CollectiblesStat>().Where(x => x.Id == 1).FirstOrDefault();
//        if (stat != null)
//        {
//            stat.TotalCoins = 0;
//            _connection.Update(stat);
//        }

//        Debug.Log("Всі дані (включно з алмазами) видалено!");
//    }
//}
using UnityEngine;
using SQLite4Unity3d;
using System.IO;
using System.Linq;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }
    private SQLiteConnection _connection;
    private string dbName = "GeoDashStats.db";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDatabase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeDatabase()
    {
        string dbPath = Path.Combine(Application.dataPath, dbName);
        _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

        // Створення таблиць
        _connection.CreateTable<LevelStat>();
        _connection.CreateTable<CollectiblesStat>();

        // ---> NEW: Створюємо таблицю для пам'яті окремих монет
        _connection.CreateTable<CoinState>();

        // Створення гаманця
        if (_connection.Table<CollectiblesStat>().Count() == 0)
        {
            _connection.Insert(new CollectiblesStat { Id = 1, TotalCoins = 0 });
        }
        Debug.Log($"База даних підключена: {dbPath}");
    }

    // --- STAR COINS (АЛМАЗИ) ---
    public void SaveStarCoins(string levelName, bool[] coinsCollectedInRun)
    {
        var stat = _connection.Table<LevelStat>().Where(x => x.LevelID == levelName).FirstOrDefault();
        if (stat == null) stat = new LevelStat { LevelID = levelName };

        if (coinsCollectedInRun[0]) stat.StarCoin1 = true;
        if (coinsCollectedInRun[1]) stat.StarCoin2 = true;
        if (coinsCollectedInRun[2]) stat.StarCoin3 = true;

        _connection.InsertOrReplace(stat);
    }

    public bool IsStarCoinCollected(string levelName, int coinIndex)
    {
        var stat = _connection.Table<LevelStat>().Where(x => x.LevelID == levelName).FirstOrDefault();
        if (stat == null) return false;

        switch (coinIndex)
        {
            case 0: return stat.StarCoin1;
            case 1: return stat.StarCoin2;
            case 2: return stat.StarCoin3;
            default: return false;
        }
    }

    // --- ЗВИЧАЙНІ МОНЕТИ (ГАМАНЕЦЬ) ---
    public void AddCoins(int amount)
    {
        var stat = _connection.Table<CollectiblesStat>().Where(x => x.Id == 1).FirstOrDefault();
        if (stat != null)
        {
            stat.TotalCoins += amount;
            _connection.Update(stat);
        }
    }

    public int GetTotalCoins()
    {
        var stat = _connection.Table<CollectiblesStat>().Where(x => x.Id == 1).FirstOrDefault();
        return stat != null ? stat.TotalCoins : 0;
    }

    // ---> NEW: ПАМ'ЯТЬ КОНКРЕТНИХ МОНЕТ <---

    // Перевірка: чи є паспорт цієї монети в базі?
    public bool IsCoinCollected(string coinID)
    {
        var existing = _connection.Table<CoinState>().Where(x => x.UniqueID == coinID).FirstOrDefault();
        return existing != null; // Поверне true, якщо знайшло запис
    }

    // Записати паспорт монети в базу
    public void MarkCoinAsCollected(string coinID)
    {
        _connection.InsertOrReplace(new CoinState { UniqueID = coinID });
    }

    // --- ПРОГРЕС РІВНЯ ---
    public void SaveProgress(string levelName, int newAttempts, float timeDelta)
    {
        var record = _connection.Table<LevelStat>().Where(x => x.LevelID == levelName).FirstOrDefault();
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

    public LevelStat GetLevelData(string levelName)
    {
        return _connection.Table<LevelStat>().Where(x => x.LevelID == levelName).FirstOrDefault();
    }

    // --- ВИДАЛЕННЯ (RESET) ---
    public void DeleteAllData()
    {
        // 1. Цей рядок видаляє ВСЕ про рівні: Час, Спроби, І АЛМАЗИ ТЕЖ
        _connection.DeleteAll<LevelStat>();

        // 2. ---> NEW: Видаляємо пам'ять про зібрані монети (тепер вони з'являться знову)
        _connection.DeleteAll<CoinState>();

        // 3. Цей блок обнуляє звичайні монети (гаманець)
        var stat = _connection.Table<CollectiblesStat>().Where(x => x.Id == 1).FirstOrDefault();
        if (stat != null)
        {
            stat.TotalCoins = 0;
            _connection.Update(stat);
        }

        Debug.Log("Всі дані (включно з алмазами та монетами) видалено!");
    }

    public int GetTotalCollectedStarCoins()
    {
        int totalCount = 0;

        // 1. Беремо список усіх рівнів з бази
        var allLevels = _connection.Table<LevelStat>().ToList();

        // 2. Проходимося по кожному і рахуємо галочки
        foreach (var level in allLevels)
        {
            if (level.StarCoin1) totalCount++;
            if (level.StarCoin2) totalCount++;
            if (level.StarCoin3) totalCount++;
        }

        return totalCount;
    }

    // ---> НОВІ МЕТОДИ <---

    // 1. Записати перемогу
    public void MarkLevelComplete(string levelName)
    {
        var stat = _connection.Table<LevelStat>().Where(x => x.LevelID == levelName).FirstOrDefault();

        if (stat == null)
        {
            stat = new LevelStat { LevelID = levelName, IsCompleted = true };
            _connection.Insert(stat);
        }
        else
        {
            stat.IsCompleted = true;
            _connection.Update(stat);
        }
    }

    // 2. Перевірити перемогу (для меню)
    public bool IsLevelCompleted(string levelName)
    {
        var stat = _connection.Table<LevelStat>().Where(x => x.LevelID == levelName).FirstOrDefault();
        return stat != null && stat.IsCompleted;
    }
}