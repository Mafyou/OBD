namespace OBD.Mobile.ViewModels;

public partial class SensitiveNotesViewModel(INoteService noteService) : ObservableObject
{
    [ObservableProperty]
    public partial ObservableCollection<Note> Notes { get; set; } = [];

    [RelayCommand]
    public async Task LoadAsync(CancellationToken stoppingToken)
    {
        var list = await noteService.GetSensitiveAsync();
        Notes = [.. list];
    }

    [RelayCommand]
    private async Task OpenNoteDetailAsync(Note note)
    {
        // As everywhere else: anything but a text opens in the sketch page (a photo showed its base64 as text)
        var route = note.Type is TypeNote.Text ? nameof(NoteDetailPage) : nameof(SketchDetailsPage);
        await Shell.Current.GoToAsync($"{route}?noteid={note.Id}&sectorid={note.SectorId}");
    }

    [RelayCommand]
    private async Task UnmarkSensitiveAsync(Note note, CancellationToken stoppingToken)
    {
        // Only the flag: a note of the list is read without the image of a sketch, and saving it whole would erase it
        await noteService.SetSensitiveAsync(note.Id, isSensitive: false);
        note.IsSensitive = false;
        Notes.Remove(note);
    }
}
