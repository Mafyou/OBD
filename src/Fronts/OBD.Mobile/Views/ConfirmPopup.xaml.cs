namespace OBD.Mobile.Views;

public partial class ConfirmPopup : Popup<bool>
{
    public ConfirmPopup(string title, string message, string? confirmText = null, bool isDanger = false)
    {
        InitializeComponent();
        TitleLabel.Text = title;
        MessageLabel.Text = message;
        ConfirmButton.Text = confirmText ?? AppResources.GetString("Common_Confirm");
        ConfirmButton.BackgroundColor = isDanger
            ? (Color)Application.Current!.Resources["DangerColor"]
            : (Color)Application.Current!.Resources["AccentPrimary"];
    }

    private async void OnConfirmClicked(object? sender, EventArgs e) => await CloseAsync(true);
    private async void OnCancelClicked(object? sender, EventArgs e) => await CloseAsync(false);
}
