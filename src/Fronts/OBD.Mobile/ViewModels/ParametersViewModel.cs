namespace OBD.Mobile.ViewModels;

public partial class ParametersViewModel(ISectorService secteurService, IWorkHabitsService workHabitsService, IPersonService personneService) : ObservableObject
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
    private async Task SaveWorkHabitsAsync()
    {
        WorkHabits.ManagerId = SelectedManager?.Id ?? 0;
        await workHabitsService.SaveAsync(WorkHabits);
    }
}
