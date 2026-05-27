namespace OBD.Mobile.Lib.Models;

[Table("WorkHabits")]
public class WorkHabits
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string RegularMeetings { get; set; } = string.Empty;
    public string RemoteWorkDays { get; set; } = string.Empty;
    public int ManagerId { get; set; }

    [Ignore]
    public Person? Manager { get; set; }
}
