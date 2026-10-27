namespace OBD.Mobile.Views;

/// <summary>
/// App dialogs (CommunityToolkit popups), to use instead of the native DisplayAlertAsync and
/// DisplayActionSheetAsync. Confirmations go through <see cref="ConfirmPopup"/>. Call on the main thread.
/// </summary>
public static class Dialogs
{
    /// <summary>
    /// Common options: each popup draws its own rounded surface, so the toolkit adds neither shape nor shadow.
    /// </summary>
    public static PopupOptions Options => new()
    {
        CanBeDismissedByTappingOutsideOfPopup = true,
        Shadow = null,
        Shape = null,
    };

    private static Page Host => Application.Current!.Windows[0].Page!;

    public static async Task AlertAsync(string title, string message, string accept) =>
        await Host.ShowPopupAsync(new MessagePopup(title, message, accept), Options);

    /// <summary>
    /// Shows a <see cref="ConfirmPopup"/>: its result is true on "Confirm", and says whether the user tapped outside instead.
    /// </summary>
    public static async Task<IPopupResult<bool>> ConfirmAsync(ConfirmPopup popup) =>
        await Host.ShowPopupAsync<bool>(popup, Options);

    /// <summary>
    /// Returns the label of the chosen action, or null when the user cancelled.
    /// </summary>
    public static async Task<string?> ChooseAsync(string title, string cancel, string? destruction, params string[] choices)
    {
        var popup = new ChoicePopup(title, cancel, destruction, choices);
        await Host.ShowPopupAsync(popup, Options);
        return popup.Choice;
    }
}
