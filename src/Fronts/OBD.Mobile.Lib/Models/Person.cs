namespace OBD.Mobile.Lib.Models;

public class Person
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public int SectorId { get; set; }
    public string Memo { get; set; } = string.Empty;

    [Ignore]
    public string SectorName { get; set; } = string.Empty;

    [Ignore]
    public string SectorAndPosition =>
        string.IsNullOrWhiteSpace(SectorName)
            ? Position
            : string.IsNullOrWhiteSpace(Position)
                ? SectorName
                : $"{SectorName} - {Position}";
}
