using SQLite4Unity3d;

public class CollectiblesStat
{
    [PrimaryKey]
    public int Id { get; set; } // Завжди 1 для локального гравця
    public int TotalCoins { get; set; }

    // У майбутньому сюди можна дописати:
    // public int TotalGems { get; set; }
}