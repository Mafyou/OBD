namespace OBD.Mobile.FunctionalTests.Platform;

/// <summary>
/// What the phone would do around the view models, recorded for the tests: the routes Shell was asked to go to, the
/// popups shown and the answer the user gives them, the preferences written, the files shared. The static fakes of
/// the view model and view namespaces (<see cref="ViewModels.Shell"/>, <see cref="Views.Dialogs"/>…) all read and
/// write here; <see cref="Reset"/> gives each test a phone of its own.
/// </summary>
public static class AppPlatform
{
    public static List<string> Routes { get; } = [];

    public static List<(string Title, string Message)> Alerts { get; } = [];

    public static List<(string Title, string[] Choices)> ChoicesShown { get; } = [];

    /// <summary>The label the user taps in a choice popup (title, choices); null for a popup dismissed.</summary>
    public static Func<string, string[], string?> ChooseAnswer { get; set; } = (_, _) => null;

    public static bool BiometricsSucceed { get; set; } = true;

    public static int BiometricPrompts { get; set; }

    public static Dictionary<string, object?> Preferences { get; } = [];

    public static List<ShareFileRequest> SharedFiles { get; } = [];

    /// <summary>Copies of the files shared, read while they still exist (the next export deletes the previous one).</summary>
    public static List<byte[]> SharedFileContents { get; } = [];

    public static List<string> OpenedUris { get; } = [];

    public static string CacheDirectory { get; set; } = Path.GetTempPath();

    public static Func<Task<FileResult?>> CapturePhoto { get; set; } = () => Task.FromResult<FileResult?>(null);

    public static Func<Task<List<FileResult>>> PickPhotos { get; set; } = () => Task.FromResult<List<FileResult>>([]);

    public static void Reset(string cacheDirectory)
    {
        Routes.Clear();
        Alerts.Clear();
        ChoicesShown.Clear();
        ChooseAnswer = (_, _) => null;
        BiometricsSucceed = true;
        BiometricPrompts = 0;
        Preferences.Clear();
        SharedFiles.Clear();
        SharedFileContents.Clear();
        OpenedUris.Clear();
        CacheDirectory = cacheDirectory;
        CapturePhoto = () => Task.FromResult<FileResult?>(null);
        PickPhotos = () => Task.FromResult<List<FileResult>>([]);
    }
}
