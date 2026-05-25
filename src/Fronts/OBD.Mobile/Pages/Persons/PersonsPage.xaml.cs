namespace OBD.Mobile.Pages.Persons;

public partial class PersonsPage : ContentPage
{
    public PersonsPage(PersonsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
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