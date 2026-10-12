namespace OBD.Mobile.Views;

/// <summary>
/// Choice of one action among several in the app's colors, shown through <see cref="Dialogs.ChooseAsync"/>.
/// The destructive action, when present, is displayed in the danger color.
/// </summary>
public partial class ChoicePopup : Popup
{
    /// <summary>
    /// Label of the chosen action, or null when the user cancelled or dismissed the popup.
    /// </summary>
    public string? Choice { get; private set; }

    public ChoicePopup(string title, string cancel, string? destruction, params string[] choices)
    {
        InitializeComponent();
        TitleLabel.Text = title;
        CancelButton.Text = cancel;
        DestructiveButton.Text = destruction;
        DestructiveButton.IsVisible = destruction is not null;
        BindableLayout.SetItemsSource(ChoicesLayout, choices);
    }

    private async void OnChoiceClicked(object? sender, EventArgs e)
    {
        Choice = (sender as Button)?.Text;
        await CloseAsync();
    }

    private async void OnCancelClicked(object? sender, EventArgs e) => await CloseAsync();
}
