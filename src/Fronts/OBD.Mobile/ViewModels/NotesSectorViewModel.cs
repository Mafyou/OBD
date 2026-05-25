namespace OBD.Mobile.ViewModels;

[QueryProperty(nameof(SectorId), "sectorid")]
public partial class NotesSectorViewModel(ISectorService secteurService, INoteService noteService) : ObservableObject
{
    [ObservableProperty]
    public partial string SectorName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial ObservableCollection<Note> Notes { get; set; } = [];

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    private int _sectorId;

    public int SectorId
    {
        set
        {
            _sectorId = value;
            _ = LoadAsync(value);
        }
    }

    private async Task LoadAsync(int id)
    {
        IsLoading = true;
        var secteur = await secteurService.GetAsync(id);
        SectorName = secteur?.Name ?? string.Empty;
        var list = await noteService.GetBySecteurAsync(id);
        Notes = new ObservableCollection<Note>(list);
        IsLoading = false;
    }

    [RelayCommand]
    private async Task OpenNoteDetailAsync(Note note)
    {
        var route = note.Type == TypeNote.Text ? nameof(NoteDetailPage) : nameof(SketchDetailsPage);
        await Shell.Current.GoToAsync($"{route}?noteid={note.Id}&sectorid={_sectorId}");
    }

    [RelayCommand]
    private async Task OpenNewTextNoteAsync()
        => await Shell.Current.GoToAsync($"{nameof(NoteDetailPage)}?noteid=0&sectorid={_sectorId}");

    [RelayCommand]
    private async Task OpenNewSketchAsync()
        => await Shell.Current.GoToAsync($"{nameof(SketchDetailsPage)}?noteid=0&sectorid={_sectorId}");

    [RelayCommand]
    private async Task MarkSensitiveAsync(Note note)
    {
        note.IsSensitive = true;
        await noteService.UpdateAsync(note);
        Notes.Remove(note);
    }
}
