namespace OBD.Mobile.Lib.UnitTests.Services;

[Collection("DatabaseTests")]
public class SectorServiceTests
{
    [Fact]
    public async Task SearchAsync_ShouldFilterByName()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        _ = await db.InsertAsync(new Sector { Name = "Engineering" });
        _ = await db.InsertAsync(new Sector { Name = "Product" });

        var service = new SectorService(db);

        var matches = await service.SearchAsync("Prod");

        matches.Count.ShouldBe(1);
        matches[0].Name.ShouldBe("Product");
    }

    [Fact]
    public async Task CrudMethods_ShouldInsertUpdateGetAndDelete()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        var service = new SectorService(db);
        var sector = new Sector { Name = "Ops" };

        var insertCount = await service.InsertAsync(sector);
        sector.Name = "Operations";
        var updateCount = await service.UpdateAsync(sector);
        var saved = await service.GetAsync(sector.Id);
        var deleteCount = await service.DeleteAsync(sector);
        var deleted = await service.GetAsync(sector.Id);

        insertCount.ShouldBe(1);
        updateCount.ShouldBe(1);
        saved.ShouldNotBeNull();
        saved.Name.ShouldBe("Operations");
        deleteCount.ShouldBe(1);
        deleted.ShouldBeNull();
    }

    [Fact]
    public async Task GetContentsAsync_ShouldCountTheNotesSensitiveOnesIncluded_AndThePeopleOfTheSectorOnly()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        var (sector, _, _) = await TestDatabaseHelper.SeedBasicDataAsync(db);
        _ = await db.InsertAsync(new Note { SectorId = sector.Id, Content = "secret", IsSensitive = true });
        var other = new Sector { Name = "Other" };
        _ = await db.InsertAsync(other);
        _ = await db.InsertAsync(new Note { SectorId = other.Id, Content = "elsewhere" });
        _ = await db.InsertAsync(new Person { Name = "Bob", SectorId = other.Id });

        var contents = await new SectorService(db).GetContentsAsync(sector.Id);

        contents.ShouldBe(new SectorContents(Notes: 2, Persons: 1));
    }

    [Fact]
    public async Task GetContentsAsync_ShouldBeEmpty_ForASectorWithNothingInIt()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        var sector = new Sector { Name = "Empty" };
        _ = await db.InsertAsync(sector);

        var contents = await new SectorService(db).GetContentsAsync(sector.Id);

        contents.IsEmpty.ShouldBeTrue();
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteTheNotesAndPeopleOfTheSector_AndLeaveTheOthers()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        var (sector, note, person) = await TestDatabaseHelper.SeedBasicDataAsync(db);
        var sensitive = new Note { SectorId = sector.Id, Content = "secret", IsSensitive = true };
        _ = await db.InsertAsync(sensitive);
        var other = new Sector { Name = "Other" };
        _ = await db.InsertAsync(other);
        var otherNote = new Note { SectorId = other.Id, Content = "kept" };
        _ = await db.InsertAsync(otherNote);
        var otherPerson = new Person { Name = "Bob", SectorId = other.Id };
        _ = await db.InsertAsync(otherPerson);

        var deleted = await new SectorService(db).DeleteAsync(sector);

        deleted.ShouldBe(1);
        (await db.GetAsync<Sector>(sector.Id)).ShouldBeNull();
        (await db.GetAsync<Note>(note.Id)).ShouldBeNull();
        (await db.GetAsync<Note>(sensitive.Id)).ShouldBeNull();
        (await db.GetAsync<Person>(person.Id)).ShouldBeNull();
        (await db.GetAsync<Sector>(other.Id)).ShouldNotBeNull();
        (await db.GetAsync<Note>(otherNote.Id)).ShouldNotBeNull();
        (await db.GetAsync<Person>(otherPerson.Id)).ShouldNotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteTheLinksFromAndToTheNotesOfTheSector_Only()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        var (sector, note, _) = await TestDatabaseHelper.SeedBasicDataAsync(db);
        var other = new Sector { Name = "Other" };
        _ = await db.InsertAsync(other);
        var otherNote = new Note { SectorId = other.Id, Content = "#plan" };
        _ = await db.InsertAsync(otherNote);
        var otherSketch = new Note { SectorId = other.Id, Type = TypeNote.Sketch };
        _ = await db.InsertAsync(otherSketch);
        var sketch = new Note { SectorId = sector.Id, Type = TypeNote.Sketch };
        _ = await db.InsertAsync(sketch);
        _ = await db.InsertAsync(new NoteLink { NoteId = note.Id, Word = "plan", SketchId = otherSketch.Id });
        _ = await db.InsertAsync(new NoteLink { NoteId = otherNote.Id, Word = "plan", SketchId = sketch.Id });
        var kept = new NoteLink { NoteId = otherNote.Id, Word = "map", SketchId = otherSketch.Id };
        _ = await db.InsertAsync(kept);

        _ = await new SectorService(db).DeleteAsync(sector);

        var links = await db.GetAllAsync<NoteLink>();
        links.Count.ShouldBe(1);
        links[0].Id.ShouldBe(kept.Id);
    }

    [Fact]
    public async Task DeleteAsync_ShouldClearTheManager_WhenTheManagerWasInTheSector()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        var (sector, _, person) = await TestDatabaseHelper.SeedBasicDataAsync(db);
        _ = await db.InsertAsync(new WorkHabits { RegularMeetings = "Daily", ManagerId = person.Id });

        _ = await new SectorService(db).DeleteAsync(sector);

        var habits = await new WorkHabitsService(db).GetAsync();
        habits.ShouldNotBeNull();
        habits.ManagerId.ShouldBe(0);
        habits.RegularMeetings.ShouldBe("Daily");
    }

    [Fact]
    public async Task DeleteAsync_ShouldKeepTheManager_WhenTheManagerWasInAnotherSector()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        var (sector, _, _) = await TestDatabaseHelper.SeedBasicDataAsync(db);
        var other = new Sector { Name = "Management" };
        _ = await db.InsertAsync(other);
        var manager = new Person { Name = "Sophie", SectorId = other.Id };
        _ = await db.InsertAsync(manager);
        _ = await db.InsertAsync(new WorkHabits { ManagerId = manager.Id });

        _ = await new SectorService(db).DeleteAsync(sector);

        (await new WorkHabitsService(db).GetAsync())!.ManagerId.ShouldBe(manager.Id);
    }
}
