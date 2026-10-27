namespace OBD.Mobile.Lib.UnitTests.Services;

[Collection("DatabaseTests")]
public class DataExportServiceTests
{
    private static readonly byte[] PngBytes = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 1, 2, 3];

    [Fact]
    public async Task HasSensitiveNotesAsync_ShouldBeFalse_WhenNoNoteIsSensitive()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        _ = await TestDatabaseHelper.SeedBasicDataAsync(db);

        var result = await new DataExportService(db).HasSensitiveNotesAsync();

        result.ShouldBeFalse();
    }

    [Fact]
    public async Task HasSensitiveNotesAsync_ShouldBeTrue_WhenANoteIsSensitive()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        _ = await db.InsertAsync(new Note { Title = "Salary", IsSensitive = true });

        var result = await new DataExportService(db).HasSensitiveNotesAsync();

        result.ShouldBeTrue();
    }

    [Fact]
    public async Task HasSensitiveNotesAsync_ShouldBeFalse_WhenTheNotebookIsEmpty()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();

        var result = await new DataExportService(db).HasSensitiveNotesAsync();

        result.ShouldBeFalse();
    }

    [Fact]
    public async Task WriteArchiveAsync_ShouldExportEveryRecord()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        var (sector, note, person) = await TestDatabaseHelper.SeedBasicDataAsync(db);
        _ = await db.InsertAsync(new WorkHabits { RegularMeetings = "Réunion du lundi", RemoteWorkDays = "Vendredi", ManagerId = person.Id });

        var (export, _) = await ExportAsync(db, includeSensitive: false);

        export.IncludesSensitiveNotes.ShouldBeFalse();
        export.Sectors.ShouldBe([new ExportedSector(sector.Id, "Engineering")]);
        export.Persons.ShouldBe([new ExportedPerson(person.Id, "Alice Martin", "Developer", sector.Id, "Main contact")]);
        export.Notes.ShouldHaveSingleItem().Content.ShouldBe(note.Content);
        export.WorkHabits.ShouldBe(new ExportedWorkHabits("Réunion du lundi", "Vendredi", person.Id));
    }

    [Fact]
    public async Task WriteArchiveAsync_ShouldWriteSketchesAsImageFiles()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        var sketch = new Note { Type = TypeNote.Sketch, Title = "Plan", Content = Convert.ToBase64String(PngBytes) };
        _ = await db.InsertAsync(sketch);

        var (export, archive) = await ExportAsync(db, includeSensitive: false);

        var exported = export.Notes.ShouldHaveSingleItem();
        exported.Content.ShouldBeEmpty();
        exported.Image.ShouldBe($"{DataExportService.ImageFolder}/{sketch.Id}.png");
        await using var image = await archive.GetEntry(exported.Image!)!.OpenAsync(TestContext.Current.CancellationToken);
        using var copy = new MemoryStream();
        await image.CopyToAsync(copy, TestContext.Current.CancellationToken);
        copy.ToArray().ShouldBe(PngBytes);
    }

    [Fact]
    public async Task WriteArchiveAsync_ShouldLeaveOutSensitiveNotesAndTheirLinks_WhenNotIncluded()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        var (_, note, _) = await TestDatabaseHelper.SeedBasicDataAsync(db);
        var secret = new Note { Type = TypeNote.Sketch, Content = Convert.ToBase64String(PngBytes), IsSensitive = true };
        _ = await db.InsertAsync(secret);
        _ = await db.InsertAsync(new NoteLink { NoteId = note.Id, Word = "plan", SketchId = secret.Id });

        var (export, archive) = await ExportAsync(db, includeSensitive: false);

        export.Notes.ShouldHaveSingleItem().Id.ShouldBe(note.Id);
        export.NoteLinks.ShouldBeEmpty();
        archive.Entries.ShouldNotContain(e => e.FullName.StartsWith(DataExportService.ImageFolder));
    }

    [Fact]
    public async Task WriteArchiveAsync_ShouldKeepSensitiveNotesAndTheirLinks_WhenIncluded()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        var (_, note, _) = await TestDatabaseHelper.SeedBasicDataAsync(db);
        var secret = new Note { Type = TypeNote.Sketch, Content = Convert.ToBase64String(PngBytes), IsSensitive = true };
        _ = await db.InsertAsync(secret);
        _ = await db.InsertAsync(new NoteLink { NoteId = note.Id, Word = "plan", SketchId = secret.Id });

        var (export, _) = await ExportAsync(db, includeSensitive: true);

        export.IncludesSensitiveNotes.ShouldBeTrue();
        export.Notes.Count.ShouldBe(2);
        export.NoteLinks.ShouldBe([new ExportedNoteLink(note.Id, "plan", secret.Id)]);
    }

    [Fact]
    public async Task WriteArchiveAsync_ShouldKeepAccentsReadableAndTypesAsNames()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        _ = await db.InsertAsync(new Note { Type = TypeNote.Text, Content = "Café à l'accueil" });

        var (_, archive) = await ExportAsync(db, includeSensitive: false);

        await using var data = await archive.GetEntry(DataExportService.DataFileName)!.OpenAsync(TestContext.Current.CancellationToken);
        using var reader = new StreamReader(data, Encoding.UTF8);
        var json = await reader.ReadToEndAsync(TestContext.Current.CancellationToken);
        json.ShouldContain("Café à l'accueil");
        json.ShouldContain("\"Text\"");
    }

    private static async Task<(DataExport Export, ZipArchive Archive)> ExportAsync(DatabaseContext db, bool includeSensitive)
    {
        var stoppingToken = TestContext.Current.CancellationToken;
        var file = new MemoryStream();
        await new DataExportService(db).WriteArchiveAsync(file, includeSensitive, stoppingToken);

        file.Position = 0;
        var archive = new ZipArchive(file, ZipArchiveMode.Read);
        await using var data = await archive.GetEntry(DataExportService.DataFileName)!.OpenAsync(stoppingToken);
        var export = await JsonSerializer.DeserializeAsync(data, DataExportJsonContext.Default.DataExport, stoppingToken);
        return (export!, archive);
    }
}
