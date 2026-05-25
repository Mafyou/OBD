namespace OBD.Mobile.Lib.UnitTests.Models;

public class NoteLinkTests
{
    [Fact]
    public void LinkedTitle_ShouldDefaultToEmptyString()
    {
        var noteLink = new NoteLink();

        noteLink.LinkedTitle.ShouldBe(string.Empty);
    }

    [Fact]
    public void Properties_ShouldStoreAssignedValues()
    {
        var noteLink = new NoteLink
        {
            Id = 10,
            NoteId = 11,
            Word = "keyword",
            SketchId = 12,
            LinkedTitle = "Visuel"
        };

        noteLink.Id.ShouldBe(10);
        noteLink.NoteId.ShouldBe(11);
        noteLink.Word.ShouldBe("keyword");
        noteLink.SketchId.ShouldBe(12);
        noteLink.LinkedTitle.ShouldBe("Visuel");
    }

    [Fact]
    public void SketchId_ShouldDefaultToZero()
    {
        var noteLink = new NoteLink();

        noteLink.SketchId.ShouldBe(0);
    }
}
