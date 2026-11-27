namespace OBD.Mobile.Pages.Parameters;

public partial class SectorDetailsPage : ContentPage
{
    private readonly SectorDetailsViewModel _vm;

    public SectorDetailsPage(SectorDetailsViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    // "Delete" used to delete the sector at once: it asks first, as for a note or a visual, and says how many notes and
    // people go with it
    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        var contents = await _vm.GetContentsAsync();
        var popup = new ConfirmPopup(
            AppResources.GetString("SectorDetails_DeleteTitle"),
            contents.DeleteMessage(),
            confirmText: AppResources.GetString("Common_Delete"),
            isDanger: true);
        var result = await Dialogs.ConfirmAsync(popup);
        if (result.Result is true && _vm.DeleteCommand is IAsyncRelayCommand asyncCmd)
            await asyncCmd.ExecuteAsync(null);
    }
}
