namespace OBD.Mobile.Pages.Persons;

public partial class PersonDetailsPage : ContentPage
{
    public PersonDetailsPage(PersonDetailsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
