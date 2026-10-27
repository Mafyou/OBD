namespace OBD.Mobile.Lib.Services;

public class DataExportService(DatabaseContext db) : IDataExportService
{
    public const string DataFileName = "obd-data.json";
    public const string ImageFolder = "sketches";

    // 0 is TypeNote.Text, stored as a number: only a text note keeps its content in the list
    private const string NoteColumnsWithoutImages =
        "SELECT Id, SectorId, Type, Title, CASE WHEN Type = 0 THEN Content ELSE '' END AS Content, Keywords, IsSensitive, CreatedAt FROM Note";

    // Accents stay readable in the file instead of becoming é
    private static readonly DataExportJsonContext Json = new(new JsonSerializerOptions
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters = { new JsonStringEnumConverter<TypeNote>() },
    });

    // Asked without reading a note: the first sensitive one could be a sketch, its whole image with it
    public async Task<bool> HasSensitiveNotesAsync() =>
        await db.ExecuteScalarAsync<int>("SELECT EXISTS(SELECT 1 FROM Note WHERE IsSensitive = 1)") is 1;

    public async Task WriteArchiveAsync(Stream destination, bool includeSensitive, CancellationToken stoppingToken)
    {
        // Off the caller's thread from here: compressing a notebook full of sketches took the screen with it
        var sectors = await db.GetAllAsync<Sector>().ConfigureAwait(false);
        var persons = await db.GetAllAsync<Person>().ConfigureAwait(false);
        // The images of the sketches are read one at a time, as each is written (WriteImageAsync): all of them together
        // held the whole notebook in memory, twice over with their base64
        var notes = (await db.QueryAsync<Note>(NoteColumnsWithoutImages).ConfigureAwait(false))
            .Where(n => includeSensitive || !n.IsSensitive)
            .ToList();
        var workHabits = (await db.GetAllAsync<WorkHabits>().ConfigureAwait(false)).FirstOrDefault();

        await using var archive = await ZipArchive.CreateAsync(destination, ZipArchiveMode.Create, leaveOpen: true, entryNameEncoding: null, stoppingToken).ConfigureAwait(false);

        List<ExportedNote> exportedNotes = new(notes.Count);
        foreach (var note in notes)
        {
            var (included, image, content) = note.Type is TypeNote.Text
                ? (true, null, note.Content)
                : await WriteImageAsync(archive, note.Id, includeSensitive, stoppingToken).ConfigureAwait(false);
            if (!included)
                continue;

            exportedNotes.Add(new ExportedNote(
                note.Id,
                note.SectorId,
                note.Type,
                note.Title,
                content,
                image,
                note.Keywords,
                note.IsSensitive,
                note.CreatedAt));
        }

        // A link from a note to a sketch left out of the export would point at nothing
        var noteIds = exportedNotes.Select(n => n.Id).ToHashSet();
        var links = (await db.GetAllAsync<NoteLink>().ConfigureAwait(false))
            .Where(l => noteIds.Contains(l.NoteId) && noteIds.Contains(l.SketchId))
            .Select(l => new ExportedNoteLink(l.NoteId, l.Word, l.SketchId))
            .ToList();

        var export = new DataExport(
            DateTime.UtcNow,
            includeSensitive,
            [.. sectors.Select(s => new ExportedSector(s.Id, s.Name))],
            [.. persons.Select(p => new ExportedPerson(p.Id, p.Name, p.Position, p.SectorId, p.Memo))],
            exportedNotes,
            links,
            workHabits is null ? null : new ExportedWorkHabits(workHabits.RegularMeetings, workHabits.RemoteWorkDays, workHabits.ManagerId));

        var entry = archive.CreateEntry(DataFileName, CompressionLevel.Optimal);
        await using var stream = await entry.OpenAsync(stoppingToken).ConfigureAwait(false);
        await JsonSerializer.SerializeAsync(stream, export, Json.DataExport, stoppingToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Writes the image of a sketch as a file of its own. Returns whether the note still belongs in the export, the path
    /// of its image in the archive, and the content to keep in the data file when there is no image.
    /// </summary>
    private async Task<(bool Included, string? Image, string Content)> WriteImageAsync(ZipArchive archive, int noteId, bool includeSensitive, CancellationToken stoppingToken)
    {
        // The image and its sensitivity are read together: a sketch marked sensitive (or deleted) since the list was read
        // must not leave with its image
        var current = (await db.QueryAsync<Note>("SELECT Id, Content, IsSensitive FROM Note WHERE Id = ?", noteId).ConfigureAwait(false))
            .FirstOrDefault();
        if (current is null || (current.IsSensitive && !includeSensitive))
            return (false, null, string.Empty);

        var content = current.Content ?? string.Empty;
        if (content.Length == 0)
            return (true, null, content);

        byte[] bytes;
        try
        {
            bytes = Convert.FromBase64String(content);
        }
        catch (FormatException)
        {
            // Not an image after all: the data file keeps what the note holds
            return (true, null, content);
        }

        var isJpeg = bytes is [0xFF, 0xD8, ..];
        var path = $"{ImageFolder}/{noteId}.{(isJpeg ? "jpg" : "png")}";

        // Already compressed
        var entry = archive.CreateEntry(path, CompressionLevel.NoCompression);
        await using var stream = await entry.OpenAsync(stoppingToken).ConfigureAwait(false);
        await stream.WriteAsync(bytes, stoppingToken).ConfigureAwait(false);
        return (true, path, string.Empty);
    }
}
