namespace OBD.Mobile.ViewModels;

public partial class NotesViewModel(ISectorService secteurService) : ObservableObject
{
    [ObservableProperty]
    public partial ObservableCollection<Sector> Sectors { get; set; } = [];

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [RelayCommand(IncludeCancelCommand = true)]
    private async Task LoadSectorsAsync(CancellationToken stoppingToken)
    {
        IsLoading = true;
        var list = await secteurService.GetAllAsync();
        Sectors = new ObservableCollection<Sector>(list);
        IsLoading = false;
    }

    [RelayCommand]
    private static async Task OpenSectorAsync(Sector secteur)
        => await Shell.Current.GoToAsync($"{nameof(NotesSectorPage)}?sectorid={secteur.Id}");
}
