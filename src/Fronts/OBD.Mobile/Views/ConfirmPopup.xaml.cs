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

        // Around the message: the paddings (48), the title (30), the buttons and their margin (48), the spaces (40);
        // measured on the window of the app, again each time it changes size (a foldable closed, a phone turned)
        ScreenFit.FollowWindow(this, () =>
            MessageScroll.MaximumHeightRequest = PopupFit.ScrollableHeight(ScreenFit.WindowHeight(), fixedHeight: 166, preferred: double.PositiveInfinity));
    }

    private async void OnConfirmClicked(object? sender, EventArgs e) => await CloseAsync(true);
    private async void OnCancelClicked(object? sender, EventArgs e) => await CloseAsync(false);
}
