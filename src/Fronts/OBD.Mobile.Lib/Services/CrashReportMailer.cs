namespace OBD.Mobile.Lib.Services;

public class CrashReportMailer : ICrashReportMailer
{
    public async Task SendReportAsync(string subject, string body)
    {
        if (!Email.Default.IsComposeSupported)
            return;

        try
        {
            var message = new EmailMessage(subject, body, [SupportLinks.DeveloperEmail]);
            await Email.Default.ComposeAsync(message);
        }
        catch (FeatureNotSupportedException)
        {
        }
        catch (ArgumentNullException)
        {
        }
    }
}
