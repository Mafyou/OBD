namespace OBD.Mobile.Views;

/// <summary>
/// Biometric check in front of the sensitive notes, wherever they are shown or leave the app.
/// </summary>
public static class SensitiveAccess
{
    /// <summary>
    /// True once the user is authenticated; says so when the device has no biometrics.
    /// </summary>
    public static async Task<bool> AuthenticateAsync()
    {
        var isAvailable = await CrossFingerprint.Current.IsAvailableAsync();
        if (!isAvailable)
        {
            await Dialogs.AlertAsync(
                AppResources.GetString("Notes_NotAvailableTitle"),
                AppResources.GetString("Notes_BiometricsUnavailableMessage"),
                AppResources.GetString("Common_Ok"));
            return false;
        }

        var request = new AuthenticationRequestConfiguration(
            AppResources.GetString("Notes_SensitiveAuthTitle"),
            AppResources.GetString("Notes_SensitiveAuthReason"));
        var result = await CrossFingerprint.Current.AuthenticateAsync(request);
        return result.Authenticated;
    }
}
