namespace OBD.Mobile.FunctionalTests.Journeys;

/// <summary>Settings: the user sets the work habits and the manager, and exports the notebook as a ZIP to share.</summary>
[Collection(AppCollection.Name)]
public class ParametersJourneyTests
{
    private static readonly JsonSerializerOptions ReadOptions = new()
    {
        Converters = { new JsonStringEnumConverter<TypeNote>() },
    };

    [Fact]
    public async Task WorkHabits_ShouldBeSavedWithTheChosenManager_AndShownAgainNextTime()
    {
        // Arrange
        using var app = new AppSession();
        await app.Get<DataSeeder>().SeedAsync();
        var settings = app.Get<ParametersViewModel>();
        await settings.LoadCommand.ExecuteAsync(null);
        var seededManager = settings.SelectedManager;

        // Act
        settings.SelectedManager = settings.Persons.Single(p => p.Name == "Thomas Dubois");
        settings.WorkHabits.RemoteWorkDays = "Mardi et jeudi";
        await settings.SaveWorkHabitsCommand.ExecuteAsync(null);
        var nextVisit = app.Get<ParametersViewModel>();
        await nextVisit.LoadCommand.ExecuteAsync(null);

        // Assert
        seededManager.ShouldNotBeNull().Name.ShouldBe("Sophie Martin");
        settings.Sectors.Count.ShouldBe(5);
        settings.Persons.Count.ShouldBe(6);
        nextVisit.IsLoading.ShouldBeFalse();
        nextVisit.SelectedManager.ShouldNotBeNull().Name.ShouldBe("Thomas Dubois");
        nextVisit.WorkHabits.RemoteWorkDays.ShouldBe("Mardi et jeudi");
        (await app.Get<DatabaseContext>().GetAllAsync<WorkHabits>()).Count.ShouldBe(1);
    }

    [Fact]
    public async Task WorkHabits_ShouldBeCreated_OnAnEmptyNotebook()
    {
        // Arrange
        using var app = new AppSession();
        var settings = app.Get<ParametersViewModel>();
        await settings.LoadCommand.ExecuteAsync(null);

        // Act
        settings.WorkHabits.RegularMeetings = "Point d'équipe lundi 10h";
        await settings.SaveWorkHabitsCommand.ExecuteAsync(null);

        // Assert
        settings.SelectedManager.ShouldBeNull();
        var saved = (await app.Get<IWorkHabitsService>().GetAsync()).ShouldNotBeNull();
        saved.RegularMeetings.ShouldBe("Point d'équipe lundi 10h");
        saved.ManagerId.ShouldBe(0);
    }

    [Fact]
    public async Task Export_ShouldShareAZipOfTheNotebook_WithoutAskingAnything_WhenNothingIsSensitive()
    {
        // Arrange
        using var app = new AppSession();
        await app.Get<DataSeeder>().SeedAsync();
        var settings = app.Get<ParametersViewModel>();

        // Act
        await settings.ExportDataCommand.ExecuteAsync(null);

        // Assert
        AppPlatform.ChoicesShown.ShouldBeEmpty();
        AppPlatform.BiometricPrompts.ShouldBe(0);
        var shared = AppPlatform.SharedFiles.ShouldHaveSingleItem();
        shared.Title.ShouldBe(AppResources.GetString("Export_ShareTitle"));
        var file = shared.File.ShouldNotBeNull();
        file.ContentType.ShouldBe("application/zip");
        Path.GetFileName(file.FullPath).ShouldStartWith("OBD-");
        file.FullPath.ShouldStartWith(Path.Combine(AppPlatform.CacheDirectory, "exports"));
        var data = ReadExport(AppPlatform.SharedFileContents.Single());
        data.Sectors.Count.ShouldBe(5);
        data.Persons.Count.ShouldBe(6);
        data.Notes.Count.ShouldBe(4);
        data.IncludesSensitiveNotes.ShouldBeFalse();
    }

    [Fact]
    public async Task Export_ShouldLeaveTheSensitiveNotesOut_WhenTheUserSaysSo()
    {
        // Arrange
        using var app = new AppSession();
        await AddNotesAsync(app);
        AppPlatform.ChooseAnswer = (_, _) => AppResources.GetString("Export_WithoutSensitive");

        // Act
        await app.Get<ParametersViewModel>().ExportDataCommand.ExecuteAsync(null);

        // Assert
        var choice = AppPlatform.ChoicesShown.ShouldHaveSingleItem();
        choice.Title.ShouldBe(AppResources.GetString("Export_SensitiveTitle"));
        choice.Choices.ShouldBe([AppResources.GetString("Export_WithSensitive"), AppResources.GetString("Export_WithoutSensitive")]);
        AppPlatform.BiometricPrompts.ShouldBe(0);
        var data = ReadExport(AppPlatform.SharedFileContents.ShouldHaveSingleItem());
        data.IncludesSensitiveNotes.ShouldBeFalse();
        data.Notes.ShouldHaveSingleItem().Content.ShouldBe("Planning des congés");
    }

    [Fact]
    public async Task Export_ShouldTakeTheSensitiveNotesAlong_OnceTheUserIsAuthenticated()
    {
        // Arrange
        using var app = new AppSession();
        await AddNotesAsync(app);
        AppPlatform.ChooseAnswer = (_, _) => AppResources.GetString("Export_WithSensitive");

        // Act
        await app.Get<ParametersViewModel>().ExportDataCommand.ExecuteAsync(null);

        // Assert
        AppPlatform.BiometricPrompts.ShouldBe(1);
        var data = ReadExport(AppPlatform.SharedFileContents.ShouldHaveSingleItem());
        data.IncludesSensitiveNotes.ShouldBeTrue();
        data.Notes.Count.ShouldBe(2);
    }

    [Fact]
    public async Task Export_ShouldShareNothing_WhenTheAuthenticationFails()
    {
        // Arrange
        using var app = new AppSession();
        await AddNotesAsync(app);
        AppPlatform.ChooseAnswer = (_, _) => AppResources.GetString("Export_WithSensitive");
        AppPlatform.BiometricsSucceed = false;

        // Act
        await app.Get<ParametersViewModel>().ExportDataCommand.ExecuteAsync(null);

        // Assert
        AppPlatform.BiometricPrompts.ShouldBe(1);
        AppPlatform.SharedFiles.ShouldBeEmpty();
        Directory.Exists(Path.Combine(AppPlatform.CacheDirectory, "exports")).ShouldBeFalse();
    }

    [Fact]
    public async Task Export_ShouldShareNothing_WhenTheUserDismissesTheChoice()
    {
        // Arrange
        using var app = new AppSession();
        await AddNotesAsync(app);

        // Act
        await app.Get<ParametersViewModel>().ExportDataCommand.ExecuteAsync(null);

        // Assert
        AppPlatform.ChoicesShown.Count.ShouldBe(1);
        AppPlatform.SharedFiles.ShouldBeEmpty();
    }

    [Fact]
    public async Task Export_ShouldNotLeaveThePreviousArchiveInTheCache()
    {
        // Arrange
        using var app = new AppSession();
        await app.Get<DataSeeder>().SeedAsync();
        var settings = app.Get<ParametersViewModel>();
        await settings.ExportDataCommand.ExecuteAsync(null);
        var first = AppPlatform.SharedFiles[0].File!.FullPath;

        // Act
        await settings.ExportDataCommand.ExecuteAsync(null);

        // Assert
        var second = AppPlatform.SharedFiles[1].File!.FullPath;
        Path.GetDirectoryName(second).ShouldNotBe(Path.GetDirectoryName(first));
        File.Exists(first).ShouldBeFalse();
        Directory.EnumerateFiles(Path.Combine(AppPlatform.CacheDirectory, "exports"), "*", SearchOption.AllDirectories)
            .ShouldHaveSingleItem().ShouldBe(second);
    }

    [Fact]
    public async Task Export_ShouldTellTheUser_WhenTheArchiveCannotBeWritten()
    {
        // Arrange
        using var app = new AppSession();
        await app.Get<DataSeeder>().SeedAsync();
        // A file where the cache folder should be: nothing can be written under it
        var blocked = Path.Combine(app.Root, "not-a-folder");
        await File.WriteAllTextAsync(blocked, "x", TestContext.Current.CancellationToken);
        AppPlatform.CacheDirectory = blocked;

        // Act
        await app.Get<ParametersViewModel>().ExportDataCommand.ExecuteAsync(null);

        // Assert
        AppPlatform.SharedFiles.ShouldBeEmpty();
        AppPlatform.Alerts.ShouldHaveSingleItem().Message.ShouldBe(AppResources.GetString("Export_ErrorMessage"));
    }

    [Fact]
    public async Task SectorsOfTheSettings_ShouldOpenTheirDetails()
    {
        // Arrange
        using var app = new AppSession();
        await app.Get<DataSeeder>().SeedAsync();
        var settings = app.Get<ParametersViewModel>();
        await settings.LoadCommand.ExecuteAsync(null);
        var rh = settings.Sectors.Single(s => s.Name == "RH");

        // Act
        await settings.OpenSectorDetailCommand.ExecuteAsync(rh);
        var details = app.Get<SectorDetailsViewModel>();
        details.SectorId = rh.Id;
        await Eventually.HappensAsync(() => details.PageTitle == "RH", "the sector to load");

        // Assert
        AppSession.LastRoute.ShouldBe($"{nameof(SectorDetailsPage)}?id={rh.Id}");
        details.IsNew.ShouldBeFalse();
    }

    private static async Task AddNotesAsync(AppSession app)
    {
        var notes = app.Get<INoteService>();
        var visible = new Note { Content = "Planning des congés" };
        var secret = new Note { Content = "Entretiens annuels" };
        _ = await notes.InsertAsync(visible);
        _ = await notes.InsertAsync(secret);
        _ = await notes.SetSensitiveAsync(secret.Id, isSensitive: true);
    }

    private static DataExport ReadExport(byte[] archiveBytes)
    {
        using ZipArchive archive = new(new MemoryStream(archiveBytes), ZipArchiveMode.Read);
        using var data = archive.GetEntry(DataExportService.DataFileName).ShouldNotBeNull().Open();
        return JsonSerializer.Deserialize<DataExport>(data, ReadOptions).ShouldNotBeNull();
    }
}
