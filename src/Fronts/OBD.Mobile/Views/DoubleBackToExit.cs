namespace OBD.Mobile.Views;

/// <summary>
/// Double Back to leave the app. On a root page (one from which Back would leave the app), the first
/// Back press only shows a toast; a second press within <see cref="DoubleBackGate.Window"/> leaves (the rules:
/// <see cref="DoubleBackGate"/>).
/// Usage, in the root page: <c>protected override bool OnBackButtonPressed() => DoubleBackToExit.Handle();</c>
/// </summary>
public static class DoubleBackToExit
{
    // Shared by every root page: the second press counts even when it lands on another tab.
    private static readonly DoubleBackGate Gate = new(TimeProvider.System);

    /// <summary>
    /// Always true: Back is consumed. The confirming press leaves the app itself: from Android 16 on, MAUI 11 no longer
    /// leaves when a root page lets Back go, and every press then showed the toast again (seen on a phone).
    /// </summary>
    public static bool Handle()
    {
        switch (Gate.Press())
        {
            case BackPress.Ask:
                _ = Toast.Make(AppResources.GetString("Common_PressAgainToExit")).Show();
                break;
            case BackPress.Leave:
                LeaveApp();
                break;
            case BackPress.SamePress:
                // Already leaving: the same press, asked a second time
                break;
            default:
                throw new System.Diagnostics.UnreachableException();
        }

        return true;
    }

    private static void LeaveApp()
    {
#if ANDROID
        if (Microsoft.Maui.ApplicationModel.Platform.CurrentActivity is { } activity)
            BackKey.Leave(activity);
#endif
    }
}
