namespace OBD.Mobile.ViewModels;

public partial class SearchViewModel(IPersonService personService, INoteService noteService) : ObservableObject
{
    [ObservableProperty]
    public partial string SearchQuery { get; set; } = string.Empty;

    [ObservableProperty]
    public partial ObservableCollection<SearchResult> Results { get; set; } = [];

    [ObservableProperty]
    public partial bool HasResults { get; set; }

    [ObservableProperty]
    public partial bool HasSearched { get; set; }

    [ObservableProperty]
    public partial bool HasNoResults { get; set; }

    [ObservableProperty]
    public partial bool IsSearching { get; set; }

    partial void OnSearchQueryChanged(string value) => _ = SearchAsync(value);

    private async Task SearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            Results = [];
            HasResults = false;
            HasSearched = false;
            HasNoResults = false;
            return;
        }

        IsSearching = true;
        HasSearched = true;

        var persons = await personService.SearchAsync(query);
        var notes = await noteService.SearchAsync(query);

        var items = new List<SearchResult>();
        items.AddRange(persons.Select(p =>
            new SearchResult(ResultType.Person, p.Name, p.Position, p.Id, 0, TypeNote.Text)));
        items.AddRange(notes.Select(n =>
            new SearchResult(ResultType.Note, n.DisplayText, n.Keywords, n.Id, n.SectorId, n.Type)));

        Results = new ObservableCollection<SearchResult>(items);
        HasResults = items.Count > 0;
        HasNoResults = items.Count == 0;
        IsSearching = false;
    }

    [RelayCommand]
    private static async Task OpenResultAsync(SearchResult item)
    {
        if (item.ResultType == ResultType.Person)
            await Shell.Current.GoToAsync($"{nameof(PersonDetailsPage)}?id={item.Id}");
        else if (item.NoteType == TypeNote.Sketch)
            await Shell.Current.GoToAsync($"{nameof(SketchDetailsPage)}?noteid={item.Id}&sectorid={item.SectorId}");
        else
            await Shell.Current.GoToAsync($"{nameof(NoteDetailPage)}?noteid={item.Id}&sectorid={item.SectorId}");
    }
}
