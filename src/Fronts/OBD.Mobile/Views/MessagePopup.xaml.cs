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
    }

    private async void OnAcceptClicked(object? sender, EventArgs e) => await CloseAsync();
}
