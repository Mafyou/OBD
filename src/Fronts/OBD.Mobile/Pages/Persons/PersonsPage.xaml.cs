namespace OBD.Mobile.Pages.Persons;

public partial class PersonsPage : ContentPage
{
    public PersonsPage(PersonsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override bool OnBackButtonPressed() => DoubleBackToExit.Handle();

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is not PersonsViewModel vm) return;

        vm.LoadCommand.Execute(null);
        // At launch the list is on display, filled, as the splash goes: not its empty state first
        if (vm.LoadCommand.ExecutionTask is { } loading)
            BrandSplash.HoldUntil(loading);
    }

    private async void OnSearchClicked(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(SearchPage));
}