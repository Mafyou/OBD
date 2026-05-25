namespace OBD.Mobile.ViewModels;

public partial class PersonsViewModel(IPersonService personneService) : ObservableObject
{
    [ObservableProperty]
    public partial ObservableCollection<Person> Persons { get; set; } = [];

    [ObservableProperty]
    public partial string SearchQuery { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [RelayCommand]
    private async Task LoadAsync(CancellationToken stoppingToken)
    {
        IsLoading = true;
        var list = string.IsNullOrWhiteSpace(SearchQuery)
            ? await personneService.GetAllAsync()
            : await personneService.SearchAsync(SearchQuery);
        Persons = new ObservableCollection<Person>(list);
        IsLoading = false;
    }

    [RelayCommand]
    private static async Task OpenDetailAsync(Person? personne)
    {
        int id = personne?.Id ?? 0;
        await Shell.Current.GoToAsync($"{nameof(PersonDetailsPage)}?id={id}");
    }

    partial void OnSearchQueryChanged(string value) => LoadCommand.Execute(null);
}
