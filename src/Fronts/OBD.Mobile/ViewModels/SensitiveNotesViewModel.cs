namespace OBD.Mobile.ViewModels;

public partial class SensitiveNotesViewModel(INoteService noteService) : ObservableObject
{
    [ObservableProperty]
    public partial ObservableCollection<Note> Notes { get; set; } = [];

    [RelayCommand]
    public async Task LoadAsync()
    {
        var list = await noteService.GetSensiblesAsync();
        Notes = new ObservableCollection<Note>(list);
    }

    [RelayCommand]
    private async Task OpenNoteDetailAsync(Note note)
    {
        var route = note.Type == TypeNote.Sketch ? nameof(SketchDetailsPage) : nameof(NoteDetailPage);
        await Shell.Current.GoToAsync($"{route}?noteid={note.Id}&sectorid={note.SectorId}");
    }

    [RelayCommand]
    private async Task UnmarkSensitiveAsync(Note note)
    {
        note.IsSensitive = false;
        await noteService.UpdateAsync(note);
        Notes.Remove(note);
    }
}
