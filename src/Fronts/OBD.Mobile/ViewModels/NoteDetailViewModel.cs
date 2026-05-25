namespace OBD.Mobile.ViewModels;

public partial class NoteDetailViewModel(INoteService noteService, INoteLinkService noteLinkService) : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    public partial Note Note { get; set; } = new();

    [ObservableProperty]
    public partial bool IsNew { get; set; } = true;

    [ObservableProperty]
    public partial string PageTitle { get; set; } = "Nouvelle note";

    [ObservableProperty]
    public partial ObservableCollection<NoteLink> Links { get; set; } = [];

    [ObservableProperty]
    public partial bool HasLinks { get; set; }

    [ObservableProperty]
    public partial bool IsViewMode { get; set; }

    [ObservableProperty]
    public partial bool IsEditMode { get; set; } = true;

    // Code-behind subscribes to re-render #hashtag spans
    public event Action? ContentRendered;

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
            IsViewMode = false;
            IsEditMode = true;
            PageTitle = "Nouvelle note";
            Links = [];
            HasLinks = false;
        }
        else
        {
            Note = await noteService.GetAsync(noteId) ?? new Note { SectorId = sectorId };
            IsNew = false;
            PageTitle = "Note";
            await RefreshLinksAsync();
            IsViewMode = true;
            IsEditMode = false;
            ContentRendered?.Invoke();
        }
    }

    private async Task RefreshLinksAsync()
    {
        var rawLinks = await noteLinkService.GetByNoteAsync(Note.Id);
        var sketches = await noteService.GetAllAsync();
        var sketchMap = sketches.Where(n => n.Type == TypeNote.Sketch).ToDictionary(n => n.Id);

        foreach (var link in rawLinks)
            link.SketchTitle = sketchMap.TryGetValue(link.SketchId, out var s) ? s.DisplayText : "Croquis";

        Links = new ObservableCollection<NoteLink>(rawLinks);
        HasLinks = Links.Count > 0;
    }

    [RelayCommand]
    private void Edit()
    {
        IsViewMode = false;
        IsEditMode = true;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsNew)
        {
            await noteService.InsertAsync(Note);
            IsNew = false;
            PageTitle = "Note";
        }
        else
        {
            await noteService.UpdateAsync(Note);
        }

        await RefreshLinksAsync();
        IsViewMode = true;
        IsEditMode = false;
        ContentRendered?.Invoke();
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        await noteService.DeleteAsync(Note);
        await Shell.Current.GoToAsync("..");
    }

    // Called from code-behind when user taps a #word span
    [RelayCommand]
    public async Task TapHashtagAsync(string word)
    {
        var existing = Links.FirstOrDefault(l => l.Word.Equals(word, StringComparison.OrdinalIgnoreCase));
        if (existing is not null)
        {
            await Shell.Current.GoToAsync($"{nameof(SketchDetailsPage)}?noteid={existing.SketchId}&sectorid=0");
            return;
        }

        var allSketches = await noteService.GetAllAsync();
        var sketches = allSketches.Where(n => n.Type == TypeNote.Sketch).ToList();

        if (sketches.Count == 0)
        {
            await Shell.Current.DisplayAlertAsync("Aucun croquis", "Créez d'abord un croquis dans ce secteur.", "OK");
            return;
        }

        var options = sketches.Select(s => s.DisplayText).ToArray();
        var chosen = await Shell.Current.DisplayActionSheetAsync($"Lier #{word} à :", "Annuler", null, options);

        if (string.IsNullOrEmpty(chosen) || chosen == "Annuler")
            return;

        var sketch = sketches.FirstOrDefault(s => s.DisplayText == chosen);
        if (sketch is null) return;

        await noteLinkService.InsertAsync(new NoteLink { NoteId = Note.Id, Word = word, SketchId = sketch.Id });
        await RefreshLinksAsync();
        ContentRendered?.Invoke();
    }

    [RelayCommand]
    private static async Task OpenLinkedSketchAsync(NoteLink link)
        => await Shell.Current.GoToAsync($"{nameof(SketchDetailsPage)}?noteid={link.SketchId}&sectorid=0");

    [RelayCommand]
    private async Task RemoveLinkAsync(NoteLink link)
    {
        await noteLinkService.DeleteAsync(link);
        Links.Remove(link);
        HasLinks = Links.Count > 0;
        ContentRendered?.Invoke();
    }
}
