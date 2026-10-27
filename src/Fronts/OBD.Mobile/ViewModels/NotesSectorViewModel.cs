namespace OBD.Mobile.ViewModels;

[QueryProperty(nameof(SectorId), "sectorid")]
public partial class NotesSectorViewModel(ISectorService sectorService, INoteService noteService) : ObservableObject
{
    [ObservableProperty]
    public partial string SectorName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial ObservableCollection<Note> Notes { get; set; } = [];

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    // The page is shared by every sector: a sector opened right after another must not get the notes of the first
    private readonly LatestRequest _latestLoad = new();
    private int _sectorId;

    public int SectorId
    {
        set
        {
            _sectorId = value;
            _ = LoadAsync(value, _latestLoad.Next());
        }
    }

    private async Task LoadAsync(int id, CancellationToken stoppingToken)
    {
        IsLoading = true;
        var sector = await sectorService.GetAsync(id);
        var list = await noteService.GetBySectorAsync(id);
        if (stoppingToken.IsCancellationRequested)
            return;

        SectorName = sector?.Name ?? string.Empty;
        Notes = [.. list];
        IsLoading = false;
    }

    [RelayCommand]
    private async Task OpenNoteDetailAsync(Note note)
    {
        var route = note.Type is TypeNote.Text ? nameof(NoteDetailPage) : nameof(SketchDetailsPage);
        await Shell.Current.GoToAsync($"{route}?noteid={note.Id}&sectorid={_sectorId}");
    }

    [RelayCommand]
    private async Task OpenNewTextNoteAsync()
        => await Shell.Current.GoToAsync($"{nameof(NoteDetailPage)}?noteid=0&sectorid={_sectorId}");

    [RelayCommand]
    private async Task OpenNewSketchAsync()
        => await Shell.Current.GoToAsync($"{nameof(SketchDetailsPage)}?noteid=0&sectorid={_sectorId}");

    [RelayCommand]
    private async Task MarkSensitiveAsync(Note note, CancellationToken stoppingToken)
    {
        // Only the flag: a note of the list is read without the image of a sketch, and saving it whole would erase it
        await noteService.SetSensitiveAsync(note.Id, isSensitive: true);
        note.IsSensitive = true;
        Notes.Remove(note);
    }
}
