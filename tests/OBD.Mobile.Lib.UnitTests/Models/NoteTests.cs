namespace OBD.Mobile.Lib.UnitTests.Models;

// Note.DisplayText now resolves its placeholder labels ("Croquis"/"Photo") through AppResources,
// which falls back to CurrentUICulture. Pin the culture to French (the app's resx-neutral default)
// so these assertions stay deterministic regardless of the host machine's locale.
public class NoteTests : IDisposable
{
    private readonly CultureInfo? _originalCulture = AppResources.Culture;

    public NoteTests() => AppResources.Culture = new CultureInfo("fr");

    public void Dispose() => AppResources.Culture = _originalCulture;

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

    [Fact]
    public void DisplayText_ShouldReturnPhotoPlaceholder_WhenTypeIsPhotoAndTitleIsEmpty()
    {
        var note = new Note
        {
            Type = TypeNote.Photo,
            Title = string.Empty,
            Content = "base64data"
        };

        note.DisplayText.ShouldBe("📷 Photo");
    }

    [Fact]
    public void DisplayText_ShouldReturnPhotoTitle_WhenTypeIsPhotoAndTitleIsProvided()
    {
        var note = new Note
        {
            Type = TypeNote.Photo,
            Title = "Reunion",
            Content = "base64data"
        };

        note.DisplayText.ShouldBe("📷 Reunion");
    }
}