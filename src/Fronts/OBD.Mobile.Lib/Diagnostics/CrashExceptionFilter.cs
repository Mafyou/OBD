namespace OBD.Mobile.Lib.Diagnostics;

/// <summary>
/// Decides which exceptions are worth reporting as a crash. Filters out exceptions that are expected
/// during normal operation (cancellation, transient network/IO failures, navigation races) to avoid
/// false-positive crash reports.
/// </summary>
public static class CrashExceptionFilter
{
    public static bool ShouldIgnore(Exception exception) => exception switch
    {
        OperationCanceledException => true, // TaskCanceledException among them
        HttpRequestException => true,
        IOException => true,
        TimeoutException => true,
        _ => IsNavigationException(exception),
    };

    private static bool IsNavigationException(Exception exception)
    {
        var typeName = exception.GetType().FullName ?? string.Empty;
        return typeName.Contains("Navigation", StringComparison.OrdinalIgnoreCase)
            || exception.Message.Contains("navigat", StringComparison.OrdinalIgnoreCase);
    }
}
