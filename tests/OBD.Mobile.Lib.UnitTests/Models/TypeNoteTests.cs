namespace OBD.Mobile.Lib.UnitTests.Models;

public class TypeNoteTests
{
    [Fact]
    public void Enum_ShouldContainExpectedValues()
    {
        Enum.IsDefined(TypeNote.Text).ShouldBeTrue();
        Enum.IsDefined(TypeNote.Sketch).ShouldBeTrue();
        Enum.IsDefined(TypeNote.Photo).ShouldBeTrue();
    }
}