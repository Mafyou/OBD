namespace OBD.Mobile.Lib.Abstractions;

/// <summary>
/// Tracks whether the previous app run terminated cleanly, and persists crash details across launches
/// so they can be reported (e.g. by email) the next time the app starts.
/// </summary>
public interface IAppCrashStateService
{
    /// <summary>Writes a marker file indicating the app is running. Call once at startup.</summary>
    void MarkAppStarted();

    /// <summary>Removes the running marker. Call when the app is shutting down cleanly.</summary>
    void MarkAppClosedCleanly();

    /// <summary>True when the running marker from a previous launch is still present, meaning the app did not shut down cleanly.</summary>
    bool WasPreviousRunUnclean();

    /// <summary>Persists details about an unhandled exception so they survive the process crashing.</summary>
    void RecordCrash(Exception exception, string source);

    /// <summary>Returns the details of the last recorded crash, or null when none were recorded.</summary>
    string? GetLastCrashDetails();

    /// <summary>Deletes any persisted crash details and running marker.</summary>
    void ClearCrashData();
}
