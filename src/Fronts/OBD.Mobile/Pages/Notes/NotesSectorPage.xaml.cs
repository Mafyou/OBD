namespace OBD.Mobile.Pages.Notes;

public partial class NotesSectorPage : ContentPage
{
    public NotesSectorPage(NotesSectorViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private void OnNoteLongPressed(object? sender, LongPressCompletedEventArgs e)
    {
        if (sender is View view
            && view.BindingContext is Note note
            && BindingContext is NotesSectorViewModel vm)
        {
            vm.MarkSensitiveCommand.Execute(note);
        }
    }
}
