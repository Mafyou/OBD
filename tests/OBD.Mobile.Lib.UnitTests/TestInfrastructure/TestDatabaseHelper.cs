namespace OBD.Mobile.Lib.UnitTests.TestInfrastructure;

public static class TestDatabaseHelper
{
    public static async Task<DatabaseContext> CreateCleanDatabaseContextAsync()
    {
        var databasePath = Path.Combine(Path.GetTempPath(), $"obd-tests-{Guid.NewGuid():N}.db3");
        var db = new DatabaseContext(databasePath);

        _ = await db.GetAllAsync<Sector>();

        return db;
    }

    public static async Task<(Sector sector, Note note, Person person)> SeedBasicDataAsync(DatabaseContext db)
    {
        var sector = new Sector { Name = "Engineering" };
        _ = await db.InsertAsync(sector);

        var note = new Note
        {
            SectorId = sector.Id,
            Type = TypeNote.Text,
            Title = "Welcome",
            Content = "Team onboarding note",
            Keywords = "onboarding,team"
        };
        _ = await db.InsertAsync(note);

        var person = new Person
        {
            Name = "Alice Martin",
            Position = "Developer",
            SectorId = sector.Id,
            Memo = "Main contact"
        };
        _ = await db.InsertAsync(person);

        return (sector, note, person);
    }
}
