namespace OBD.Mobile.Lib.Models;

public class Sector
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
