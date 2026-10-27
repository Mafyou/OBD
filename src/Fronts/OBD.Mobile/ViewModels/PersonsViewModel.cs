namespace OBD.Mobile.ViewModels;

public partial class PersonsViewModel(IPersonService personService, ISectorService sectorService) : ObservableObject
{
    // Each letter typed in the filter reloads the list: only the latest load reaches the screen
    private readonly LatestRequest _latestLoad = new();

    [ObservableProperty]
    public partial ObservableCollection<Person> Persons { get; set; } = [];

    [ObservableProperty]
    public partial string SearchQuery { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [RelayCommand(AllowConcurrentExecutions = true)]
    private async Task LoadAsync()
    {
        var stoppingToken = _latestLoad.Next();
        IsLoading = true;

        var query = SearchQuery;
        var list = string.IsNullOrWhiteSpace(query)
            ? await personService.GetAllAsync()
            : await personService.SearchAsync(query);
        var sectors = await sectorService.GetAllAsync();

        if (stoppingToken.IsCancellationRequested)
            return;

        var sectorsById = sectors.ToDictionary(s => s.Id, s => s.Name);
        foreach (var person in list)
            person.SectorName = sectorsById.GetValueOrDefault(person.SectorId, string.Empty);

        Persons = [.. list];
        IsLoading = false;
    }

    [RelayCommand]
    private static async Task OpenDetailAsync(Person? person)
    {
        int id = person?.Id ?? 0;
        await Shell.Current.GoToAsync($"{nameof(PersonDetailsPage)}?id={id}");
    }

    partial void OnSearchQueryChanged(string value) => LoadCommand.Execute(null);
}