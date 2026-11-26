namespace OBD.Mobile.FunctionalTests.TestInfrastructure;

/// <summary>
/// Waits for what a page would see once a load started in the background is over: Shell hands its query to a view
/// model through a property setter or <see cref="IQueryAttributable"/>, which start the load without awaiting it.
/// </summary>
public static class Eventually
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

    public static async Task HappensAsync(Func<bool> condition, string because)
    {
        var stoppingToken = TestContext.Current.CancellationToken;
        var deadline = DateTime.UtcNow + Timeout;
        while (!condition())
        {
            if (DateTime.UtcNow > deadline)
                throw new ShouldAssertException($"Still waiting after {Timeout.TotalSeconds} s: {because}");
            await Task.Delay(10, stoppingToken);
        }
    }
}
