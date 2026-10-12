namespace OBD.Mobile.Lib.UnitTests.Diagnostics;

public class CrashExceptionFilterTests
{
    [Theory]
    [InlineData(typeof(TaskCanceledException))]
    [InlineData(typeof(OperationCanceledException))]
    [InlineData(typeof(HttpRequestException))]
    [InlineData(typeof(IOException))]
    [InlineData(typeof(TimeoutException))]
    public void ShouldIgnore_ShouldReturnTrue_ForExpectedTransientExceptions(Type exceptionType)
    {
        var exception = (Exception)Activator.CreateInstance(exceptionType)!;

        CrashExceptionFilter.ShouldIgnore(exception).ShouldBeTrue();
    }

    [Fact]
    public void ShouldIgnore_ShouldReturnTrue_WhenMessageMentionsNavigation()
    {
        var exception = new InvalidOperationException("Navigation to the requested route failed.");

        CrashExceptionFilter.ShouldIgnore(exception).ShouldBeTrue();
    }

    [Fact]
    public void ShouldIgnore_ShouldReturnFalse_ForGenuineCrashes()
    {
        var exception = new NullReferenceException("Object reference not set to an instance of an object.");

        CrashExceptionFilter.ShouldIgnore(exception).ShouldBeFalse();
    }
}
