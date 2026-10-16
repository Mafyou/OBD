namespace OBD.Mobile.Pages.Persons;

public partial class PersonDetailsPage : ContentPage
{
    private readonly PersonDetailsViewModel _vm;

    public PersonDetailsPage(PersonDetailsViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    private async void OnGoToParametersClicked(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync($"//{nameof(ParametersPage)}");

    // "Delete" used to delete the person at once: it asks first, as for a note or a visual
    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        var popup = new ConfirmPopup(
            AppResources.GetString("PersonDetails_DeleteTitle"),
            AppResources.GetString("Common_IrreversibleAction"),
            confirmText: AppResources.GetString("Common_Delete"),
            isDanger: true);
        var result = await App.Current!.Windows[0].Page!.ShowPopupAsync<bool>(popup, new PopupOptions
        {
            CanBeDismissedByTappingOutsideOfPopup = true,
            Shadow = null,
            Shape = null,
        });
        if (result.Result is true && _vm.DeleteCommand is IAsyncRelayCommand asyncCmd)
            await asyncCmd.ExecuteAsync(null);
    }
}
