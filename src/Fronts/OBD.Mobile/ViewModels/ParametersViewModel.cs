namespace OBD.Mobile.ViewModels;

public partial class ParametersViewModel(
    ISectorService secteurService,
    IWorkHabitsService workHabitsService,
    IPersonService personneService,
    IDataExportService dataExportService) : ObservableObject
{
    [ObservableProperty]
    public partial ObservableCollection<Sector> Sectors { get; set; } = [];

    [ObservableProperty]
    public partial ObservableCollection<Person> Persons { get; set; } = [];

    [ObservableProperty]
    public partial Person? SelectedManager { get; set; }

    [ObservableProperty]
    public partial WorkHabits WorkHabits { get; set; } = new();

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    public IReadOnlyList<LanguageOption> AvailableLanguages { get; } = LocalizationResourceManager.SupportedLanguages;

    [ObservableProperty]
    public partial LanguageOption SelectedLanguage { get; set; } = LocalizationResourceManager.Instance.CurrentLanguage;

    partial void OnSelectedLanguageChanged(LanguageOption value) =>
        LocalizationResourceManager.Instance.SetLanguage(value.Code);

    [RelayCommand(IncludeCancelCommand = true)]
    private async Task LoadAsync(CancellationToken stoppingToken)
    {
        IsLoading = true;
        var secteursList = await secteurService.GetAllAsync();
        Sectors = new ObservableCollection<Sector>(secteursList);

        var personsList = await personneService.GetAllAsync();
        Persons = new ObservableCollection<Person>(personsList);

        WorkHabits = await workHabitsService.GetAsync() ?? new WorkHabits();
        SelectedManager = Persons.FirstOrDefault(p => p.Id == WorkHabits.ManagerId);

        IsLoading = false;
    }

    [RelayCommand]
    private static async Task OpenSectorDetailAsync(Sector? secteur)
    {
        int id = secteur?.Id ?? 0;
        await Shell.Current.GoToAsync($"{nameof(SectorDetailsPage)}?id={id}");
    }

    [RelayCommand]
    private async Task SaveWorkHabitsAsync(CancellationToken stoppingToken)
    {
        WorkHabits.ManagerId = SelectedManager?.Id ?? 0;
        await workHabitsService.SaveAsync(WorkHabits);
    }

    [RelayCommand]
    private async Task ExportDataAsync(CancellationToken stoppingToken)
    {
        var includeSensitive = false;
        if (await dataExportService.HasSensitiveNotesAsync())
        {
            var withSensitive = AppResources.GetString("Export_WithSensitive");
            var choice = await Dialogs.ChooseAsync(
                AppResources.GetString("Export_SensitiveTitle"),
                AppResources.GetString("Common_Cancel"),
                null,
                withSensitive,
                AppResources.GetString("Export_WithoutSensitive"));
            if (choice is null)
                return;

            includeSensitive = choice == withSensitive;
            if (includeSensitive && !await SensitiveAccess.AuthenticateAsync())
                return;
        }

        try
        {
            // One archive at a time in the cache: the previous one, sensitive notes included, does not stay behind
            var folder = Path.Combine(FileSystem.CacheDirectory, "exports");
            if (Directory.Exists(folder))
                Directory.Delete(folder, recursive: true);
            // Each export in a folder of its own: on Android an app the previous archive was shared with keeps its
            // read grant on that path, and would read a new archive written at the same place, sensitive notes included
            var exportFolder = Path.Combine(folder, $"{Guid.NewGuid():N}");
            Directory.CreateDirectory(exportFolder);

            var path = Path.Combine(exportFolder, $"OBD-{DateTime.Now:yyyy-MM-dd-HHmm}.zip");
            await using (var file = File.Create(path))
                await dataExportService.WriteArchiveAsync(file, includeSensitive, stoppingToken);

            await Share.Default.RequestAsync(new ShareFileRequest(
                AppResources.GetString("Export_ShareTitle"),
                new ShareFile(path, "application/zip")));
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            await Dialogs.AlertAsync(
                AppResources.GetString("Common_Error"),
                AppResources.GetString("Export_ErrorMessage"),
                AppResources.GetString("Common_Ok"));
        }
    }
}
