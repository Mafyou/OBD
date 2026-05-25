namespace OBD.Mobile.Lib.Models;

public enum TypeNote { Text, Sketch, Photo }

public class Note
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    public int SectorId { get; set; }
    public TypeNote Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Keywords { get; set; } = string.Empty;
    public bool IsSensitive { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Ignore]
    public string DisplayText => Type switch
    {
        TypeNote.Sketch => string.IsNullOrEmpty(Title) ? "✏ Croquis" : $"✏ {Title}",
        TypeNote.Photo => string.IsNullOrEmpty(Title) ? "📷 Photo" : $"📷 {Title}",
        _ => string.IsNullOrEmpty(Title) ? Content : Title,
    };
}
