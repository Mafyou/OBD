namespace OBD.Mobile.ViewModels;

public partial class ParametersViewModel(ISectorService secteurService, IReperesTravailService reperesTravailService) : ObservableObject
{
    [ObservableProperty]
    public partial ObservableCollection<Sector> Sectors { get; set; } = [];

    [ObservableProperty]
    public partial ReperesTravail WorkHabits { get; set; } = new();

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [RelayCommand(IncludeCancelCommand = true)]
    private async Task LoadAsync(CancellationToken stoppingToken)
    {
        IsLoading = true;
        var secteursList = await secteurService.GetAllAsync();
        Sectors = new ObservableCollection<Sector>(secteursList);
        WorkHabits = await reperesTravailService.GetAsync() ?? new ReperesTravail();
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
        => await reperesTravailService.SaveAsync(WorkHabits);
}
