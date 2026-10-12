namespace OBD.Mobile.Lib.Abstractions;

/// <summary>Sends a crash or diagnostic report to the developer via the device's email client.</summary>
public interface ICrashReportMailer
{
    Task SendReportAsync(string subject, string body);
}
