namespace OBD.Mobile.Pages.Notes;

public partial class NotesSectorPage : ContentPage
{
    public NotesSectorPage(NotesSectorViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
