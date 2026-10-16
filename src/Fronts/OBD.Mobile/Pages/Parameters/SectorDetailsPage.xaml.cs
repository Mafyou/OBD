namespace OBD.Mobile.Pages.Parameters;

public partial class SectorDetailsPage : ContentPage
{
    private readonly SectorDetailsViewModel _vm;

    public SectorDetailsPage(SectorDetailsViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    // "Delete" used to delete the sector at once: it asks first, as for a note or a visual
    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        var popup = new ConfirmPopup(
            AppResources.GetString("SectorDetails_DeleteTitle"),
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
