namespace OBD.Mobile.Pages.Onboarding;

public partial class OnboardingPage : ContentPage
{
    public OnboardingPage(OnboardingViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        vm.OnboardingCompleted += OnCompleted;
    }

    private async void OnCompleted()
    {
        if (Application.Current is null) return;
        Application.Current.Windows[0].Page = IPlatformApplication.Current!.Services.GetRequiredService<AppShell>();
    }
}