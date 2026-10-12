namespace OBD.Mobile.Pages.Diagnostics;

public partial class LogsPage : ContentPage
{
    public LogsPage(LogsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is LogsViewModel vm)
            vm.LoadCommand.Execute(null);
    }
}
