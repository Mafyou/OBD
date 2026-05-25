namespace OBD.Mobile.Pages.Persons;

public partial class PersonsPage : ContentPage
{
    private DateTime? _lastBackPressTime;

    public PersonsPage(PersonsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
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

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is PersonsViewModel vm)
            vm.LoadCommand.Execute(null);
    }

    private async void OnSearchClicked(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(SearchPage));
}