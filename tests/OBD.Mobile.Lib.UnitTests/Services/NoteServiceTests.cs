namespace OBD.Mobile.Lib.UnitTests.Services;

[Collection("DatabaseTests")]
public class NoteServiceTests
{
    [Fact]
    public async Task GetBySecteurAsync_ShouldReturnOnlyNonSensitiveNotesForSector()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        var sector = new Sector { Name = "Engineering" };
        _ = await db.InsertAsync(sector);

        _ = await db.InsertAsync(new Note { SectorId = sector.Id, Content = "Visible", Keywords = "abc", IsSensitive = false });
        _ = await db.InsertAsync(new Note { SectorId = sector.Id, Content = "Secret", Keywords = "abc", IsSensitive = true });

        var service = new NoteService(db);

        var notes = await service.GetBySecteurAsync(sector.Id);

        notes.Count.ShouldBe(1);
        notes[0].Content.ShouldBe("Visible");
    }

    [Fact]
    public async Task GetSensiblesAsync_ShouldReturnOnlySensitiveNotes()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        _ = await db.InsertAsync(new Note { Content = "Visible", Keywords = "x", IsSensitive = false });
        _ = await db.InsertAsync(new Note { Content = "Hidden", Keywords = "x", IsSensitive = true });

        var service = new NoteService(db);

        var notes = await service.GetSensiblesAsync();

        notes.Count.ShouldBe(1);
        notes[0].Content.ShouldBe("Hidden");
    }

    [Fact]
    public async Task SearchAsync_ShouldSearchTitleContentAndKeywords_AndExcludeSensitive()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        _ = await db.InsertAsync(new Note { Title = "Topic", Content = "x", Keywords = "x", IsSensitive = false });
        _ = await db.InsertAsync(new Note { Title = "x", Content = "Keyword in content", Keywords = "x", IsSensitive = false });
        _ = await db.InsertAsync(new Note { Title = "x", Content = "x", Keywords = "magic", IsSensitive = false });
        _ = await db.InsertAsync(new Note { Title = "Topic", Content = "Sensitive hit", Keywords = "magic", IsSensitive = true });

        var service = new NoteService(db);

        var titleMatches = await service.SearchAsync("Topic");
        var contentMatches = await service.SearchAsync("content");
        var keywordMatches = await service.SearchAsync("magic");

        titleMatches.Count.ShouldBe(1);
        contentMatches.Count.ShouldBe(1);
        keywordMatches.Count.ShouldBe(1);
        keywordMatches[0].IsSensitive.ShouldBeFalse();
    }

    [Fact]
    public async Task CrudMethods_ShouldInsertUpdateGetAndDelete()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        var service = new NoteService(db);
        var note = new Note { Content = "Before", Keywords = "k" };

        var insertCount = await service.InsertAsync(note);
        note.Content = "After";
        var updateCount = await service.UpdateAsync(note);
        var saved = await service.GetAsync(note.Id);
        var deleteCount = await service.DeleteAsync(note);
        var deleted = await service.GetAsync(note.Id);

        insertCount.ShouldBe(1);
        updateCount.ShouldBe(1);
        saved.ShouldNotBeNull();
        saved.Content.ShouldBe("After");
        deleteCount.ShouldBe(1);
        deleted.ShouldBeNull();
    }
}