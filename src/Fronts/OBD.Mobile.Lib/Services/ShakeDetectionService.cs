namespace OBD.Mobile.Lib.Services;

/// <summary>
/// Watches the accelerometer for two shakes within a short window, so developers can trigger the
/// diagnostic log/report flow (<see cref="ICrashReportMailer"/>) at any time, not just after a crash.
/// </summary>
public class ShakeDetectionService : IShakeDetectionService
{
    private const double ShakeThreshold = 2.7; // combined g-force; ~1.0g at rest, a shake spikes well above that.
    private static readonly TimeSpan DoubleShakeWindow = TimeSpan.FromSeconds(1.5);
    private static readonly TimeSpan ShakeDebounce = TimeSpan.FromMilliseconds(500);

    private DateTime _firstShakeAt = DateTime.MinValue;
    private DateTime _lastShakeAt = DateTime.MinValue;
    private bool _isListening;

    public event EventHandler? DoubleShakeDetected;

    public void Start()
    {
        if (_isListening || !Accelerometer.Default.IsSupported)
            return;

        Accelerometer.Default.ReadingChanged += OnReadingChanged;
        Accelerometer.Default.Start(SensorSpeed.UI);
        _isListening = true;
    }

    public void Stop()
    {
        if (!_isListening)
            return;

        Accelerometer.Default.Stop();
        Accelerometer.Default.ReadingChanged -= OnReadingChanged;
        _isListening = false;
    }

    private void OnReadingChanged(object? sender, AccelerometerChangedEventArgs e)
    {
        var acceleration = e.Reading.Acceleration;
        var magnitude = Math.Sqrt(
            acceleration.X * acceleration.X +
            acceleration.Y * acceleration.Y +
            acceleration.Z * acceleration.Z);

        if (magnitude < ShakeThreshold)
            return;

        var now = DateTime.UtcNow;
        if (now - _lastShakeAt < ShakeDebounce)
            return;

        _lastShakeAt = now;

        if (now - _firstShakeAt <= DoubleShakeWindow)
        {
            _firstShakeAt = DateTime.MinValue;
            DoubleShakeDetected?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            _firstShakeAt = now;
        }
    }
}
