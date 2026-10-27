namespace OBD.Mobile.ViewModels;

public partial class SearchViewModel(
    IPersonService personService,
    INoteService noteService,
    ISectorService sectorService,
    TimeProvider timeProvider) : ObservableObject
{
    // Typing goes faster than this: the search runs once the user pauses, not for each letter
    private static readonly TimeSpan TypingPause = TimeSpan.FromMilliseconds(250);

    private readonly LatestRequest _latestSearch = new();

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

    partial void OnSearchQueryChanged(string value) => _ = SearchAsync(value, _latestSearch.Next());

    private async Task SearchAsync(string query, CancellationToken stoppingToken)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            Results = [];
            HasResults = false;
            HasSearched = false;
            HasNoResults = false;
            IsSearching = false;
            return;
        }

        IsSearching = true;
        HasSearched = true;

        try
        {
            await Task.Delay(TypingPause, timeProvider, stoppingToken);

            var personsTask = personService.SearchAsync(query);
            var notesTask = noteService.SearchAsync(query);
            var sectorsTask = sectorService.SearchAsync(query);
            var (persons, notes, sectors) = (await personsTask, await notesTask, await sectorsTask);

            // A newer search started meanwhile: its results are the ones to show
            if (stoppingToken.IsCancellationRequested)
                return;

            var sectorSubtitle = AppResources.GetString("Search_SectorSubtitle");
            List<SearchResult> items = new(persons.Count + notes.Count + sectors.Count);
            items.AddRange(persons.Select(p => new SearchResult(ResultType.Person, p.Name, p.Position, p.Id, 0, TypeNote.Text)));
            items.AddRange(notes.Select(n => new SearchResult(ResultType.Note, n.DisplayText, n.Keywords, n.Id, n.SectorId, n.Type)));
            items.AddRange(sectors.Select(s => new SearchResult(ResultType.Sector, s.Name, sectorSubtitle, s.Id, s.Id, TypeNote.Text)));

            Results = [.. items];
            HasResults = items.Count > 0;
            HasNoResults = items.Count is 0;
            IsSearching = false;
        }
        catch (OperationCanceledException)
        {
            // Replaced by the next search, which updates the screen
        }
    }

    [RelayCommand]
    private static async Task OpenResultAsync(SearchResult item)
    {
        var route = item switch
        {
            { ResultType: ResultType.Person } => $"{nameof(PersonDetailsPage)}?id={item.Id}",
            { ResultType: ResultType.Sector } => $"{nameof(NotesSectorPage)}?sectorid={item.Id}",
            { NoteType: TypeNote.Text } => $"{nameof(NoteDetailPage)}?noteid={item.Id}&sectorid={item.SectorId}",
            _ => $"{nameof(SketchDetailsPage)}?noteid={item.Id}&sectorid={item.SectorId}",
        };
        await Shell.Current.GoToAsync(route);
    }
}