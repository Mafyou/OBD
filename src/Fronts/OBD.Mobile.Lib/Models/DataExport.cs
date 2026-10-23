namespace OBD.Mobile.Lib.Models;

/// <summary>
/// Data file of an export: every record of the notebook, in a format other tools can read.
/// The images of the sketches are written beside it in the archive, at the path given by <see cref="ExportedNote.Image"/>.
/// </summary>
public sealed record DataExport(
    DateTime ExportedAt,
    bool IncludesSensitiveNotes,
    IReadOnlyList<ExportedSector> Sectors,
    IReadOnlyList<ExportedPerson> Persons,
    IReadOnlyList<ExportedNote> Notes,
    IReadOnlyList<ExportedNoteLink> NoteLinks,
    ExportedWorkHabits? WorkHabits);

public readonly record struct ExportedSector(int Id, string Name);

public readonly record struct ExportedPerson(int Id, string Name, string Position, int SectorId, string Memo);

/// <summary>
/// A note; for a sketch, <see cref="Content"/> is empty and <see cref="Image"/> names its file in the archive.
/// </summary>
public readonly record struct ExportedNote(
    int Id,
    int SectorId,
    TypeNote Type,
    string Title,
    string Content,
    string? Image,
    string Keywords,
    bool IsSensitive,
    DateTime CreatedAt);

public readonly record struct ExportedNoteLink(int NoteId, string Word, int SketchId);

public readonly record struct ExportedWorkHabits(string RegularMeetings, string RemoteWorkDays, int ManagerId);

// Source generated: the reflection serializer may lose these types to trimming in a Release build
[JsonSourceGenerationOptions(UseStringEnumConverter = true)]
[JsonSerializable(typeof(DataExport))]
public partial class DataExportJsonContext : JsonSerializerContext;
