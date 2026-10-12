namespace OBD.Mobile.Lib.Services;

/// <summary>
/// Stores its marker/crash files under <paramref name="appDataDirectory"/> rather than reading
/// <c>FileSystem.AppDataDirectory</c> itself, so it stays unit-testable with a plain temp directory
/// (mirrors how <see cref="Data.DatabaseContext"/> takes its db path instead of resolving it internally).
/// </summary>
public class AppCrashStateService(string appDataDirectory) : IAppCrashStateService
{
    private const string RunningMarkerFileName = "app-running.marker";
    private const string LastCrashFileName = "last-crash.txt";

    private string RunningMarkerPath => Path.Combine(appDataDirectory, RunningMarkerFileName);
    private string LastCrashPath => Path.Combine(appDataDirectory, LastCrashFileName);

    public void MarkAppStarted() =>
        TryIo(() => File.WriteAllText(RunningMarkerPath, DateTime.UtcNow.ToString("O")));

    public void MarkAppClosedCleanly() =>
        TryIo(() => DeleteIfExists(RunningMarkerPath));

    public bool WasPreviousRunUnclean() => File.Exists(RunningMarkerPath);

    public void RecordCrash(Exception exception, string source) =>
        TryIo(() => File.WriteAllText(LastCrashPath, BuildCrashDetails(exception, source)));

    public string? GetLastCrashDetails() =>
        File.Exists(LastCrashPath) ? File.ReadAllText(LastCrashPath) : null;

    public void ClearCrashData() => TryIo(() =>
    {
        DeleteIfExists(LastCrashPath);
        DeleteIfExists(RunningMarkerPath);
    });

    private static string BuildCrashDetails(Exception exception, string source) => $"""
        Date: {DateTime.UtcNow:O}
        Source: {source}
        Exception: {exception.GetType().FullName}
        Message: {exception.Message}
        StackTrace:
        {exception.StackTrace}
        """;

    private static void DeleteIfExists(string path)
    {
        if (File.Exists(path))
            File.Delete(path);
    }

    /// <summary>
    /// Crash-state I/O must never throw — this code runs from unhandled-exception handlers, where
    /// throwing again would just terminate the process before the original crash gets recorded.
    /// </summary>
    private static void TryIo(Action action)
    {
        try
        {
            action();
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
