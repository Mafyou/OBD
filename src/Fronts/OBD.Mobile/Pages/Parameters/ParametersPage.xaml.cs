namespace OBD.Mobile.Pages.Parameters;

public partial class ParametersPage : ContentPage
{
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

    private async void OnSearchClicked(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(SearchPage));

    private async void OnPrivacyPolicyClicked(object? sender, TappedEventArgs e)
        => await Launcher.OpenAsync("http://mafyouit.tech/apps/OBD");

    private async void OnRepoClicked(object? sender, TappedEventArgs e)
        => await Launcher.OpenAsync("https://github.com/Mafyou/OBD");

    private async void OnPromoPageClicked(object? sender, TappedEventArgs e)
        => await Launcher.OpenAsync("http://mafyouit.tech/apps/Promo/OBD");
}
