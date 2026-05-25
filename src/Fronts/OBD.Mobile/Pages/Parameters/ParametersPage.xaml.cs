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
}
