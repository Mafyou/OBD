namespace OBD.Mobile.ViewModels;

[QueryProperty(nameof(PersonId), "id")]
public partial class PersonDetailsViewModel(IPersonService personneService, ISectorService secteurService) : ObservableObject
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
    public partial string PageTitle { get; set; } = "Nouvelle personne";

    public int PersonId
    {
        set => _ = LoadAsync(value);
    }

    private async Task LoadAsync(int id)
    {
        var secteurs = await secteurService.GetAllAsync();
        Sectors = new ObservableCollection<Sector>(secteurs);
        HasNoSectors = Sectors.Count == 0;

        if (id is 0)
        {
            Person = new Person();
            IsNew = true;
            PageTitle = "Nouvelle personne";
        }
        else
        {
            Person = await personneService.GetAsync(id) ?? new Person();
            IsNew = false;
            PageTitle = Person.Name;
            SelectedSector = Sectors.FirstOrDefault(s => s.Id == Person.SectorId);
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Person.Name))
            return;

        if (SelectedSector is not null)
            Person.SectorId = SelectedSector.Id;

        if (IsNew)
            await personneService.InsertAsync(Person);
        else
            await personneService.UpdateAsync(Person);

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        await personneService.DeleteAsync(Person);
        await Shell.Current.GoToAsync("..");
    }
}
