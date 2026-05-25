namespace OBD.Mobile.Pages.Parameters;

public partial class ParametersPage : ContentPage
{
    private DateTime? _lastBackPressTime;

    public ParametersPage(ParametersViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ParametersViewModel vm)
            vm.LoadCommand.Execute(null);
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

    private async void OnPrivacyPolicyClicked(object? sender, TappedEventArgs e)
        => await Launcher.OpenAsync("http://mafyouit.tech/apps/OBD");

    private async void OnRepoClicked(object? sender, TappedEventArgs e)
        => await Launcher.OpenAsync("https://github.com/Mafyou/OBD");

    private async void OnPromoPageClicked(object? sender, TappedEventArgs e)
        => await Launcher.OpenAsync("http://mafyouit.tech/apps/Promo/OBD");
}
