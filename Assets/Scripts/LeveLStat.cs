//using SQLite4Unity3d; // Підключаємо бібліотеку

//public class LevelStat
//{
//    // [PrimaryKey] означає, що це головний ідентифікатор. 
//    // Ми не можемо мати два записи з однаковим LevelID.
//    [PrimaryKey]
//    public string LevelID { get; set; }

//    public int TotalAttempts { get; set; }
//    public float TotalTime { get; set; }

//    // Цей метод потрібен просто щоб красиво виводити інфу в консоль (для тестів)
//    public override string ToString()
//    {
//        return string.Format("[Рівень: {0} | Спроб: {1} | Час: {2} с]", LevelID, TotalAttempts, TotalTime);
//    }
//}
//using SQLite4Unity3d;

//public class LevelStat
//{
//    [PrimaryKey]
//    public string LevelID { get; set; }

//    public int TotalAttempts { get; set; }
//    public float TotalTime { get; set; }

//    // ---> НОВІ ПОЛЯ ДЛЯ АЛМАЗІВ <---
//    // True = зібрано, False = не зібрано
//    public bool StarCoin1 { get; set; }
//    public bool StarCoin2 { get; set; }
//    public bool StarCoin3 { get; set; }

//    public override string ToString()
//    {
//        // Оновимо вивід в консоль, щоб бачити стан монет (+ або -)
//        return string.Format("[Рівень: {0} | Спроб: {1} | Алмази: {2}-{3}-{4}]",
//            LevelID,
//            TotalAttempts,
//            StarCoin1 ? "+" : "-",
//            StarCoin2 ? "+" : "-",
//            StarCoin3 ? "+" : "-");
//    }
//}
using SQLite4Unity3d;

public class LevelStat
{
    [PrimaryKey]
    public string LevelID { get; set; }

    public int TotalAttempts { get; set; }
    public float TotalTime { get; set; }

    // Алмази
    public bool StarCoin1 { get; set; }
    public bool StarCoin2 { get; set; }
    public bool StarCoin3 { get; set; }

    // ---> НОВЕ ПОЛЕ <---
    public bool IsCompleted { get; set; }

    public override string ToString()
    {
        return string.Format("[Level: {0}, Completed: {1}]", LevelID, IsCompleted);
    }
}