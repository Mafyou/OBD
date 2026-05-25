namespace OBD.Mobile.Lib.Models;

public class ReperesTravail
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string RegularMeetings { get; set; } = string.Empty;
    public string RemoteWorkDays { get; set; } = string.Empty;
    public string Manager { get; set; } = string.Empty;
}
