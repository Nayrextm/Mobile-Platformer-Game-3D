using SQLite4Unity3d;

public class CoinState
{
    // Унікальний ID монети (Наприклад: "Level1_12.5_3.0_1.0")
    [PrimaryKey]
    public string UniqueID { get; set; }
}