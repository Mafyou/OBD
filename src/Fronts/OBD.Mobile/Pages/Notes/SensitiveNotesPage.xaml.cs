namespace OBD.Mobile.Pages.Notes;

public partial class SensitiveNotesPage : ContentPage
{
    public SensitiveNotesPage(SensitiveNotesViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is SensitiveNotesViewModel vm)
            vm.LoadCommand.Execute(null);
    }

    private async void OnNoteTapped(object? sender, TappedEventArgs e)
    {
        if (sender is View view
            && view.BindingContext is Note note
            && BindingContext is SensitiveNotesViewModel vm)
        {
            await vm.OpenNoteDetailCommand.ExecuteAsync(note);
        }
    }

    private void OnNoteLongPressed(object? sender, LongPressCompletedEventArgs e)
    {
        if (sender is View view
            && view.BindingContext is Note note
            && BindingContext is SensitiveNotesViewModel vm)
        {
            vm.UnmarkSensitiveCommand.Execute(note);
        }
    }
}
