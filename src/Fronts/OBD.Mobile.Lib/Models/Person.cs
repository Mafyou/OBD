namespace OBD.Mobile.Lib.Models;

public class Person
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public int SectorId { get; set; }
    public string Memo { get; set; } = string.Empty;
}
