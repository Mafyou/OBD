namespace OBD.Mobile.ViewModels;

public partial class LogsViewModel(IAppCrashStateService crashStateService, ICrashReportMailer crashReportMailer) : ObservableObject
{
    [ObservableProperty]
    public partial string CrashDetails { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool HasCrashDetails { get; set; }

    [ObservableProperty]
    public partial bool IsBusy { get; set; }

    [RelayCommand]
    private void Load()
    {
        var details = crashStateService.GetLastCrashDetails();
        HasCrashDetails = !string.IsNullOrWhiteSpace(details);
        CrashDetails = HasCrashDetails ? details! : AppResources.GetString("Logs_NoCrashRecorded");
    }

    [RelayCommand]
    private async Task SendReportAsync(CancellationToken stoppingToken)
    {
        IsBusy = true;
        try
        {
            var body = $"""
                App: OBD {AppInfo.Current.VersionString} ({AppInfo.Current.BuildString})
                Platform: {DeviceInfo.Current.Platform} {DeviceInfo.Current.VersionString}
                Device: {DeviceInfo.Current.Manufacturer} {DeviceInfo.Current.Model}
                Date: {DateTime.UtcNow:O}

                {CrashDetails}
                """;
            await crashReportMailer.SendReportAsync(AppResources.GetString("Logs_EmailSubject"), body);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void ClearCrashData()
    {
        crashStateService.ClearCrashData();
        Load();
    }

    [RelayCommand]
    private static async Task OpenHelpAsync(CancellationToken stoppingToken)
        => await Launcher.OpenAsync(SupportLinks.HelpUrl);
}
