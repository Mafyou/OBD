namespace OBD.Mobile;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        UserAppTheme = AppTheme.Dark;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var services = IPlatformApplication.Current!.Services;
        bool onboardingDone = Preferences.Default.Get(OnboardingViewModel.OnboardingCompletedKey, false);
        Page startPage = onboardingDone
            ? services.GetRequiredService<AppShell>()
            : services.GetRequiredService<OnboardingPage>();
        return new Window(startPage);
    }
}
