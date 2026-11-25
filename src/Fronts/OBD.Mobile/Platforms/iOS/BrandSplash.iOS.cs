namespace OBD.Mobile.Views;

public static partial class BrandSplash
{
    /// <summary>On the UIWindow, above the root view controller's view, at the full size of the window. An app that
    /// puts another page in the window meanwhile (the onboarding page giving way to the Shell) may get a new root view
    /// laid above the splash: the splash is brought back to the front each time.</summary>
    private static partial Action? Attach(Window window, BrandSplashView splash, IMauiContext context)
    {
        if (window.Handler?.PlatformView is not UIKit.UIWindow platformWindow) return null;

        var view = Microsoft.Maui.Platform.ElementExtensions.ToPlatform(splash, context);
        view.Frame = platformWindow.Bounds;
        view.AutoresizingMask = UIKit.UIViewAutoresizing.FlexibleWidth | UIKit.UIViewAutoresizing.FlexibleHeight;
        platformWindow.AddSubview(view);

        // The window is created before it is made visible (by the scene delegate, with the UIScene lifecycle): the
        // root view controller's view may be put in it after the splash. Brought to the front once that is done
        MainThread.BeginInvokeOnMainThread(BringToFront);

        void BringToFront() => Safely(() =>
        {
            if (view.Superview == platformWindow) platformWindow.BringSubviewToFront(view);
        }, "The splash could not be brought to the front");

        void KeepInFront(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Window.Page))
                MainThread.BeginInvokeOnMainThread(BringToFront);
        }

        window.PropertyChanged += KeepInFront;

        return () =>
        {
            window.PropertyChanged -= KeepInFront;
            view.RemoveFromSuperview();
        };
    }
}
