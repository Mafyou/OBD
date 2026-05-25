namespace OBD.Mobile;

public partial class App : Application
{
    public App(DatabaseContext db)
    {
        InitializeComponent();
        UserAppTheme = AppTheme.Dark;
#if DEBUG
        _ = new DataSeeder(db).SeedAsync();
#endif
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}