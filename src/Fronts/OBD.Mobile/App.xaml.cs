namespace OBD.Mobile;

public partial class App : Application
{
    private readonly IAppCrashStateService _crashStateService;
    private readonly ICrashReportMailer _crashReportMailer;
    private readonly IShakeDetectionService _shakeDetectionService;

    public App(
        IAppCrashStateService crashStateService,
        ICrashReportMailer crashReportMailer,
        IShakeDetectionService shakeDetectionService)
    {
        _crashStateService = crashStateService;
        _crashReportMailer = crashReportMailer;
        _shakeDetectionService = shakeDetectionService;

        LocalizationResourceManager.Instance.RestoreSavedCulture();
        InitializeComponent();
        UserAppTheme = AppTheme.Dark;

        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        _shakeDetectionService.DoubleShakeDetected += OnDoubleShakeDetected;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var services = IPlatformApplication.Current!.Services;
        bool onboardingDone = Preferences.Default.Get(OnboardingViewModel.OnboardingCompletedKey, false);
        Page startPage = onboardingDone
            ? services.GetRequiredService<AppShell>()
            : services.GetRequiredService<OnboardingPage>();

        var window = new Window(startPage);
        window.Destroying += (_, _) => _crashStateService.MarkAppClosedCleanly();
        return window;
    }

    protected override void OnStart()
    {
        base.OnStart();

        bool wasUnclean = _crashStateService.WasPreviousRunUnclean();
        string? pendingCrashDetails = wasUnclean ? _crashStateService.GetLastCrashDetails() : null;

        _crashStateService.MarkAppStarted();
        _shakeDetectionService.Start();

        if (!string.IsNullOrWhiteSpace(pendingCrashDetails))
            _ = SendPendingCrashReportAsync(pendingCrashDetails);
    }

    protected override void OnSleep()
    {
        base.OnSleep();
        _shakeDetectionService.Stop();
    }

    protected override void OnResume()
    {
        base.OnResume();
        _shakeDetectionService.Start();
    }

    private async Task SendPendingCrashReportAsync(string details)
    {
        await _crashReportMailer.SendReportAsync(AppResources.GetString("CrashReport_EmailSubject"), details);
        _crashStateService.ClearCrashData();
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception exception && !CrashExceptionFilter.ShouldIgnore(exception))
            _crashStateService.RecordCrash(exception, "AppDomain.UnhandledException");
    }

    private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        if (!CrashExceptionFilter.ShouldIgnore(e.Exception))
            _crashStateService.RecordCrash(e.Exception, "TaskScheduler.UnobservedTaskException");
        e.SetObserved();
    }

    private void OnDoubleShakeDetected(object? sender, EventArgs e) =>
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            if (Shell.Current is not null)
                await Shell.Current.GoToAsync(nameof(LogsPage));
        });
}
