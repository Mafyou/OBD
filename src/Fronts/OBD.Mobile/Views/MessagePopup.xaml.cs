namespace OBD.Mobile.Views;

/// <summary>
/// One-button message in the app's colors, shown through <see cref="Dialogs.AlertAsync"/>.
/// </summary>
public partial class MessagePopup : Popup
{
    public MessagePopup(string title, string message, string accept)
    {
        InitializeComponent();
        TitleLabel.Text = title;
        MessageLabel.Text = message;
        AcceptButton.Text = accept;

        // Around the message: the paddings (48), the title (30), the button and its margin (48), the spaces (40);
        // measured on the window of the app, again each time it changes size (a foldable closed, a phone turned)
        ScreenFit.FollowWindow(this, () =>
            MessageScroll.MaximumHeightRequest = PopupFit.ScrollableHeight(ScreenFit.WindowHeight(), fixedHeight: 166, preferred: double.PositiveInfinity));
    }

    private async void OnAcceptClicked(object? sender, EventArgs e) => await CloseAsync();
}
