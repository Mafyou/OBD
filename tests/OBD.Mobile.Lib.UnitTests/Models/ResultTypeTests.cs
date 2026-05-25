namespace OBD.Mobile.Lib.UnitTests.Models;

public class ResultTypeTests
{
    [Fact]
    public void Enum_ShouldContainExpectedValues()
    {
        Enum.IsDefined(ResultType.Person).ShouldBeTrue();
        Enum.IsDefined(ResultType.Note).ShouldBeTrue();
        Enum.IsDefined(ResultType.Sector).ShouldBeTrue();
    }
}