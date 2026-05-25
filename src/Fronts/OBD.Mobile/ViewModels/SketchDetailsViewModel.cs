namespace OBD.Mobile.ViewModels;

public partial class SketchDetailsViewModel(INoteService noteService) : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    public partial Note Note { get; set; } = new();

    [ObservableProperty]
    public partial bool IsNew { get; set; } = true;

    [ObservableProperty]
    public partial string PageTitle { get; set; } = "Nouveau croquis";

    [ObservableProperty]
    public partial ImageSource? CurrentImage { get; set; }

    [ObservableProperty]
    public partial bool HasBackground { get; set; }

    partial void OnCurrentImageChanged(ImageSource? value)
    {
        HasBackground = value is not null;
        OnPropertyChanged(nameof(CanClear));
    }

    public bool CanClear => HasBackground;

    [ObservableProperty]
    public partial Color LineColor { get; set; } = Colors.White;

    [ObservableProperty]
    public partial string SelectedColorName { get; set; } = "White";

    [RelayCommand]
    private void SetLineColor(string colorName)
    {
        SelectedColorName = colorName;
        LineColor = colorName switch
        {
            "Green" => Color.FromArgb("#10B981"),
            "Red" => Color.FromArgb("#EF4444"),
            _ => Colors.White,
        };
    }

    [ObservableProperty]
    public partial bool IsViewMode { get; set; }

    [ObservableProperty]
    public partial bool IsEditMode { get; set; } = true;

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
            Note = new Note { SectorId = sectorId, Type = TypeNote.Sketch };
            IsNew = true;
            IsViewMode = false;
            IsEditMode = true;
            PageTitle = "Nouveau croquis";
        }
        else
        {
            Note = await noteService.GetAsync(noteId) ?? new Note { SectorId = sectorId, Type = TypeNote.Sketch };
            IsNew = false;
            PageTitle = string.IsNullOrEmpty(Note.Title) ? "Croquis" : Note.Title;

            IsViewMode = true;
            IsEditMode = false;

            if (!string.IsNullOrEmpty(Note.Content))
            {
                var bytes = Convert.FromBase64String(Note.Content);
                CurrentImage = ImageSource.FromStream(() => new MemoryStream(bytes));
            }
        }
    }

    [RelayCommand]
    private void Edit()
    {
        IsViewMode = false;
        IsEditMode = true;
    }

    public async Task SaveAsync(Stream? imageStream, bool clearImage = false)
    {
        if (imageStream is not null)
        {
            using var mem = new MemoryStream();
            await imageStream.CopyToAsync(mem);
            Note.Content = Convert.ToBase64String(mem.ToArray());
        }
        else if (clearImage)
        {
            Note.Content = string.Empty;
        }

        Note.Type = TypeNote.Sketch;
        PageTitle = string.IsNullOrEmpty(Note.Title) ? "Croquis" : Note.Title;

        if (IsNew)
            await noteService.InsertAsync(Note);
        else
            await noteService.UpdateAsync(Note);

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task TakePhotoAsBackgroundAsync()
    {
        try
        {
            var photo = await MediaPicker.CapturePhotoAsync();
            await LoadPhotoAsBackgroundAsync(photo);
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlertAsync("Erreur", "Impossible d'accéder à la caméra.", "OK");
        }
    }

    [RelayCommand]
    private async Task PickPhotoAsBackgroundAsync()
    {
        try
        {
            var photos = await MediaPicker.PickPhotosAsync();
            await LoadPhotoAsBackgroundAsync(photos.FirstOrDefault());
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlertAsync("Erreur", "Impossible d'accéder à la galerie.", "OK");
        }
    }

    private async Task LoadPhotoAsBackgroundAsync(FileResult? photo)
    {
        if (photo is null) return;
        using var stream = await photo.OpenReadAsync();
        using var mem = new MemoryStream();
        await stream.CopyToAsync(mem);
        var bytes = mem.ToArray();
        CurrentImage = ImageSource.FromStream(() => new MemoryStream(bytes));
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        await noteService.DeleteAsync(Note);
        await Shell.Current.GoToAsync("..");
    }
}
