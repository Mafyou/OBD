namespace OBD.Mobile.Lib.UnitTests.Data;

[Collection("DatabaseTests")]
public class DatabaseContextTests
{
    [Fact]
    public async Task InsertAndGetAsync_ShouldPersistAndReturnEntity()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        var sector = new Sector { Name = "Ops" };

        var insertCount = await db.InsertAsync(sector);
        var saved = await db.GetAsync<Sector>(sector.Id);

        insertCount.ShouldBe(1);
        saved.ShouldNotBeNull();
        saved.Id.ShouldBeGreaterThan(0);
        saved.Name.ShouldBe("Ops");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllInsertedEntities()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        _ = await db.InsertAsync(new Sector { Name = "A" });
        _ = await db.InsertAsync(new Sector { Name = "B" });

        var sectors = await db.GetAllAsync<Sector>();

        sectors.Count.ShouldBe(2);
        sectors.Select(x => x.Name).ShouldContain("A");
        sectors.Select(x => x.Name).ShouldContain("B");
    }

    [Fact]
    public async Task UpdateAsync_ShouldPersistUpdatedValues()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        var person = new Person { Name = "Bob", Position = "Junior", Memo = "Memo" };
        _ = await db.InsertAsync(person);

        person.Position = "Senior";

        var updateCount = await db.UpdateAsync(person);
        var updated = await db.GetAsync<Person>(person.Id);

        updateCount.ShouldBe(1);
        updated.ShouldNotBeNull();
        updated.Position.ShouldBe("Senior");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveEntity()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        var note = new Note { Content = "Temporary", Keywords = "tmp" };
        _ = await db.InsertAsync(note);

        var deleteCount = await db.DeleteAsync(note);
        var deleted = await db.GetAsync<Note>(note.Id);

        deleteCount.ShouldBe(1);
        deleted.ShouldBeNull();
    }

    [Fact]
    public async Task QueryAsync_ShouldApplySqlFilters()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        var sensitive = new Note { Content = "Sensitive", Keywords = "x", IsSensitive = true };
        var normal = new Note { Content = "Visible", Keywords = "y", IsSensitive = false };
        _ = await db.InsertAsync(sensitive);
        _ = await db.InsertAsync(normal);

        var visibleNotes = await db.QueryAsync<Note>("SELECT * FROM Note WHERE IsSensitive = 0");

        visibleNotes.Count.ShouldBe(1);
        visibleNotes[0].Content.ShouldBe("Visible");
    }
}