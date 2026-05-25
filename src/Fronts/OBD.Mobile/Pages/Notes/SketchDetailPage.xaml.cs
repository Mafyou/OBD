namespace OBD.Mobile.Pages.Notes;

public partial class SketchDetailsPage : ContentPage
{
    private readonly SketchDetailsViewModel _vm;

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
    }

    private void OnClearClicked(object? sender, EventArgs e)
        => DrawingCanvas.Lines.Clear();

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (DrawingCanvas.Lines.Count == 0 && _vm.IsNew)
            return;

        if (DrawingCanvas.Lines.Count > 0)
        {
            var screenshot = await SketchEditorSurface.CaptureAsync();

            if (screenshot is null)
                return;

            await using var stream = await screenshot.OpenReadAsync();
            await _vm.SaveAsync(stream);
        }
        else
        {
            await _vm.SaveAsync(null);
        }
    }
}
