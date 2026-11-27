#if ANDROID
namespace OBD.Mobile.Lib.Resources.Strings;

/// <summary>
/// The language picked for this app under Settings, App languages (Android 13+), or the language of the phone. A cold
/// start gets it, but a change made while the app runs (in the background behind Settings, most of the time) only
/// hands the app a new configuration: its texts were resolved at start-up, so it stayed in its former language until
/// it was closed (seen on a phone). The app then starts again, in the new language.
/// </summary>
internal static class AppLanguageTags
{
    private const string LogTag = "AppLanguage";

    private static LanguageWatcher? _watcher;

    /// <summary>
    /// The language tags of the app ("en-US"), empty or null when it follows the phone or below Android 13. To call at
    /// start-up, before any text is resolved: it also starts watching the language.
    /// </summary>
    public static string? Read()
    {
        try
        {
            var context = global::Android.App.Application.Context;
            WatchTheLanguage(context);
            if (!OperatingSystem.IsAndroidVersionAtLeast(33)) return null;

            var tags = context.GetSystemService(global::Android.Content.Context.LocaleService) is global::Android.App.LocaleManager manager
                ? manager.ApplicationLocales.ToLanguageTags()
                : null;
            global::Android.Util.Log.Info(LogTag, $"App language \"{tags}\"");
            return tags;
        }
        catch (Exception exception)
        {
            // Never stop the start-up for this: the app stays in the language of the phone
            global::Android.Util.Log.Warn(LogTag, $"App language not read: {exception.Message}");
            return null;
        }
    }

    private static void WatchTheLanguage(global::Android.Content.Context context)
    {
        if (_watcher is not null || LanguageOf(context.Resources?.Configuration) is not { } language) return;

        _watcher = new LanguageWatcher(language);
        context.RegisterComponentCallbacks(_watcher);
    }

    private static string? LanguageOf(global::Android.Content.Res.Configuration? configuration) =>
        OperatingSystem.IsAndroidVersionAtLeast(24) && configuration is { Locales.IsEmpty: false }
            ? configuration.Locales.Get(0)?.ToLanguageTag()
            : null;

    private static void StartAgain(global::Android.Content.Context context)
    {
        var process = new global::Android.App.ActivityManager.RunningAppProcessInfo();
        global::Android.App.ActivityManager.GetMyMemoryState(process);

        // On screen (the language changed from adb, or from the app): it opens again at once. Behind Settings, Android
        // does not let it open a screen: the process ends, and the app starts in the new language when the user returns.
        if (process.Importance == global::Android.App.Importance.Foreground
            && context.PackageManager?.GetLaunchIntentForPackage(context.PackageName ?? string.Empty)?.Component is { } launcher)
        {
            context.StartActivity(global::Android.Content.Intent.MakeRestartActivityTask(launcher));
        }

        global::Android.OS.Process.KillProcess(global::Android.OS.Process.MyPid());
    }

    private sealed class LanguageWatcher(string startLanguage) : Java.Lang.Object, global::Android.Content.IComponentCallbacks
    {
        public void OnConfigurationChanged(global::Android.Content.Res.Configuration newConfig)
        {
            var language = LanguageOf(newConfig);
            if (language is null || language == startLanguage) return;

            global::Android.Util.Log.Info(LogTag, $"Language {startLanguage} changed to {language}: the app starts again");
            StartAgain(global::Android.App.Application.Context);
        }

        public void OnLowMemory()
        {
        }
    }
}
#endif
