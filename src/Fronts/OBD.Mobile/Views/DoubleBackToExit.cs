namespace OBD.Mobile.Views;

/// <summary>
/// Double Back to leave the app. On a root page (one from which Back would leave the app), the first
/// Back press only shows a toast; a second press within <see cref="ConfirmationDelay"/> leaves.
/// Usage, in the root page: <c>protected override bool OnBackButtonPressed() => DoubleBackToExit.Handle();</c>
/// </summary>
public static class DoubleBackToExit
{
    private static readonly TimeSpan ConfirmationDelay = TimeSpan.FromSeconds(2);

    // Shared by every root page: the second press counts even when it lands on another tab.
    private static long? _lastBackPress;

    /// <returns>True when Back was consumed (the toast is shown); false when the app may be left.</returns>
    public static bool Handle()
    {
        var now = Environment.TickCount64;
        if (_lastBackPress is not null && now - _lastBackPress.Value < ConfirmationDelay.TotalMilliseconds)
        {
            _lastBackPress = null;
            return false;
        }

        _lastBackPress = now;
        _ = Toast.Make(AppResources.GetString("Common_PressAgainToExit")).Show();
        return true;
    }
}