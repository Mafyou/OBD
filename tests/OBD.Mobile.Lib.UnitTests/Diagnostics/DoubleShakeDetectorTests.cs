namespace OBD.Mobile.Lib.UnitTests.Diagnostics;

public class DoubleShakeDetectorTests
{
    private readonly ManualTimeProvider _time = new();

    [Fact]
    public void OnReading_ShouldIgnoreAPhoneAtRest()
    {
        var detector = new DoubleShakeDetector(_time);

        detector.OnReading(0, 0, 1).ShouldBeFalse();
        _time.Advance(TimeSpan.FromSeconds(1));
        detector.OnReading(0, 0, 1).ShouldBeFalse();
    }

    [Fact]
    public void OnReading_ShouldDetectTwoShakesWithinTheWindow()
    {
        var detector = new DoubleShakeDetector(_time);

        detector.OnReading(3, 0, 0).ShouldBeFalse();
        _time.Advance(TimeSpan.FromMilliseconds(800));
        detector.OnReading(0, 3, 0).ShouldBeTrue();
    }

    [Fact]
    public void OnReading_ShouldCountTheReadingsOfOneShakeOnce()
    {
        var detector = new DoubleShakeDetector(_time);

        detector.OnReading(3, 0, 0).ShouldBeFalse();
        _time.Advance(TimeSpan.FromMilliseconds(100));
        detector.OnReading(3, 0, 0).ShouldBeFalse();
    }

    [Fact]
    public void OnReading_ShouldStartOver_WhenTheSecondShakeComesTooLate()
    {
        var detector = new DoubleShakeDetector(_time);

        detector.OnReading(3, 0, 0).ShouldBeFalse();
        _time.Advance(TimeSpan.FromSeconds(2));
        detector.OnReading(3, 0, 0).ShouldBeFalse();
        _time.Advance(TimeSpan.FromSeconds(1));
        detector.OnReading(3, 0, 0).ShouldBeTrue();
    }

    [Fact]
    public void OnReading_ShouldNeedTwoNewShakes_AfterADoubleShake()
    {
        var detector = new DoubleShakeDetector(_time);
        detector.OnReading(3, 0, 0);
        _time.Advance(TimeSpan.FromMilliseconds(600));
        detector.OnReading(3, 0, 0).ShouldBeTrue();

        _time.Advance(TimeSpan.FromMilliseconds(600));

        detector.OnReading(3, 0, 0).ShouldBeFalse();
    }

    private sealed class ManualTimeProvider : TimeProvider
    {
        private DateTimeOffset _now = new(2026, 10, 27, 9, 0, 0, TimeSpan.Zero);

        public override DateTimeOffset GetUtcNow() => _now;

        public void Advance(TimeSpan by) => _now += by;
    }
}