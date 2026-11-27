namespace OBD.Mobile.Lib.IntegrationTests.Journeys;

/// <summary>
/// The services of the app working together on one real database, as the screens use them.
/// </summary>
[Collection(SqliteCollection.Name)]
public class NotebookServicesTests
{
    private static readonly byte[] PngBytes = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 7, 7, 7];
    private static readonly byte[] JpegBytes = [0xFF, 0xD8, 0xFF, 0xE0, 4, 2];

    private static readonly JsonSerializerOptions ReadOptions = new()
    {
        Converters = { new JsonStringEnumConverter<TypeNote>() },
    };

    [Fact]
    public async Task SeededNotebook_ShouldBeFoundByTheSearchOfEveryKindOfRecord()
    {
        // Arrange
        using var workspace = new TempWorkspace();
        using var provider = workspace.BuildServices();
        await provider.GetRequiredService<DataSeeder>().SeedAsync();

        // Act
        var persons = await provider.GetRequiredService<IPersonService>().SearchAsync("CI/CD");
        var sectors = await provider.GetRequiredService<ISectorService>().SearchAsync("dévelop");
        var notes = await provider.GetRequiredService<INoteService>().SearchAsync("figma");

        // Assert
        persons.ShouldHaveSingleItem().Name.ShouldBe("Julie Petit");
        sectors.ShouldHaveSingleItem().Name.ShouldBe("Développement");
        notes.ShouldHaveSingleItem().Keywords.ShouldContain("design system");
    }

    [Fact]
    public async Task SeededNotebook_ShouldNameTheManagerAmongThePersons()
    {
        // Arrange
        using var workspace = new TempWorkspace();
        using var provider = workspace.BuildServices();
        await provider.GetRequiredService<DataSeeder>().SeedAsync();

        // Act
        var habits = await provider.GetRequiredService<IWorkHabitsService>().GetAsync();
        var manager = await provider.GetRequiredService<IPersonService>().GetAsync(habits.ShouldNotBeNull().ManagerId);

        // Assert
        manager.ShouldNotBeNull().Name.ShouldBe("Sophie Martin");
        var managementSector = await provider.GetRequiredService<ISectorService>().GetAsync(manager.SectorId);
        managementSector.ShouldNotBeNull().Name.ShouldBe("Management");
    }

    [Fact]
    public async Task SeededNotebook_ShouldGiveEachSectorItsNotes()
    {
        // Arrange
        using var workspace = new TempWorkspace();
        using var provider = workspace.BuildServices();
        await provider.GetRequiredService<DataSeeder>().SeedAsync();
        var sectors = await provider.GetRequiredService<ISectorService>().GetAllAsync();
        var noteService = provider.GetRequiredService<INoteService>();

        // Act
        Dictionary<string, int> notesBySector = [];
        foreach (var sector in sectors)
            notesBySector[sector.Name] = (await noteService.GetBySectorAsync(sector.Id)).Count;

        // Assert
        notesBySector["Développement"].ShouldBe(1);
        notesBySector["Produit"].ShouldBe(1);
        notesBySector["Design"].ShouldBe(1);
        notesBySector["Management"].ShouldBe(1);
        notesBySector["RH"].ShouldBe(0);
    }

    [Fact]
    public async Task DeletingASketch_ShouldRemoveTheLinksOfTheNotesThatPointedAtIt()
    {
        // Arrange
        using var workspace = new TempWorkspace();
        using var provider = workspace.BuildServices();
        var notes = provider.GetRequiredService<INoteService>();
        var links = provider.GetRequiredService<INoteLinkService>();
        var note = new Note { Content = "Voir #organigramme et #plan" };
        var orgChart = new Note { Type = TypeNote.Sketch, Title = "Organigramme" };
        var plan = new Note { Type = TypeNote.Photo, Title = "Plan du bureau" };
        _ = await notes.InsertAsync(note);
        _ = await notes.InsertAsync(orgChart);
        _ = await notes.InsertAsync(plan);
        _ = await links.InsertAsync(new NoteLink { NoteId = note.Id, Word = "organigramme", SketchId = orgChart.Id });
        _ = await links.InsertAsync(new NoteLink { NoteId = note.Id, Word = "plan", SketchId = plan.Id });

        // Act
        var deleted = await notes.DeleteAsync(orgChart);

        // Assert
        deleted.ShouldBe(1);
        (await links.GetByNoteAsync(note.Id)).ShouldHaveSingleItem().Word.ShouldBe("plan");
        (await notes.GetVisualsAsync()).ShouldHaveSingleItem().Id.ShouldBe(plan.Id);
    }

    [Fact]
    public async Task MarkingANoteSensitive_ShouldHideItFromItsSectorAndTheSearch_AndListItWithTheSensitiveOnes()
    {
        // Arrange
        using var workspace = new TempWorkspace();
        using var provider = workspace.BuildServices();
        var sector = new Sector { Name = "RH" };
        _ = await provider.GetRequiredService<ISectorService>().InsertAsync(sector);
        var notes = provider.GetRequiredService<INoteService>();
        var salary = new Note { SectorId = sector.Id, Content = "Grille des salaires", Keywords = "salaire" };
        _ = await notes.InsertAsync(salary);
        var export = provider.GetRequiredService<IDataExportService>();
        (await export.HasSensitiveNotesAsync()).ShouldBeFalse();

        // Act
        _ = await notes.SetSensitiveAsync(salary.Id, isSensitive: true);

        // Assert
        (await notes.GetBySectorAsync(sector.Id)).ShouldBeEmpty();
        (await notes.SearchAsync("salaire")).ShouldBeEmpty();
        (await notes.GetSensitiveAsync()).ShouldHaveSingleItem().Id.ShouldBe(salary.Id);
        (await export.HasSensitiveNotesAsync()).ShouldBeTrue();
    }

    [Fact]
    public async Task Export_ShouldWriteAnArchiveFileThatHoldsTheWholeNotebook()
    {
        // Arrange
        using var workspace = new TempWorkspace();
        using var provider = workspace.BuildServices();
        await provider.GetRequiredService<DataSeeder>().SeedAsync();
        var notes = provider.GetRequiredService<INoteService>();
        var note = new Note { SectorId = 1, Content = "Voir #schéma", Keywords = "archi" };
        var sketch = new Note { SectorId = 1, Type = TypeNote.Sketch, Title = "schéma", Content = Convert.ToBase64String(PngBytes) };
        var photo = new Note { SectorId = 1, Type = TypeNote.Photo, Title = "Badge", Content = Convert.ToBase64String(JpegBytes) };
        _ = await notes.InsertAsync(note);
        _ = await notes.InsertAsync(sketch);
        _ = await notes.InsertAsync(photo);
        _ = await provider.GetRequiredService<INoteLinkService>().InsertAsync(new NoteLink { NoteId = note.Id, Word = "schéma", SketchId = sketch.Id });
        var archivePath = workspace.PathOf("export.zip");

        // Act
        await using (var file = File.Create(archivePath))
            await provider.GetRequiredService<IDataExportService>().WriteArchiveAsync(file, includeSensitive: false, TestContext.Current.CancellationToken);

        // Assert
        using var archive = ZipFile.OpenRead(archivePath);
        archive.Entries.Select(e => e.FullName).ShouldBe(
            [$"{DataExportService.ImageFolder}/{sketch.Id}.png", $"{DataExportService.ImageFolder}/{photo.Id}.jpg", DataExportService.DataFileName],
            ignoreOrder: true);
        ReadEntry(archive, $"{DataExportService.ImageFolder}/{sketch.Id}.png").ShouldBe(PngBytes);
        ReadEntry(archive, $"{DataExportService.ImageFolder}/{photo.Id}.jpg").ShouldBe(JpegBytes);

        var data = JsonSerializer.Deserialize<DataExport>(ReadEntry(archive, DataExportService.DataFileName), ReadOptions).ShouldNotBeNull();
        data.IncludesSensitiveNotes.ShouldBeFalse();
        data.Sectors.Count.ShouldBe(5);
        data.Persons.Count.ShouldBe(6);
        data.Notes.Count.ShouldBe(7);
        data.Notes.Single(n => n.Id == sketch.Id).Image.ShouldBe($"{DataExportService.ImageFolder}/{sketch.Id}.png");
        data.NoteLinks.ShouldHaveSingleItem().ShouldBe(new ExportedNoteLink(note.Id, "schéma", sketch.Id));
        var habits = data.WorkHabits.ShouldNotBeNull();
        data.Persons.Single(p => p.Id == habits.ManagerId).Name.ShouldBe("Sophie Martin");
    }

    [Fact]
    public async Task Export_ShouldLeaveASensitiveSketchBehind_UnlessAskedFor()
    {
        // Arrange
        using var workspace = new TempWorkspace();
        using var provider = workspace.BuildServices();
        var notes = provider.GetRequiredService<INoteService>();
        var note = new Note { Content = "Voir #accès" };
        var secret = new Note { Type = TypeNote.Sketch, Title = "Codes d'accès", Content = Convert.ToBase64String(PngBytes) };
        _ = await notes.InsertAsync(note);
        _ = await notes.InsertAsync(secret);
        _ = await provider.GetRequiredService<INoteLinkService>().InsertAsync(new NoteLink { NoteId = note.Id, Word = "accès", SketchId = secret.Id });
        _ = await notes.SetSensitiveAsync(secret.Id, isSensitive: true);
        var export = provider.GetRequiredService<IDataExportService>();

        // Act
        var without = await ExportAsync(export, includeSensitive: false);
        var with = await ExportAsync(export, includeSensitive: true);

        // Assert
        without.Entries.Select(e => e.FullName).ShouldBe([DataExportService.DataFileName]);
        var withoutData = JsonSerializer.Deserialize<DataExport>(ReadEntry(without, DataExportService.DataFileName), ReadOptions).ShouldNotBeNull();
        withoutData.Notes.ShouldHaveSingleItem().Id.ShouldBe(note.Id);
        withoutData.NoteLinks.ShouldBeEmpty();

        with.Entries.Count.ShouldBe(2);
        var withData = JsonSerializer.Deserialize<DataExport>(ReadEntry(with, DataExportService.DataFileName), ReadOptions).ShouldNotBeNull();
        withData.IncludesSensitiveNotes.ShouldBeTrue();
        withData.Notes.Count.ShouldBe(2);
        withData.NoteLinks.ShouldHaveSingleItem().SketchId.ShouldBe(secret.Id);
    }

    [Fact]
    public async Task ColdStart_ShouldAnswerQueriesStartedTogetherOnAFreshDatabase()
    {
        // Arrange
        using var workspace = new TempWorkspace();
        using (var seeding = workspace.BuildServices())
            await seeding.GetRequiredService<DataSeeder>().SeedAsync();
        TempWorkspace.SimulateAppRestart();
        using var provider = workspace.BuildServices();

        // Act: the search screen runs its three queries at once, before any connection exists
        var persons = provider.GetRequiredService<IPersonService>().SearchAsync("a");
        var notes = provider.GetRequiredService<INoteService>().SearchAsync("a");
        var sectors = provider.GetRequiredService<ISectorService>().SearchAsync("a");
        await Task.WhenAll(persons, notes, sectors);

        // Assert
        (await persons).ShouldNotBeEmpty();
        (await notes).ShouldNotBeEmpty();
        (await sectors).ShouldNotBeEmpty();
    }

    private static async Task<ZipArchive> ExportAsync(IDataExportService export, bool includeSensitive)
    {
        MemoryStream stream = new();
        await export.WriteArchiveAsync(stream, includeSensitive, TestContext.Current.CancellationToken);
        stream.Position = 0;
        return new ZipArchive(stream, ZipArchiveMode.Read);
    }

    private static byte[] ReadEntry(ZipArchive archive, string name)
    {
        using var entry = archive.GetEntry(name).ShouldNotBeNull().Open();
        using MemoryStream copy = new();
        entry.CopyTo(copy);
        return copy.ToArray();
    }
}
