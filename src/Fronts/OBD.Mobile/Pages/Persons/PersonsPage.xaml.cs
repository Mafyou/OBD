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
}