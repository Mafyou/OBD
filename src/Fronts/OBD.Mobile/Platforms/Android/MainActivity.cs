namespace OBD.Mobile;

// A foldable that opens or closes moves the app from one screen to the other: another size, density, colour mode,
// font scale (1.15 on the outer screen of the Motorola razr) and on some a keyboard or navigation change. Each one
// not listed here makes Android rebuild the app, which then starts over from its home page with its state lost.
// Android types are written in full: imported globally, Android.App.Application would clash with the one of MAUI.
[Android.App.Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    LaunchMode = Android.Content.PM.LaunchMode.SingleTop,
    ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.Orientation
        | Android.Content.PM.ConfigChanges.UiMode | Android.Content.PM.ConfigChanges.ScreenLayout
        | Android.Content.PM.ConfigChanges.SmallestScreenSize | Android.Content.PM.ConfigChanges.Density
        | Android.Content.PM.ConfigChanges.ColorMode | Android.Content.PM.ConfigChanges.Keyboard
        | Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Navigation
        | Android.Content.PM.ConfigChanges.FontScale)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Android.OS.Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        CrossFingerprint.SetCurrentActivityResolver(() => Microsoft.Maui.ApplicationModel.Platform.CurrentActivity);
    }
}
