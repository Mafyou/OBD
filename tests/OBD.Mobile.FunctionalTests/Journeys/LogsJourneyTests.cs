namespace OBD.Mobile.FunctionalTests.Journeys;

/// <summary>Logs page: after a crash, the user reads what happened, sends the report and clears it.</summary>
[Collection(AppCollection.Name)]
public class LogsJourneyTests
{
    [Fact]
    public void Logs_ShouldShowTheLastCrash_RecordedByThePreviousRun()
    {
        // Arrange
        using var app = new AppSession();
        new AppCrashStateService(app.Root).RecordCrash(new InvalidOperationException("Sketch too large"), "MainThread");
        var logs = app.Get<LogsViewModel>();

        // Act
        logs.LoadCommand.Execute(null);

        // Assert
        logs.HasCrashDetails.ShouldBeTrue();
        logs.CrashDetails.ShouldContain("Sketch too large");
        logs.CrashDetails.ShouldContain("Source: MainThread");
    }

    [Fact]
    public void Logs_ShouldSayNoCrashWasRecorded()
    {
        // Arrange
        using var app = new AppSession();
        var logs = app.Get<LogsViewModel>();

        // Act
        logs.LoadCommand.Execute(null);

        // Assert
        logs.HasCrashDetails.ShouldBeFalse();
        logs.CrashDetails.ShouldBe(AppResources.GetString("Logs_NoCrashRecorded"));
    }

    [Fact]
    public async Task Report_ShouldBeMailedWithTheCrashAndTheDevice()
    {
        // Arrange
        using var app = new AppSession();
        app.Get<IAppCrashStateService>().RecordCrash(new TimeoutException("Database locked"), "TaskScheduler");
        var logs = app.Get<LogsViewModel>();
        logs.LoadCommand.Execute(null);
        string? body = null;
        app.Mailer.Setup(m => m.SendReportAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Callback<string, string>((_, b) => body = b)
            .Returns(Task.CompletedTask);

        // Act
        await logs.SendReportCommand.ExecuteAsync(null);

        // Assert
        app.Mailer.Verify(m => m.SendReportAsync(AppResources.GetString("Logs_EmailSubject"), It.IsAny<string>()), Times.Once);
        body.ShouldNotBeNull();
        body.ShouldContain("App: OBD 2.8 (24)");
        body.ShouldContain("Device: Contoso Phone 1");
        body.ShouldContain("Database locked");
        logs.IsBusy.ShouldBeFalse();
    }

    [Fact]
    public async Task Report_ShouldFreeThePage_WhenTheMailCannotBeComposed()
    {
        // Arrange
        using var app = new AppSession();
        app.Mailer.Setup(m => m.SendReportAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("No mail app"));
        var logs = app.Get<LogsViewModel>();

        // Act
        var thrown = await Should.ThrowAsync<InvalidOperationException>(() => logs.SendReportCommand.ExecuteAsync(null));

        // Assert
        thrown.Message.ShouldBe("No mail app");
        logs.IsBusy.ShouldBeFalse();
    }

    [Fact]
    public void ClearedCrash_ShouldBeGone_ForThisRunAndTheNext()
    {
        // Arrange
        using var app = new AppSession();
        var crashState = app.Get<IAppCrashStateService>();
        crashState.MarkAppStarted();
        crashState.RecordCrash(new InvalidOperationException("Boom"), "MainThread");
        var logs = app.Get<LogsViewModel>();
        logs.LoadCommand.Execute(null);

        // Act
        logs.ClearCrashDataCommand.Execute(null);

        // Assert
        logs.HasCrashDetails.ShouldBeFalse();
        logs.CrashDetails.ShouldBe(AppResources.GetString("Logs_NoCrashRecorded"));
        var nextRun = new AppCrashStateService(app.Root);
        nextRun.WasPreviousRunUnclean().ShouldBeFalse();
        nextRun.GetLastCrashDetails().ShouldBeNull();
    }

    [Fact]
    public async Task Help_ShouldOpenTheHelpCenter()
    {
        // Arrange
        using var app = new AppSession();
        var logs = app.Get<LogsViewModel>();

        // Act
        await logs.OpenHelpCommand.ExecuteAsync(null);

        // Assert
        AppPlatform.OpenedUris.ShouldBe([SupportLinks.HelpUrl]);
    }
}
