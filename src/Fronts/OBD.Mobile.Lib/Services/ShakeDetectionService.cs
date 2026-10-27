namespace OBD.Mobile.Lib.Services;

/// <summary>
/// Watches the accelerometer for two shakes within a short window (<see cref="DoubleShakeDetector"/>), so developers can
/// open the diagnostic log/report flow (<see cref="ICrashReportMailer"/>) at any time, not just after a crash.
/// </summary>
public class ShakeDetectionService(TimeProvider timeProvider) : IShakeDetectionService
{
    private readonly DoubleShakeDetector _detector = new(timeProvider);
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
        if (_detector.OnReading(acceleration.X, acceleration.Y, acceleration.Z))
            DoubleShakeDetected?.Invoke(this, EventArgs.Empty);
    }
}