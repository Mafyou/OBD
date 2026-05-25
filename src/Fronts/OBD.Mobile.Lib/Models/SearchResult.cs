namespace OBD.Mobile.Lib.Models;

public record SearchResult(
    ResultType ResultType,
    string Title,
    string Subtitle,
    int Id,
    int SectorId,
    TypeNote NoteType);
