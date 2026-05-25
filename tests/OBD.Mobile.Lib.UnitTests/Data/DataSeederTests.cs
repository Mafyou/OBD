namespace OBD.Mobile.Lib.UnitTests.Data;

[Collection("DatabaseTests")]
public class DataSeederTests
{
    [Fact]
    public async Task SeedAsync_ShouldInsertInitialData_WhenDatabaseIsEmpty()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        var seeder = new DataSeeder(db);

        await seeder.SeedAsync();

        var sectors = await db.GetAllAsync<Sector>();
        var persons = await db.GetAllAsync<Person>();
        var notes = await db.GetAllAsync<Note>();
        var workHabits = await db.GetAllAsync<WorkHabits>();

        sectors.Count.ShouldBe(4);
        persons.Count.ShouldBe(5);
        notes.Count.ShouldBe(4);
        workHabits.Count.ShouldBe(1);
    }

    [Fact]
    public async Task SeedAsync_ShouldNotDuplicateData_WhenCalledTwice()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        var seeder = new DataSeeder(db);

        await seeder.SeedAsync();
        await seeder.SeedAsync();

        var sectors = await db.GetAllAsync<Sector>();
        var persons = await db.GetAllAsync<Person>();
        var notes = await db.GetAllAsync<Note>();
        var workHabits = await db.GetAllAsync<WorkHabits>();

        sectors.Count.ShouldBe(4);
        persons.Count.ShouldBe(5);
        notes.Count.ShouldBe(4);
        workHabits.Count.ShouldBe(1);
    }
}