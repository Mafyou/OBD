namespace OBD.Mobile.ViewModels;

public partial class NoteDetailViewModel(INoteService noteService) : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    public partial Note Note { get; set; } = new();

    [ObservableProperty]
    public partial bool IsNew { get; set; } = true;

    [ObservableProperty]
    public partial string PageTitle { get; set; } = "Nouvelle note";

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        int noteId = 0;
        int sectorId = 0;

        if (query.TryGetValue("noteid", out var nVal) && int.TryParse(nVal?.ToString(), out int nId))
            noteId = nId;
        if (query.TryGetValue("sectorid", out var sVal) && int.TryParse(sVal?.ToString(), out int sId))
            sectorId = sId;

        _ = LoadAsync(noteId, sectorId);
    }

    private async Task LoadAsync(int noteId, int sectorId)
    {
        if (noteId is 0)
        {
            Note = new Note { SectorId = sectorId };
            IsNew = true;
            PageTitle = "Nouvelle note";
        }
        else
        {
            Note = await noteService.GetAsync(noteId) ?? new Note { SectorId = sectorId };
            IsNew = false;
            PageTitle = "Note";
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsNew)
            await noteService.InsertAsync(Note);
        else
            await noteService.UpdateAsync(Note);
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        await noteService.DeleteAsync(Note);
        await Shell.Current.GoToAsync("..");
    }
}
