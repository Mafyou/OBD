namespace OBD.Mobile.Lib.UnitTests.Services;

public class AppCrashStateServiceTests : IDisposable
{
    private readonly string _tempDirectory = Path.Combine(Path.GetTempPath(), $"obd-crash-tests-{Guid.NewGuid():N}");

    public AppCrashStateServiceTests() => Directory.CreateDirectory(_tempDirectory);

    public void Dispose()
    {
        if (Directory.Exists(_tempDirectory))
            Directory.Delete(_tempDirectory, recursive: true);
    }

    [Fact]
    public void WasPreviousRunUnclean_ShouldBeFalse_WhenNoMarkerExists()
    {
        var service = new AppCrashStateService(_tempDirectory);

        service.WasPreviousRunUnclean().ShouldBeFalse();
    }

    [Fact]
    public void WasPreviousRunUnclean_ShouldBeTrue_AfterMarkAppStarted()
    {
        var service = new AppCrashStateService(_tempDirectory);

        service.MarkAppStarted();

        service.WasPreviousRunUnclean().ShouldBeTrue();
    }

    [Fact]
    public void WasPreviousRunUnclean_ShouldBeFalse_AfterMarkAppClosedCleanly()
    {
        var service = new AppCrashStateService(_tempDirectory);
        service.MarkAppStarted();

        service.MarkAppClosedCleanly();

        service.WasPreviousRunUnclean().ShouldBeFalse();
    }

    [Fact]
    public void GetLastCrashDetails_ShouldBeNull_WhenNoCrashWasRecorded()
    {
        var service = new AppCrashStateService(_tempDirectory);

        service.GetLastCrashDetails().ShouldBeNull();
    }

    [Fact]
    public void RecordCrash_ShouldPersistExceptionDetails()
    {
        var service = new AppCrashStateService(_tempDirectory);
        var exception = new InvalidOperationException("Boom");

        service.RecordCrash(exception, "UnitTest.Source");

        var details = service.GetLastCrashDetails();
        details.ShouldNotBeNull();
        details.ShouldContain("Boom");
        details.ShouldContain("UnitTest.Source");
        details.ShouldContain(nameof(InvalidOperationException));
    }

    [Fact]
    public void ClearCrashData_ShouldRemoveCrashDetailsAndRunningMarker()
    {
        var service = new AppCrashStateService(_tempDirectory);
        service.MarkAppStarted();
        service.RecordCrash(new Exception("Boom"), "UnitTest.Source");

        service.ClearCrashData();

        service.GetLastCrashDetails().ShouldBeNull();
        service.WasPreviousRunUnclean().ShouldBeFalse();
    }
}
