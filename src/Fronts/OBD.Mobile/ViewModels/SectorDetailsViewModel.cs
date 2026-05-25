namespace OBD.Mobile.ViewModels;

[QueryProperty(nameof(SectorId), "id")]
public partial class SectorDetailsViewModel(ISectorService secteurService) : ObservableObject
{
    [ObservableProperty]
    public partial Sector Sector { get; set; } = new();

    [ObservableProperty]
    public partial bool IsNew { get; set; } = true;

    [ObservableProperty]
    public partial string PageTitle { get; set; } = "Nouveau secteur";

    public int SectorId
    {
        set => _ = LoadAsync(value);
    }

    private async Task LoadAsync(int id)
    {
        if (id is 0)
        {
            Sector = new Sector();
            IsNew = true;
            PageTitle = "Nouveau secteur";
        }
        else
        {
            Sector = await secteurService.GetAsync(id) ?? new Sector();
            IsNew = false;
            PageTitle = Sector.Name;
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
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
    private async Task DeleteAsync()
    {
        await secteurService.DeleteAsync(Sector);
        await Shell.Current.GoToAsync("..");
    }
}
