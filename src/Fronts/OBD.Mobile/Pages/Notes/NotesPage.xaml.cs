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
        var isAvailable = await CrossFingerprint.Current.IsAvailableAsync();
        if (!isAvailable)
        {
            await Dialogs.AlertAsync(
                AppResources.GetString("Notes_NotAvailableTitle"),
                AppResources.GetString("Notes_BiometricsUnavailableMessage"),
                AppResources.GetString("Common_Ok"));
            return;
        }

        var request = new AuthenticationRequestConfiguration(
            AppResources.GetString("Notes_SensitiveAuthTitle"),
            AppResources.GetString("Notes_SensitiveAuthReason"));
        var result = await CrossFingerprint.Current.AuthenticateAsync(request);

        if (result.Authenticated)
            await Shell.Current.GoToAsync(nameof(SensitiveNotesPage));
    }
}
