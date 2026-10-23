namespace OBD.Mobile.Lib.Services;

public class DataExportService(DatabaseContext db) : IDataExportService
{
    public const string DataFileName = "obd-data.json";
    public const string ImageFolder = "sketches";

    // Accents stay readable in the file instead of becoming é
    private static readonly DataExportJsonContext Json = new(new JsonSerializerOptions
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters = { new JsonStringEnumConverter<TypeNote>() },
    });

    public async Task<bool> HasSensitiveNotesAsync()
    {
        var notes = await db.QueryAsync<Note>("SELECT * FROM Note WHERE IsSensitive = 1 LIMIT 1");
        return notes.Count > 0;
    }

    public async Task WriteArchiveAsync(Stream destination, bool includeSensitive, CancellationToken stoppingToken)
    {
        var sectors = await db.GetAllAsync<Sector>();
        var persons = await db.GetAllAsync<Person>();
        var notes = (await db.GetAllAsync<Note>()).Where(n => includeSensitive || !n.IsSensitive).ToList();
        var workHabits = (await db.GetAllAsync<WorkHabits>()).FirstOrDefault();

        // A link from a note to a sketch left out of the export would point at nothing
        var noteIds = notes.Select(n => n.Id).ToHashSet();
        var links = (await db.GetAllAsync<NoteLink>())
            .Where(l => noteIds.Contains(l.NoteId) && noteIds.Contains(l.SketchId))
            .Select(l => new ExportedNoteLink(l.NoteId, l.Word, l.SketchId))
            .ToList();

        await using var archive = await ZipArchive.CreateAsync(destination, ZipArchiveMode.Create, leaveOpen: true, entryNameEncoding: null, stoppingToken);

        List<ExportedNote> exportedNotes = new(notes.Count);
        foreach (var note in notes)
        {
            var image = await WriteImageAsync(archive, note, stoppingToken);
            exportedNotes.Add(new ExportedNote(
                note.Id,
                note.SectorId,
                note.Type,
                note.Title,
                image is null ? note.Content : string.Empty,
                image,
                note.Keywords,
                note.IsSensitive,
                note.CreatedAt));
        }

        var export = new DataExport(
            DateTime.UtcNow,
            includeSensitive,
            [.. sectors.Select(s => new ExportedSector(s.Id, s.Name))],
            [.. persons.Select(p => new ExportedPerson(p.Id, p.Name, p.Position, p.SectorId, p.Memo))],
            exportedNotes,
            links,
            workHabits is null ? null : new ExportedWorkHabits(workHabits.RegularMeetings, workHabits.RemoteWorkDays, workHabits.ManagerId));

        var entry = archive.CreateEntry(DataFileName, CompressionLevel.Optimal);
        await using var stream = await entry.OpenAsync(stoppingToken);
        await JsonSerializer.SerializeAsync(stream, export, Json.DataExport, stoppingToken);
    }

    /// <summary>
    /// Writes the image of a sketch as a file of its own and returns its path in the archive,
    /// or null when the note holds no image.
    /// </summary>
    private static async Task<string?> WriteImageAsync(ZipArchive archive, Note note, CancellationToken stoppingToken)
    {
        if (note.Type is TypeNote.Text || string.IsNullOrEmpty(note.Content))
            return null;

        byte[] bytes;
        try
        {
            bytes = Convert.FromBase64String(note.Content);
        }
        catch (FormatException)
        {
            return null;
        }

        var isJpeg = bytes is [0xFF, 0xD8, ..];
        var path = $"{ImageFolder}/{note.Id}.{(isJpeg ? "jpg" : "png")}";

        // Already compressed
        var entry = archive.CreateEntry(path, CompressionLevel.NoCompression);
        await using var stream = await entry.OpenAsync(stoppingToken);
        await stream.WriteAsync(bytes, stoppingToken);
        return path;
    }
}
