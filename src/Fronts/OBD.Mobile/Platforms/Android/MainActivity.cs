using Android.App;
using Android.Content.PM;
using Android.OS;

namespace OBD.Mobile
{
    // A foldable that opens or closes moves the app from one screen to the other: another size, density, colour mode,
    // font scale (1.15 on the outer screen of the Motorola razr) and on some a keyboard or navigation change. Each one
    // not listed here makes Android rebuild the app, which then starts over from its home page with its state lost.
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density | ConfigChanges.ColorMode | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.Navigation | ConfigChanges.FontScale)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CrossFingerprint.SetCurrentActivityResolver(() => Microsoft.Maui.ApplicationModel.Platform.CurrentActivity);
        }
    }
}
