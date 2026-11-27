namespace OBD.Mobile.Lib.UnitTests.Navigation;

public class DoubleBackGateTests
{
    private readonly ManualTimeProvider _time = new();

    [Fact]
    public void Press_ShouldAsk_OnAFirstPress()
    {
        var gate = new DoubleBackGate(_time);

        gate.Press().ShouldBe(BackPress.Ask);
    }

    [Theory]
    [InlineData(800)]
    [InlineData(1500)]
    [InlineData(2000)]
    public void Press_ShouldLeave_OnASecondPressWithinTheWindow(int afterMilliseconds)
    {
        var gate = new DoubleBackGate(_time);

        gate.Press().ShouldBe(BackPress.Ask);
        _time.Advance(TimeSpan.FromMilliseconds(afterMilliseconds));
        gate.Press().ShouldBe(BackPress.Leave);
    }

    [Fact]
    public void Press_ShouldAskAgain_WhenTheSecondPressComesTooLate()
    {
        var gate = new DoubleBackGate(_time);

        gate.Press().ShouldBe(BackPress.Ask);
        _time.Advance(DoubleBackGate.Window + TimeSpan.FromMilliseconds(1));
        gate.Press().ShouldBe(BackPress.Ask);
        _time.Advance(TimeSpan.FromMilliseconds(600));
        gate.Press().ShouldBe(BackPress.Leave);
    }

    // The phone test: each press showed the toast again and the app was never left
    [Fact]
    public void Press_ShouldNeverAskAgain_WhenTheConfirmingPressReachesThePageTwice()
    {
        var gate = new DoubleBackGate(_time);

        gate.Press().ShouldBe(BackPress.Ask);
        _time.Advance(TimeSpan.FromMilliseconds(800));
        gate.Press().ShouldBe(BackPress.Leave);
        _time.Advance(TimeSpan.FromMilliseconds(5));
        gate.Press().ShouldBe(BackPress.SamePress);
        _time.Advance(DoubleBackGate.SamePress - TimeSpan.FromMilliseconds(5));
        gate.Press().ShouldBe(BackPress.SamePress);
    }

    [Fact]
    public void Press_ShouldStartOver_OnceTheConfirmingPressIsPast()
    {
        var gate = new DoubleBackGate(_time);

        gate.Press().ShouldBe(BackPress.Ask);
        _time.Advance(TimeSpan.FromSeconds(1));
        gate.Press().ShouldBe(BackPress.Leave);

        // Back in the app later: one press asks again, it does not leave
        _time.Advance(TimeSpan.FromMinutes(1));
        gate.Press().ShouldBe(BackPress.Ask);
        _time.Advance(TimeSpan.FromMilliseconds(700));
        gate.Press().ShouldBe(BackPress.Leave);
    }

    [Fact]
    public void Press_ShouldAsk_WhenTheClockGoesBackwards()
    {
        var gate = new DoubleBackGate(_time);

        gate.Press().ShouldBe(BackPress.Ask);
        _time.Advance(TimeSpan.FromSeconds(-1));
        gate.Press().ShouldBe(BackPress.Ask);
    }

    private sealed class ManualTimeProvider : TimeProvider
    {
        private DateTimeOffset _now = new(2026, 11, 27, 9, 0, 0, TimeSpan.Zero);

        public override DateTimeOffset GetUtcNow() => _now;

        public void Advance(TimeSpan by) => _now += by;
    }
}
