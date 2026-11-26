// The types of the app's Views namespace the view models call: in the app they show CommunityToolkit popups and ask
// for the fingerprint, here the user's answer comes from AppPlatform.
namespace OBD.Mobile.Views
{
    public static class Dialogs
    {
        public static Task AlertAsync(string title, string message, string accept)
        {
            AppPlatform.Alerts.Add((title, message));
            return Task.CompletedTask;
        }

        public static Task<string?> ChooseAsync(string title, string cancel, string? destruction, params string[] choices)
        {
            AppPlatform.ChoicesShown.Add((title, choices));
            return Task.FromResult(AppPlatform.ChooseAnswer(title, choices));
        }
    }

    public static class SensitiveAccess
    {
        public static Task<bool> AuthenticateAsync()
        {
            AppPlatform.BiometricPrompts++;
            return Task.FromResult(AppPlatform.BiometricsSucceed);
        }
    }

    public static class AppColors
    {
        public static Color Get(string key, Color fallback) => fallback;
    }
}

// The pages the view models navigate to, by their name: only the route matters here
namespace OBD.Mobile.Pages.Notes
{
    public sealed class NotesSectorPage;

    public sealed class NoteDetailPage;

    public sealed class SketchDetailsPage;
}

namespace OBD.Mobile.Pages.Persons
{
    public sealed class PersonDetailsPage;
}

namespace OBD.Mobile.Pages.Parameters
{
    public sealed class SectorDetailsPage;
}
