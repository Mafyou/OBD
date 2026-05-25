namespace OBD.Mobile.Lib.UnitTests.Models;

public class SearchResultTests
{
    [Fact]
    public void Constructor_ShouldAssignAllProperties()
    {
        var result = new SearchResult(
            ResultType.Note,
            "Title",
            "Subtitle",
            42,
            7,
            TypeNote.Sketch);

        result.ResultType.ShouldBe(ResultType.Note);
        result.Title.ShouldBe("Title");
        result.Subtitle.ShouldBe("Subtitle");
        result.Id.ShouldBe(42);
        result.SectorId.ShouldBe(7);
        result.NoteType.ShouldBe(TypeNote.Sketch);
    }
}