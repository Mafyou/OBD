namespace OBD.Mobile.Lib.UnitTests.Models;

public class NoteLinkTests
{
    [Fact]
    public void SketchTitle_ShouldDefaultToEmptyString()
    {
        var noteLink = new NoteLink();

        noteLink.SketchTitle.ShouldBe(string.Empty);
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
            SketchTitle = "Sketch"
        };

        noteLink.Id.ShouldBe(10);
        noteLink.NoteId.ShouldBe(11);
        noteLink.Word.ShouldBe("keyword");
        noteLink.SketchId.ShouldBe(12);
        noteLink.SketchTitle.ShouldBe("Sketch");
    }
}