namespace OBD.Mobile.Pages.Notes;

public partial class NotesPage : ContentPage
{
    private DateTime? _lastBackPressTime;

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

    protected override bool OnBackButtonPressed()
    {
        var now = DateTime.UtcNow;
        if (_lastBackPressTime.HasValue && (now - _lastBackPressTime.Value).TotalSeconds < 2)
        {
            _lastBackPressTime = null;
            return false;
        }
        _lastBackPressTime = now;
        _ = Toast.Make("Appuyez à nouveau pour quitter").Show();
        return true;
    }

    private async void OnSearchClicked(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(SearchPage));

    private async void OnSensitiveClicked(object? sender, EventArgs e)
    {
        var isAvailable = await CrossFingerprint.Current.IsAvailableAsync();
        if (!isAvailable)
        {
            await DisplayAlertAsync("Non disponible", "L'authentification biométrique n'est pas disponible sur cet appareil.", "OK");
            return;
        }

        var request = new AuthenticationRequestConfiguration("Notes sensibles", "Authentifiez-vous pour accéder aux notes sensibles");
        var result = await CrossFingerprint.Current.AuthenticateAsync(request);

        if (result.Authenticated)
            await Shell.Current.GoToAsync(nameof(SensitiveNotesPage));
    }
}
