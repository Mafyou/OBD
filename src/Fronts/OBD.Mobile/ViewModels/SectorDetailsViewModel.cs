namespace OBD.Mobile.ViewModels;

[QueryProperty(nameof(SectorId), "id")]
public partial class SectorDetailsViewModel(ISectorService secteurService) : ObservableObject
{
    [ObservableProperty]
    public partial Sector Sector { get; set; } = new();

    [ObservableProperty]
    public partial bool IsNew { get; set; } = true;

    [ObservableProperty]
    public partial string PageTitle { get; set; } = AppResources.GetString("SectorDetails_NewTitle");

    // Shell applies the query of the route again each time the page comes back to the front, a popup closed among
    // others ("Cancel" on the delete confirmation): only the first application chooses the sector, or the form being
    // edited would be replaced. The page and its view model are built for each visit.
    private bool _isQueryApplied;

    public int SectorId
    {
        set
        {
            if (_isQueryApplied) return;

            _isQueryApplied = true;
            _ = LoadAsync(value);
        }
    }

    private async Task LoadAsync(int id)
    {
        if (id is 0)
        {
            Sector = new Sector();
            IsNew = true;
            PageTitle = AppResources.GetString("SectorDetails_NewTitle");
        }
        else
        {
            Sector = await secteurService.GetAsync(id) ?? new Sector();
            IsNew = false;
            PageTitle = Sector.Name;
        }
    }

    [RelayCommand]
    private async Task SaveAsync(CancellationToken stoppingToken)
    {
        if (string.IsNullOrWhiteSpace(Sector.Name))
            return;

        if (IsNew)
            await secteurService.InsertAsync(Sector);
        else
            await secteurService.UpdateAsync(Sector);

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task DeleteAsync(CancellationToken stoppingToken)
    {
        await secteurService.DeleteAsync(Sector);
        await Shell.Current.GoToAsync("..");
    }
}
