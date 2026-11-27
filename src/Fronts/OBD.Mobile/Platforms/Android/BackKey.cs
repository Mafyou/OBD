namespace OBD.Mobile.Views;

/// <summary>
/// Gives the Back key to the app on every page (ported from the shared library of the MIT apps). From Android 16, Back
/// goes to the system unless the app keeps a callback enabled, and MAUI only enables its own while it has something to
/// go back to (a pushed page, a modal page): on a root page Back could leave the app at once without asking the page,
/// or, the page having let it go, not leave at all.
/// </summary>
public static class BackKey
{
    /// <summary>Call once, after base.OnCreate: the callback added last is the one Android asks first.</summary>
    public static void GiveToTheApp(AndroidX.Activity.ComponentActivity activity) =>
        activity.OnBackPressedDispatcher.AddCallback(activity, new Callback(activity));

    /// <summary>
    /// Leaves the app the way Android leaves the first activity of a task: it goes to the background, it is not
    /// destroyed. Handing the press back to the dispatcher instead brought it to the page once more.
    /// </summary>
    public static void Leave(Android.App.Activity activity)
    {
        if (activity.IsTaskRoot)
            activity.MoveTaskToBack(true);
        else
            activity.FinishAfterTransition();
    }

    // Always enabled: Android hands every press on Back to the app. Depending on how MAUI shows the page, this callback
    // is the first to hear of the press or the second, so one press can reach the page twice: DoubleBackGate.SamePress
    private sealed class Callback(AndroidX.Activity.ComponentActivity activity) : AndroidX.Activity.OnBackPressedCallback(true)
    {
        public override void HandleOnBackPressed()
        {
            // During the launch splash, Back opens the app: it neither leaves nor reaches the page underneath
            if (BrandSplash.Skip())
                return;

            try
            {
                // The window hands Back to what is on display, as MAUI does: a popup closes, a pushed page goes back,
                // a root page asks before leaving (DoubleBackToExit)
                if (Application.Current?.Windows.FirstOrDefault() is IWindow window && window.BackButtonClicked())
                    return;
            }
            catch (Exception exception)
            {
                // A page that fails on Back keeps the app open: leaving it here would look like a crash
                Android.Util.Log.Error("OBD", $"[BackKey] The page on display failed to handle Back: {exception}");
                return;
            }

            // Nobody kept it
            Leave(activity);
        }
    }
}
