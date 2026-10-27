namespace OBD.Mobile.Lib.Diagnostics;

/// <summary>
/// Tells two shakes of the phone in quick succession from the readings of its accelerometer. Pure rules, without the
/// sensor, so they are tested on their own; <see cref="ShakeDetectionService"/> feeds it.
/// </summary>
public sealed class DoubleShakeDetector(TimeProvider timeProvider)
{
    /// <summary>Combined g-force of a shake: about 1 g at rest, a shake spikes well above that.</summary>
    public const double ShakeThreshold = 2.7;

    public static readonly TimeSpan DoubleShakeWindow = TimeSpan.FromSeconds(1.5);

    /// <summary>The readings of one shake keep coming for a moment: they count once.</summary>
    public static readonly TimeSpan ShakeDebounce = TimeSpan.FromMilliseconds(500);

    private DateTimeOffset? _firstShakeAt;
    private DateTimeOffset? _lastShakeAt;

    /// <summary>True when this reading, in g on each axis, completes a double shake.</summary>
    public bool OnReading(double x, double y, double z)
    {
        if (x * x + y * y + z * z < ShakeThreshold * ShakeThreshold)
            return false;

        var now = timeProvider.GetUtcNow();
        if (now - _lastShakeAt < ShakeDebounce)
            return false;

        _lastShakeAt = now;

        if (now - _firstShakeAt <= DoubleShakeWindow)
        {
            _firstShakeAt = null;
            return true;
        }

        _firstShakeAt = now;
        return false;
    }
}