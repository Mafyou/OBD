namespace OBD.Mobile.ViewModels;

[QueryProperty(nameof(PersonId), "id")]
public partial class PersonDetailsViewModel(IPersonService personService, ISectorService sectorService) : ObservableObject
{
    [ObservableProperty]
    public partial Person Person { get; set; } = new();

    [ObservableProperty]
    public partial ObservableCollection<Sector> Sectors { get; set; } = [];

    [ObservableProperty]
    public partial Sector? SelectedSector { get; set; }

    [ObservableProperty]
    public partial bool IsNew { get; set; } = true;

    [ObservableProperty]
    public partial bool HasNoSectors { get; set; }

    [ObservableProperty]
    public partial string PageTitle { get; set; } = AppResources.GetString("PersonDetails_NewTitle");

    // Shell applies the query of the route again each time the page comes back to the front, a popup closed among
    // others ("Cancel" on the delete confirmation): only the first application chooses the person, or the form being
    // edited would be replaced. The page and its view model are built for each visit.
    private bool _isQueryApplied;

    public int PersonId
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
        var sectors = await sectorService.GetAllAsync();
        Sectors = new ObservableCollection<Sector>(sectors);
        HasNoSectors = Sectors.Count == 0;

        if (id is 0)
        {
            Person = new Person();
            IsNew = true;
            PageTitle = AppResources.GetString("PersonDetails_NewTitle");
        }
        else
        {
            Person = await personService.GetAsync(id) ?? new Person();
            IsNew = false;
            PageTitle = Person.Name;
            SelectedSector = Sectors.FirstOrDefault(s => s.Id == Person.SectorId);
        }
    }

    [RelayCommand]
    private async Task SaveAsync(CancellationToken stoppingToken)
    {
        if (string.IsNullOrWhiteSpace(Person.Name))
            return;

        if (SelectedSector is not null)
            Person.SectorId = SelectedSector.Id;

        if (IsNew)
            await personService.InsertAsync(Person);
        else
            await personService.UpdateAsync(Person);

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task DeleteAsync(CancellationToken stoppingToken)
    {
        await personService.DeleteAsync(Person);
        await Shell.Current.GoToAsync("..");
    }
}
