namespace OBD.Mobile.Lib.UnitTests.Threading;

public class LatestRequestTests
{
    [Fact]
    public void Next_ShouldCancelThePreviousRequestOnly()
    {
        using var latest = new LatestRequest();

        var first = latest.Next();
        var second = latest.Next();

        first.IsCancellationRequested.ShouldBeTrue();
        second.IsCancellationRequested.ShouldBeFalse();
    }

    [Fact]
    public void Cancel_ShouldCancelTheRequestInProgress()
    {
        using var latest = new LatestRequest();
        var token = latest.Next();

        latest.Cancel();

        token.IsCancellationRequested.ShouldBeTrue();
    }

    [Fact]
    public async Task Next_ShouldStopAWaitOfThePreviousRequest()
    {
        using var latest = new LatestRequest();
        var waiting = Task.Delay(Timeout.InfiniteTimeSpan, latest.Next());

        _ = latest.Next();

        await Should.ThrowAsync<TaskCanceledException>(waiting);
    }
}