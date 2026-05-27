namespace OBD.Mobile;

public partial class App : Application
{
    private readonly DatabaseContext _db;
    private readonly TaskCompletionSource _startupReady = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public Task StartupReady => _startupReady.Task;

    public App(DatabaseContext db)
    {
        InitializeComponent();
        _db = db;
        UserAppTheme = AppTheme.Dark;
    }

    protected override Window CreateWindow(IActivationState? activationState)
        => new(new AppShell());

    protected override async void OnStart()
    {
        base.OnStart();

        try
        {
#if DEBUG
            await new DataSeeder(_db).SeedAsync();
#elif RELEASE
            await new DataSeeder(_db).SeedIfEmptyAsync();
#endif
        }
        finally
        {
            _startupReady.TrySetResult();
        }
    }
}
