namespace OBD.Mobile.ViewModels;

public partial class OnboardingViewModel(DatabaseContext db) : ObservableObject
{
    public const string OnboardingCompletedKey = "onboarding_completed";

    public event Action? OnboardingCompleted;

    [RelayCommand]
    private async Task ChoosePrefilledAsync(CancellationToken stoppingToken)
    {
        await new DataSeeder(db).SeedAsync();
        Complete();
    }

    [RelayCommand]
    private Task ChooseEmptyAsync(CancellationToken stoppingToken)
    {
        Complete();
        return Task.CompletedTask;
    }

    private void Complete()
    {
        Preferences.Default.Set(OnboardingCompletedKey, true);
        OnboardingCompleted?.Invoke();
    }
}