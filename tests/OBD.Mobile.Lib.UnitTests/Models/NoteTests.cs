namespace OBD.Mobile.Lib.UnitTests.Models;

public class NoteTests
{
    [Fact]
    public void DisplayText_ShouldReturnSketchPlaceholder_WhenTypeIsSketchAndTitleIsEmpty()
    {
        var note = new Note
        {
            Type = TypeNote.Sketch,
            Title = string.Empty,
            Content = "Ignored"
        };

        note.DisplayText.ShouldBe("✏ Croquis");
    }

    [Fact]
    public void DisplayText_ShouldReturnSketchTitle_WhenTypeIsSketchAndTitleIsProvided()
    {
        var note = new Note
        {
            Type = TypeNote.Sketch,
            Title = "Architecture",
            Content = "Ignored"
        };

        note.DisplayText.ShouldBe("✏ Architecture");
    }

    [Fact]
    public void DisplayText_ShouldReturnContent_WhenTypeIsTextAndTitleIsEmpty()
    {
        var note = new Note
        {
            Type = TypeNote.Text,
            Title = string.Empty,
            Content = "Important note"
        };

        note.DisplayText.ShouldBe("Important note");
    }

    [Fact]
    public void DisplayText_ShouldReturnTitle_WhenTypeIsTextAndTitleIsProvided()
    {
        var note = new Note
        {
            Type = TypeNote.Text,
            Title = "Summary",
            Content = "Ignored"
        };

        note.DisplayText.ShouldBe("Summary");
    }
}