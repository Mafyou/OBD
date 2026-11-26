namespace OBD.Mobile.Lib.IntegrationTests.TestInfrastructure;

/// <summary>
/// A temp folder of its own for one test: the SQLite database and the files the services write (crash markers,
/// export archives) all land there, and the folder goes away with the test.
/// </summary>
public sealed class TempWorkspace : IDisposable
{
    public TempWorkspace()
    {
        Root = Path.Combine(Path.GetTempPath(), "obd-integration-tests", $"{Guid.NewGuid():N}");
        Directory.CreateDirectory(Root);
    }

    public string Root { get; }

    public string DatabasePath => Path.Combine(Root, "obd.db3");

    public string PathOf(string fileName) => Path.Combine(Root, fileName);

    /// <summary>
    /// The services of the app wired as <see cref="ServicesExtension.AddServices"/> does, with the database and the
    /// crash files in this folder instead of <c>FileSystem.AppDataDirectory</c> (not available off a phone).
    /// </summary>
    public ServiceProvider BuildServices()
    {
        ServiceCollection services = new();
        services.AddServices();
        services.AddSingleton(_ => new DatabaseContext(DatabasePath));
        services.AddTransient<DataSeeder>();
        services.Replace(ServiceDescriptor.Singleton<IAppCrashStateService>(_ => new AppCrashStateService(Root)));
        return services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
    }

    /// <summary>
    /// Closes every SQLite connection, as when the app process ends: a <see cref="DatabaseContext"/> built afterwards
    /// opens the file again instead of reusing the pooled connection.
    /// </summary>
    public static void SimulateAppRestart() => SQLiteAsyncConnection.ResetPool();

    public void Dispose()
    {
        SimulateAppRestart();
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
