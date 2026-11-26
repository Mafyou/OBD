namespace OBD.Mobile.Lib.IntegrationTests.Persistence;

/// <summary>
/// What the user wrote is still there the next time the app starts: the database file is read again by a new
/// <see cref="DatabaseContext"/> once every connection of the previous run is closed.
/// </summary>
[Collection(SqliteCollection.Name)]
public class AppRestartTests
{
    [Fact]
    public async Task Notebook_ShouldSurviveAnAppRestart()
    {
        // Arrange
        using var workspace = new TempWorkspace();
        int sectorId, personId, noteId, sketchId;
        using (var firstRun = workspace.BuildServices())
        {
            var sector = new Sector { Name = "Qualité" };
            _ = await firstRun.GetRequiredService<ISectorService>().InsertAsync(sector);
            var person = new Person { Name = "Élodie Carré", Position = "QA", SectorId = sector.Id, Memo = "Tests de non-régression" };
            _ = await firstRun.GetRequiredService<IPersonService>().InsertAsync(person);
            var note = new Note { SectorId = sector.Id, Content = "Voir #plan", Keywords = "qa" };
            var sketch = new Note { SectorId = sector.Id, Type = TypeNote.Sketch, Title = "plan", Content = Convert.ToBase64String([1, 2, 3]) };
            var notes = firstRun.GetRequiredService<INoteService>();
            _ = await notes.InsertAsync(note);
            _ = await notes.InsertAsync(sketch);
            _ = await firstRun.GetRequiredService<INoteLinkService>().InsertAsync(new NoteLink { NoteId = note.Id, Word = "plan", SketchId = sketch.Id });
            await firstRun.GetRequiredService<IWorkHabitsService>().SaveAsync(new WorkHabits { RegularMeetings = "Lundi 9h", ManagerId = person.Id });
            (sectorId, personId, noteId, sketchId) = (sector.Id, person.Id, note.Id, sketch.Id);
        }

        // Act
        TempWorkspace.SimulateAppRestart();
        using var secondRun = workspace.BuildServices();

        // Assert
        (await secondRun.GetRequiredService<ISectorService>().GetAsync(sectorId)).ShouldNotBeNull().Name.ShouldBe("Qualité");
        var savedPerson = (await secondRun.GetRequiredService<IPersonService>().GetAsync(personId)).ShouldNotBeNull();
        savedPerson.Name.ShouldBe("Élodie Carré");
        savedPerson.Memo.ShouldBe("Tests de non-régression");
        var savedSketch = (await secondRun.GetRequiredService<INoteService>().GetAsync(sketchId)).ShouldNotBeNull();
        Convert.FromBase64String(savedSketch.Content).ShouldBe(new byte[] { 1, 2, 3 });
        var link = (await secondRun.GetRequiredService<INoteLinkService>().GetByNoteAsync(noteId)).ShouldHaveSingleItem();
        link.SketchId.ShouldBe(sketchId);
        var habits = (await secondRun.GetRequiredService<IWorkHabitsService>().GetAsync()).ShouldNotBeNull();
        habits.ManagerId.ShouldBe(personId);
        habits.RegularMeetings.ShouldBe("Lundi 9h");
    }

    [Fact]
    public async Task WorkHabits_ShouldStayASingleRow_WhenSavedAgainAfterARestart()
    {
        // Arrange
        using var workspace = new TempWorkspace();
        using (var firstRun = workspace.BuildServices())
            await firstRun.GetRequiredService<IWorkHabitsService>().SaveAsync(new WorkHabits { RemoteWorkDays = "Lundi" });
        TempWorkspace.SimulateAppRestart();
        using var secondRun = workspace.BuildServices();
        var service = secondRun.GetRequiredService<IWorkHabitsService>();

        // Act
        var habits = (await service.GetAsync()).ShouldNotBeNull();
        habits.RemoteWorkDays = "Lundi et jeudi";
        await service.SaveAsync(habits);

        // Assert
        var rows = await secondRun.GetRequiredService<DatabaseContext>().GetAllAsync<WorkHabits>();
        rows.ShouldHaveSingleItem().RemoteWorkDays.ShouldBe("Lundi et jeudi");
    }

    [Fact]
    public async Task Seeder_ShouldNotSeedAgain_WhenTheAppStartsOnANotebookAlreadySeeded()
    {
        // Arrange
        using var workspace = new TempWorkspace();
        using (var firstRun = workspace.BuildServices())
            await firstRun.GetRequiredService<DataSeeder>().SeedAsync();
        TempWorkspace.SimulateAppRestart();
        using var secondRun = workspace.BuildServices();

        // Act
        await secondRun.GetRequiredService<DataSeeder>().SeedAsync();

        // Assert
        (await secondRun.GetRequiredService<ISectorService>().GetAllAsync()).Count.ShouldBe(5);
        (await secondRun.GetRequiredService<IPersonService>().GetAllAsync()).Count.ShouldBe(6);
    }

    [Fact]
    public void CrashState_ShouldReachTheNextLaunch_WhenTheAppCrashed()
    {
        // Arrange
        using var workspace = new TempWorkspace();
        var crashedRun = new AppCrashStateService(workspace.Root);
        crashedRun.MarkAppStarted();
        crashedRun.RecordCrash(new InvalidOperationException("Sketch could not be saved"), "AppDomain.UnhandledException");

        // Act
        var nextLaunch = new AppCrashStateService(workspace.Root);
        var wasUnclean = nextLaunch.WasPreviousRunUnclean();
        var details = nextLaunch.GetLastCrashDetails();

        // Assert
        wasUnclean.ShouldBeTrue();
        details.ShouldNotBeNull();
        details.ShouldContain("Source: AppDomain.UnhandledException");
        details.ShouldContain("System.InvalidOperationException");
        details.ShouldContain("Sketch could not be saved");
    }

    [Fact]
    public void CrashState_ShouldBeCleanOnTheNextLaunch_WhenTheAppClosedCleanly()
    {
        // Arrange
        using var workspace = new TempWorkspace();
        var run = new AppCrashStateService(workspace.Root);
        run.MarkAppStarted();

        // Act
        run.MarkAppClosedCleanly();
        var nextLaunch = new AppCrashStateService(workspace.Root);

        // Assert
        nextLaunch.WasPreviousRunUnclean().ShouldBeFalse();
        nextLaunch.GetLastCrashDetails().ShouldBeNull();
    }

    [Fact]
    public void CrashState_ShouldBeGoneForEveryLaunch_OnceCleared()
    {
        // Arrange
        using var workspace = new TempWorkspace();
        var run = new AppCrashStateService(workspace.Root);
        run.MarkAppStarted();
        run.RecordCrash(new TimeoutException("Slow disk"), "TaskScheduler.UnobservedTaskException");

        // Act
        new AppCrashStateService(workspace.Root).ClearCrashData();

        // Assert
        var later = new AppCrashStateService(workspace.Root);
        later.WasPreviousRunUnclean().ShouldBeFalse();
        later.GetLastCrashDetails().ShouldBeNull();
        Directory.EnumerateFiles(workspace.Root).ShouldBeEmpty();
    }
}
