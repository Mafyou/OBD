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

        // Sized from the window of the app, again each time it changes size: on the outer screen of a foldable
        // (410 units tall) a list 320 units high pushed "Cancel" below the bottom edge
        ScreenFit.FollowWindow(this, () => ChoicesScroll.MaximumHeightRequest = PopupFit.ScrollableHeight(
            ScreenFit.WindowHeight(),
            fixedHeight: DestructiveButton.IsVisible ? AroundListWithDestruction : AroundList,
            preferred: PreferredListHeight));
    }

    private const double PreferredListHeight = 320;

    // Around the list: the paddings (48), the title (30), "Cancel" (44) and the spaces between them (40)
    private const double AroundList = 162;

    // The same with the destructive action and the space above it
    private const double AroundListWithDestruction = AroundList + 64;

    private async void OnChoiceClicked(object? sender, EventArgs e)
    {
        Choice = (sender as Button)?.Text;
        await CloseAsync();
    }

    private async void OnCancelClicked(object? sender, EventArgs e) => await CloseAsync();
}
