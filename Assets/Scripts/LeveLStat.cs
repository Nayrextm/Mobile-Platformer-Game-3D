using SQLite4Unity3d;

public class LevelStat
{
    [PrimaryKey]
    public string LevelID { get; set; }

    public int TotalAttempts { get; set; }
    public double TotalTime { get; set; }

    public bool StarCoin1 { get; set; }
    public bool StarCoin2 { get; set; }
    public bool StarCoin3 { get; set; }

    public bool IsCompleted { get; set; }

    public override string ToString()
    {
        return $"[Level: {LevelID}, Completed: {IsCompleted}]";
    }
}