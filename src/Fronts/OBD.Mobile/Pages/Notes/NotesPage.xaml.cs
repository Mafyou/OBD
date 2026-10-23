namespace OBD.Mobile.Pages.Notes;

public partial class NotesPage : ContentPage
{
    public NotesPage(NotesViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is NotesViewModel vm)
            vm.LoadSectorsCommand.Execute(null);
    }

    protected override bool OnBackButtonPressed() => DoubleBackToExit.Handle();

    private async void OnSearchClicked(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(SearchPage));

    private async void OnSensitiveClicked(object? sender, EventArgs e)
    {
        if (await SensitiveAccess.AuthenticateAsync())
            await Shell.Current.GoToAsync(nameof(SensitiveNotesPage));
    }
}
