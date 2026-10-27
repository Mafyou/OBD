namespace OBD.Mobile.Lib.Threading;

/// <summary>
/// Keeps only the latest of requests that follow one another, a search typed letter by letter: each new one cancels
/// the one before, so an older and slower answer can no longer replace the newer one on screen.
/// </summary>
public sealed class LatestRequest : IDisposable
{
    private readonly Lock _gate = new();
    private CancellationTokenSource? _current;

    /// <summary>Cancels the previous request and returns the token of the new one.</summary>
    public CancellationToken Next()
    {
        lock (_gate)
        {
            CancelCurrent();
            _current = new CancellationTokenSource();
            return _current.Token;
        }
    }

    /// <summary>Cancels the request in progress, if any.</summary>
    public void Cancel()
    {
        lock (_gate)
        {
            CancelCurrent();
        }
    }

    public void Dispose() => Cancel();

    // Cancelled before it is disposed: a token already handed out then reads as cancelled, and never throws
    private void CancelCurrent()
    {
        _current?.Cancel();
        _current?.Dispose();
        _current = null;
    }
}