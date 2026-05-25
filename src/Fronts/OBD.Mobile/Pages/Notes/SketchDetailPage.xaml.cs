namespace OBD.Mobile.Pages.Notes;

public partial class SketchDetailsPage : ContentPage
{
    private readonly SketchDetailsViewModel _vm;
    private bool _isImageCleared;

    public SketchDetailsPage(SketchDetailsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm = vm;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        DrawingCanvas.Lines.Clear();
        _isImageCleared = false;

        Shell.SetBackButtonBehavior(this, new BackButtonBehavior
        {
            Command = new Command(() => _ = HandleBackAsync())
        });
    }

    protected override bool OnBackButtonPressed()
    {
        _ = HandleBackAsync();
        return true;
    }

    private async Task HandleBackAsync()
    {
        if (!_vm.IsEditMode || (DrawingCanvas.Lines.Count == 0 && !_vm.HasBackground && !_isImageCleared))
        {
            await Shell.Current.GoToAsync("..");
            return;
        }

        var popup = new ConfirmPopup(
            "Quitter sans sauvegarder ?",
            "Les modifications seront perdues.");
        var result = await App.Current!.Windows[0].Page!.ShowPopupAsync<bool>(popup, new PopupOptions
        {
            CanBeDismissedByTappingOutsideOfPopup = true,
            Shadow = null,
            Shape = null,
        });
        if (result.WasDismissedByTappingOutsideOfPopup)
            return;
        if (result.Result is true)
            await Shell.Current.GoToAsync("..");
        else
        {
            _vm.EditCommand.Execute(null);
        }
    }

    private async void OnClearClicked(object? sender, EventArgs e)
    {
        if (DrawingCanvas.Lines.Count > 0)
        {
            var popup = new ConfirmPopup(
                "Effacer le dessin ?",
                "Les traits seront supprimés.");
            var result = await App.Current!.Windows[0].Page!.ShowPopupAsync<bool>(popup, new PopupOptions
            {
                CanBeDismissedByTappingOutsideOfPopup = true,
                Shadow = null,
                Shape = null,
            });
            if (result.Result is true)
                DrawingCanvas.Lines.Clear();
            else
                _vm.EditCommand.Execute(null);
        }
        else if (_vm.HasBackground)
        {
            var popup = new ConfirmPopup(
                "Effacer le fond ?",
                "La photo de fond sera supprimée définitivement.",
                isDanger: true);
            var result = await App.Current!.Windows[0].Page!.ShowPopupAsync<bool>(popup, new PopupOptions
            {
                CanBeDismissedByTappingOutsideOfPopup = true,
                Shadow = null,
                Shape = null,
            });
            if (result.Result is true)
            {
                _vm.CurrentImage = null;
                _isImageCleared = true;
            }
            else
                _vm.EditCommand.Execute(null);
        }
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        var popup = new ConfirmPopup(
            "Supprimer ce visuel ?",
            "Cette action est irréversible.",
            confirmText: "Supprimer",
            isDanger: true);
        var result = await App.Current!.Windows[0].Page!.ShowPopupAsync<bool>(popup, new PopupOptions
        {
            CanBeDismissedByTappingOutsideOfPopup = true,
            Shadow = null,
            Shape = null,
        });
        if (result.Result is true && _vm.DeleteCommand is IAsyncRelayCommand asyncCmd)
            await asyncCmd.ExecuteAsync(null);
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (DrawingCanvas.Lines.Count == 0 && _vm.IsNew && !_vm.HasBackground)
        {
            await Shell.Current.GoToAsync("..");
            return;
        }

        if (DrawingCanvas.Lines.Count > 0 || _vm.HasBackground)
        {
            var screenshot = await SketchEditorSurface.CaptureAsync();

            if (screenshot is null)
                return;

            await using var stream = await screenshot.OpenReadAsync();
            await _vm.SaveAsync(stream);
        }
        else
        {
            await _vm.SaveAsync(null, clearImage: _isImageCleared);
        }
    }
}
