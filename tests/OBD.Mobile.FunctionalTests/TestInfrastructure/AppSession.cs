namespace OBD.Mobile.FunctionalTests.TestInfrastructure;

/// <summary>
/// One run of the app for a test: the services of OBD.Mobile.Lib wired as on the phone, with the database and the
/// files in a temp folder of its own, the clock under the test's control, and the view models built the way the
/// pages get them. Only the platform is fake (<see cref="AppPlatform"/>, the crash report mailer).
/// </summary>
public sealed class AppSession : IDisposable
{
    private readonly ServiceProvider _provider;

    public AppSession()
    {
        Root = Path.Combine(Path.GetTempPath(), "obd-functional-tests", $"{Guid.NewGuid():N}");
        Directory.CreateDirectory(Root);
        var cache = Path.Combine(Root, "cache");
        Directory.CreateDirectory(cache);
        AppPlatform.Reset(cache);

        ServiceCollection services = new();
        services.AddServices();
        services.AddSingleton(_ => new DatabaseContext(Path.Combine(Root, "obd.db3")));
        services.AddTransient<DataSeeder>();
        services.Replace(ServiceDescriptor.Singleton<TimeProvider>(Clock));
        services.Replace(ServiceDescriptor.Singleton<IAppCrashStateService>(_ => new AppCrashStateService(Root)));
        services.Replace(ServiceDescriptor.Singleton(Mailer.Object));

        // As PagesViewModelsExtension registers them; built here on each request so a test can open a page twice
        services.AddTransient<OnboardingViewModel>();
        services.AddTransient<NotesViewModel>();
        services.AddTransient<NotesSectorViewModel>();
        services.AddTransient<NoteDetailViewModel>();
        services.AddTransient<SketchDetailsViewModel>();
        services.AddTransient<SensitiveNotesViewModel>();
        services.AddTransient<PersonsViewModel>();
        services.AddTransient<PersonDetailsViewModel>();
        services.AddTransient<SectorDetailsViewModel>();
        services.AddTransient<SearchViewModel>();
        services.AddTransient<ParametersViewModel>();
        services.AddTransient<LogsViewModel>();

        _provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true });
    }

    public string Root { get; }

    public FakeTimeProvider Clock { get; } = new();

    public Mock<ICrashReportMailer> Mailer { get; } = new();

    public T Get<T>() where T : notnull => _provider.GetRequiredService<T>();

    /// <summary>The route of the last navigation the view models asked Shell for.</summary>
    public static string? LastRoute => AppPlatform.Routes.LastOrDefault();

    public void Dispose()
    {
        _provider.Dispose();
        SQLiteAsyncConnection.ResetPool();
        try
        {
            Directory.Delete(Root, recursive: true);
        }
        catch (IOException)
        {
            // A file still held by the OS: the temp folder is cleaned up later
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
