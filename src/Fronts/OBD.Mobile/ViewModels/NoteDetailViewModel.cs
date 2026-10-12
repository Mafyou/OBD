namespace OBD.Mobile.ViewModels;

public partial class NoteDetailViewModel(INoteService noteService, INoteLinkService noteLinkService) : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    public partial Note Note { get; set; } = new();

    [ObservableProperty]
    public partial bool IsNew { get; set; } = true;

    [ObservableProperty]
    public partial string PageTitle { get; set; } = AppResources.GetString("NoteDetail_NewTitle");

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

    // Shell applies the query of the route again each time the page comes back to the front: a popup closed,
    // a pushed page popped. Only the first application chooses the note, or the one on screen would be replaced.
    private bool _isQueryApplied;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (_isQueryApplied)
        {
            // Back from a linked sketch: it may have been renamed or deleted
            if (!IsNew)
                _ = RefreshLinksAndRenderAsync();
            return;
        }

        _isQueryApplied = true;

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
            PageTitle = AppResources.GetString("NoteDetail_NewTitle");
            Links = [];
            HasLinks = false;
        }
        else
        {
            Note = await noteService.GetAsync(noteId) ?? new Note { SectorId = sectorId };
            IsNew = false;
            PageTitle = AppResources.GetString("NoteDetail_Title");
            await RefreshLinksAsync();
            IsViewMode = true;
            IsEditMode = false;
            ContentRendered?.Invoke();
        }
    }

    private async Task RefreshLinksAndRenderAsync()
    {
        await RefreshLinksAsync();
        ContentRendered?.Invoke();
    }

    private async Task RefreshLinksAsync()
    {
        var rawLinks = await noteLinkService.GetByNoteAsync(Note.Id);
        var allNotes = await noteService.GetAllAsync();
        var visualMap = allNotes
            .Where(n => n.Type != TypeNote.Text)
            .ToDictionary(n => n.Id);

        foreach (var link in rawLinks)
            link.LinkedTitle = visualMap.TryGetValue(link.SketchId, out var s) ? s.DisplayText : AppResources.GetString("NoteDetail_DefaultVisualName");

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
    private async Task SaveAsync(CancellationToken stoppingToken)
    {
        if (IsNew)
        {
            await noteService.InsertAsync(Note);
            IsNew = false;
            PageTitle = AppResources.GetString("NoteDetail_Title");
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
    private async Task DeleteAsync(CancellationToken stoppingToken)
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

        var allNotes = await noteService.GetAllAsync();
        var visuals = allNotes.Where(n => n.Type != TypeNote.Text).ToList();

        if (visuals.Count == 0)
        {
            await Dialogs.AlertAsync(
                AppResources.GetString("NoteDetail_NoVisualTitle"),
                AppResources.GetString("NoteDetail_NoVisualMessage"),
                AppResources.GetString("Common_Ok"));
            return;
        }

        var cancel = AppResources.GetString("Common_Cancel");
        var options = visuals.Select(s => s.DisplayText).ToArray();
        var chosen = await Dialogs.ChooseAsync(AppResources.GetString("NoteDetail_LinkPromptFormat", word), cancel, null, options);

        if (string.IsNullOrEmpty(chosen) || chosen == cancel)
            return;

        var visual = visuals.FirstOrDefault(s => s.DisplayText == chosen);
        if (visual is null) return;

        await noteLinkService.InsertAsync(new NoteLink { NoteId = Note.Id, Word = word, SketchId = visual.Id });
        await RefreshLinksAsync();
        ContentRendered?.Invoke();
    }

    [RelayCommand]
    private static async Task OpenLinkedSketchAsync(NoteLink link)
        => await Shell.Current.GoToAsync($"{nameof(SketchDetailsPage)}?noteid={link.SketchId}&sectorid=0");

    [RelayCommand]
    private async Task RemoveLinkAsync(NoteLink link, CancellationToken stoppingToken)
    {
        await noteLinkService.DeleteAsync(link);
        Links.Remove(link);
        HasLinks = Links.Count > 0;
        ContentRendered?.Invoke();
    }
}
