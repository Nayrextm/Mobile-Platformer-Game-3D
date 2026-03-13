
//using SQLite4Unity3d;

//public class LevelStat
//{
//    [PrimaryKey]
//    public string LevelID { get; set; }

//    public int TotalAttempts { get; set; }
//    public float TotalTime { get; set; }

//    // Алмази
//    public bool StarCoin1 { get; set; }
//    public bool StarCoin2 { get; set; }
//    public bool StarCoin3 { get; set; }

//    // ---> НОВЕ ПОЛЕ <---
//    public bool IsCompleted { get; set; }

//    public override string ToString()
//    {
//        return string.Format("[Level: {0}, Completed: {1}]", LevelID, IsCompleted);
//    }
//}
using SQLite4Unity3d;

public class LevelStat
{
    [PrimaryKey]
    public string LevelID { get; set; }

    public int TotalAttempts { get; set; }
    public float TotalTime { get; set; }

    public bool StarCoin1 { get; set; }
    public bool StarCoin2 { get; set; }
    public bool StarCoin3 { get; set; }

    public bool IsCompleted { get; set; }

    public override string ToString()
    {
        return $"[Level: {LevelID}, Completed: {IsCompleted}]";
    }
}