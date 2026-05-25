namespace OBD.Mobile.Pages.Parameters;

public partial class SectorDetailsPage : ContentPage
{
    public SectorDetailsPage(SectorDetailsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
