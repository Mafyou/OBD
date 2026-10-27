namespace OBD.Mobile.Lib.UnitTests.Services;

[Collection("DatabaseTests")]
public class NoteServiceTests
{
    [Fact]
    public async Task GetBySectorAsync_ShouldReturnOnlyNonSensitiveNotesForSector()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        var sector = new Sector { Name = "Engineering" };
        _ = await db.InsertAsync(sector);

        _ = await db.InsertAsync(new Note { SectorId = sector.Id, Content = "Visible", Keywords = "abc", IsSensitive = false });
        _ = await db.InsertAsync(new Note { SectorId = sector.Id, Content = "Secret", Keywords = "abc", IsSensitive = true });

        var service = new NoteService(db);

        var notes = await service.GetBySectorAsync(sector.Id);

        notes.Count.ShouldBe(1);
        notes[0].Content.ShouldBe("Visible");
    }

    [Fact]
    public async Task GetSensitiveAsync_ShouldReturnOnlySensitiveNotes()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        _ = await db.InsertAsync(new Note { Content = "Visible", Keywords = "x", IsSensitive = false });
        _ = await db.InsertAsync(new Note { Content = "Hidden", Keywords = "x", IsSensitive = true });

        var service = new NoteService(db);

        var notes = await service.GetSensitiveAsync();

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

    [Fact]
    public async Task Lists_ShouldLeaveSketchImagesOut_AndKeepTextContent()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        var sector = new Sector { Name = "Engineering" };
        _ = await db.InsertAsync(sector);
        _ = await db.InsertAsync(new Note { SectorId = sector.Id, Type = TypeNote.Text, Content = "Daily at 9:30" });
        var sketch = new Note { SectorId = sector.Id, Type = TypeNote.Sketch, Title = "Plan", Content = "iVBORw0KGgo=" };
        _ = await db.InsertAsync(sketch);

        var service = new NoteService(db);

        var notes = await service.GetBySectorAsync(sector.Id);
        var visuals = await service.GetVisualsAsync();

        notes.Count.ShouldBe(2);
        notes.Single(n => n.Type is TypeNote.Text).Content.ShouldBe("Daily at 9:30");
        var listed = notes.Single(n => n.Type is TypeNote.Sketch);
        listed.Content.ShouldBeEmpty();
        listed.Title.ShouldBe("Plan");
        visuals.ShouldHaveSingleItem().Id.ShouldBe(sketch.Id);
        (await service.GetAsync(sketch.Id))!.Content.ShouldBe("iVBORw0KGgo=");
    }

    [Fact]
    public async Task SearchAsync_ShouldNotMatchTheBase64OfASketch()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        _ = await db.InsertAsync(new Note { Type = TypeNote.Sketch, Title = "Plan", Content = "iVBORw0KGgoAAAANSUhEUg==" });

        var notes = await new NoteService(db).SearchAsync("KGgo");

        notes.ShouldBeEmpty();
    }

    [Fact]
    public async Task SearchAsync_ShouldSearchPercentAndUnderscoreAsThemselves()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        _ = await db.InsertAsync(new Note { Content = "Bonus of 10% in March" });
        _ = await db.InsertAsync(new Note { Content = "Bonus of 10 euros" });
        _ = await db.InsertAsync(new Note { Content = "file_name.txt" });
        _ = await db.InsertAsync(new Note { Content = "filename.txt" });

        var service = new NoteService(db);

        (await service.SearchAsync("10%")).ShouldHaveSingleItem().Content.ShouldBe("Bonus of 10% in March");
        (await service.SearchAsync("e_n")).ShouldHaveSingleItem().Content.ShouldBe("file_name.txt");
    }

    [Fact]
    public async Task SetSensitiveAsync_ShouldOnlyChangeTheFlag()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        var sketch = new Note { Type = TypeNote.Sketch, Title = "Plan", Content = "iVBORw0KGgo=" };
        _ = await db.InsertAsync(sketch);
        var service = new NoteService(db);

        await service.SetSensitiveAsync(sketch.Id, isSensitive: true);

        var saved = await service.GetAsync(sketch.Id);
        saved!.IsSensitive.ShouldBeTrue();
        saved.Content.ShouldBe("iVBORw0KGgo=");
        (await service.GetSensitiveAsync()).ShouldHaveSingleItem().Id.ShouldBe(sketch.Id);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteTheLinksFromAndToTheNote()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        var text = new Note { Content = "See #plan" };
        var sketch = new Note { Type = TypeNote.Sketch, Title = "Plan" };
        var other = new Note { Content = "Another #plan" };
        _ = await db.InsertAsync(text);
        _ = await db.InsertAsync(sketch);
        _ = await db.InsertAsync(other);
        _ = await db.InsertAsync(new NoteLink { NoteId = text.Id, Word = "plan", SketchId = sketch.Id });
        _ = await db.InsertAsync(new NoteLink { NoteId = other.Id, Word = "plan", SketchId = sketch.Id });
        var unrelated = new NoteLink { NoteId = other.Id, Word = "map", SketchId = 999 };
        _ = await db.InsertAsync(unrelated);

        var deleted = await new NoteService(db).DeleteAsync(sketch);

        deleted.ShouldBe(1);
        (await db.GetAsync<Note>(sketch.Id)).ShouldBeNull();
        (await db.GetAllAsync<NoteLink>()).ShouldHaveSingleItem().Id.ShouldBe(unrelated.Id);
    }
}