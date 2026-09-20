using SQLite4Unity3d;

public class CollectiblesStat
{
    [PrimaryKey]
    public int Id { get; set; } 
    public int TotalCoins { get; set; }
}