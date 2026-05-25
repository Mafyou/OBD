namespace OBD.Mobile.Lib.Models;

public class NoteLink
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    public int NoteId { get; set; }
    public string Word { get; set; } = string.Empty;
    public int SketchId { get; set; }

    [Ignore]
    public string SketchTitle { get; set; } = string.Empty;
}
