namespace OBD.Mobile.Lib.Navigation;

/// <summary>What a press on Back on a root page does.</summary>
public enum BackPress
{
    /// <summary>A first press: the app asks (a toast) and stays.</summary>
    Ask,

    /// <summary>A second press within <see cref="DoubleBackGate.Window"/>: the app is left.</summary>
    Leave,

    /// <summary>The press that has just confirmed, handed to the page a second time: it is already being left.</summary>
    SamePress,
}

/// <summary>
/// The rules of the double Back that leaves the app from a root page, without the toast nor the platform, so they are
/// tested on their own: a first press asks, a second one within two seconds leaves. Pure rules: the page shows the
/// toast and leaves the app (DoubleBackToExit).
/// </summary>
public sealed class DoubleBackGate(TimeProvider clock)
{
    /// <summary>Time the user has to press Back again.</summary>
    public static readonly TimeSpan Window = TimeSpan.FromSeconds(2);

    /// <summary>
    /// How long the press that confirmed stands. One press can reach the page twice on Android: the Shell of MAUI asks
    /// it, then the callback that gives Back to the app (BackKey) may ask it again. Taken for a new first press, it
    /// showed the toast again and the app could never be left.
    /// </summary>
    public static readonly TimeSpan SamePress = TimeSpan.FromMilliseconds(500);

    private DateTimeOffset? _askedAt;
    private DateTimeOffset? _confirmedAt;

    /// <summary>Takes a press on Back that would leave the app.</summary>
    public BackPress Press()
    {
        var now = clock.GetUtcNow();

        if (_confirmedAt is { } confirmedAt && now >= confirmedAt && now - confirmedAt <= SamePress)
            return BackPress.SamePress;

        _confirmedAt = null;

        if (_askedAt is { } askedAt && now >= askedAt && now - askedAt <= Window)
        {
            _askedAt = null;
            _confirmedAt = now;
            return BackPress.Leave;
        }

        _askedAt = now;
        return BackPress.Ask;
    }
}
