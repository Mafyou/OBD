namespace OBD.Mobile.Pages.Persons;

public partial class PersonDetailsPage : ContentPage
{
    public PersonDetailsPage(PersonDetailsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private async void OnGoToParametersClicked(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync("//ParametersPage");
}
