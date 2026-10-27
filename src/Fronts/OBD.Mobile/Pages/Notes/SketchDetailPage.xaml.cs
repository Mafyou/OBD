namespace OBD.Mobile.Pages.Notes;

public partial class SketchDetailsPage : ContentPage
{
    private readonly SketchDetailsViewModel _vm;
    private bool _isImageCleared;
    private bool _isCanvasReady;

    public SketchDetailsPage(SketchDetailsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm = vm;

        ScreenFit.FollowWindow(this, PlaceEditTools);
    }

    // Width of the column of buttons beside the canvas on a short window
    private const double SideToolsWidth = 184;

    /// <summary>
    /// The buttons of the edit mode under the canvas, or beside it on a short window (the outer screen of a foldable,
    /// 480 x 410 units): under it they took all of its height there, and Save was below the bottom edge.
    /// </summary>
    private void PlaceEditTools()
    {
        var isShort = ScreenFit.IsShort();

        EditGrid.RowDefinitions = isShort
            ? [new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Star)]
            : [new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Star), new RowDefinition(GridLength.Auto)];
        EditGrid.ColumnDefinitions = isShort
            ? [new ColumnDefinition(GridLength.Star), new ColumnDefinition(SideToolsWidth)]
            : [new ColumnDefinition(GridLength.Star)];

        Grid.SetRow(EditTools, isShort ? 0 : 2);
        Grid.SetRowSpan(EditTools, isShort ? 2 : 1);
        Grid.SetColumn(EditTools, isShort ? 1 : 0);
        EditToolsLayout.Padding = isShort ? new Thickness(0, 12, 16, 16) : new Thickness(16, 0, 16, 16);
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        // Raised again when a popup of this page closes: the drawing in progress has to survive "Cancel"
        if (_isCanvasReady)
            return;

        _isCanvasReady = true;
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
            AppResources.GetString("SketchDetail_DiscardTitle"),
            AppResources.GetString("SketchDetail_DiscardMessage"));
        var result = await Dialogs.ConfirmAsync(popup);
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
                AppResources.GetString("SketchDetail_ClearDrawingTitle"),
                AppResources.GetString("SketchDetail_ClearDrawingMessage"));
            var result = await Dialogs.ConfirmAsync(popup);
            if (result.Result is true)
                DrawingCanvas.Lines.Clear();
            else
                _vm.EditCommand.Execute(null);
        }
        else if (_vm.HasBackground)
        {
            var popup = new ConfirmPopup(
                AppResources.GetString("SketchDetail_ClearBackgroundTitle"),
                AppResources.GetString("SketchDetail_ClearBackgroundMessage"),
                isDanger: true);
            var result = await Dialogs.ConfirmAsync(popup);
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
            AppResources.GetString("SketchDetail_DeleteTitle"),
            AppResources.GetString("Common_IrreversibleAction"),
            confirmText: AppResources.GetString("Common_Delete"),
            isDanger: true);
        var result = await Dialogs.ConfirmAsync(popup);
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
