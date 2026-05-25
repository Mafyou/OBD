namespace OBD.Mobile.Pages.Notes;

public partial class NoteDetailPage : ContentPage
{
    public NoteDetailPage(NoteDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
