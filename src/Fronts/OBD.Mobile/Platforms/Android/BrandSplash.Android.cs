namespace OBD.Mobile.Views;

public static partial class BrandSplash
{
    /// <summary>On the decor view, above the activity's content, at the full size of the window. The content is hidden
    /// from TalkBack meanwhile, so that it reads the splash and not the page underneath; Back skips the splash.</summary>
    private static partial Action? Attach(Window window, BrandSplashView splash, IMauiContext context)
    {
        if (window.Handler?.PlatformView is not global::Android.App.Activity { Window.DecorView: global::Android.Views.ViewGroup decor } activity)
            return null;

        var view = Microsoft.Maui.Platform.ElementExtensions.ToPlatform(splash, context);
        decor.AddView(view, new global::Android.Widget.FrameLayout.LayoutParams(
            global::Android.Views.ViewGroup.LayoutParams.MatchParent, global::Android.Views.ViewGroup.LayoutParams.MatchParent));

        var content = activity.FindViewById(global::Android.Resource.Id.Content);
        // Given back as it was, whatever it was
        var accessibility = content?.ImportantForAccessibility ?? global::Android.Views.ImportantForAccessibility.Auto;
        content?.ImportantForAccessibility = global::Android.Views.ImportantForAccessibility.NoHideDescendants;

        // Added at Window.Created (Activity.OnPostCreate), after MAUI's own callback (MauiAppCompatActivity.OnCreate):
        // Android asks this one first. Tied to the activity, so that it is still asked first when the app comes back
        // from the background. Back never reaches the page nobody sees yet, and never leaves the app from the splash.
        var back = new SkipOnBack(splash);
        if (activity is global::AndroidX.Activity.ComponentActivity component)
            component.OnBackPressedDispatcher.AddCallback(component, back);

        return () =>
        {
            back.Remove();
            decor.RemoveView(view);
            content?.ImportantForAccessibility = accessibility;
        };
    }

    private sealed class SkipOnBack(BrandSplashView splash) : global::AndroidX.Activity.OnBackPressedCallback(true)
    {
        public override void HandleOnBackPressed() => splash.Skip();
    }
}
