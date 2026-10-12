namespace OBD.Mobile.Lib.Abstractions;

/// <summary>Detects two device shakes in quick succession using the accelerometer.</summary>
public interface IShakeDetectionService
{
    /// <summary>Raised when two shakes are detected within the double-shake window.</summary>
    event EventHandler? DoubleShakeDetected;

    /// <summary>Starts listening to the accelerometer. Safe to call multiple times.</summary>
    void Start();

    /// <summary>Stops listening to the accelerometer.</summary>
    void Stop();
}
