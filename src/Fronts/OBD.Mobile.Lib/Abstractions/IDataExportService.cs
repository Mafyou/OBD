namespace OBD.Mobile.Lib.Abstractions;

public interface IDataExportService
{
    Task<bool> HasSensitiveNotesAsync();

    /// <summary>
    /// Writes a ZIP archive of the whole notebook to <paramref name="destination"/>, which stays open.
    /// Sensitive notes, and the links that lead to them, are left out unless <paramref name="includeSensitive"/> is set.
    /// </summary>
    Task WriteArchiveAsync(Stream destination, bool includeSensitive, CancellationToken stoppingToken);
}
