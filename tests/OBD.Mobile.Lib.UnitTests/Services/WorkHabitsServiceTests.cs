namespace OBD.Mobile.Lib.UnitTests.Services;

[Collection("DatabaseTests")]
public class WorkHabitsServiceTests
{
    [Fact]
    public async Task GetAsync_ShouldReturnNull_WhenNoRecordExists()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        var service = new WorkHabitsService(db);

        var result = await service.GetAsync();

        result.ShouldBeNull();
    }

    [Fact]
    public async Task SaveAsync_ShouldInsert_WhenIdIsZero()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        var service = new WorkHabitsService(db);
        var workHabits = new WorkHabits
        {
            RegularMeetings = "Daily",
            RemoteWorkDays = "Monday",
            ManagerId = 12
        };

        await service.SaveAsync(workHabits);

        var saved = await service.GetAsync();
        saved.ShouldNotBeNull();
        saved.ManagerId.ShouldBe(12);
    }

    [Fact]
    public async Task SaveAsync_ShouldUpdate_WhenIdIsNotZero()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        var service = new WorkHabitsService(db);
        var workHabits = new WorkHabits
        {
            RegularMeetings = "Daily",
            RemoteWorkDays = "Monday",
            ManagerId = 12
        };

        await service.SaveAsync(workHabits);

        var saved = await service.GetAsync();
        saved.ShouldNotBeNull();
        saved.ManagerId = 42;

        await service.SaveAsync(saved);

        var updated = await service.GetAsync();
        updated.ShouldNotBeNull();
        updated.ManagerId.ShouldBe(42);
    }
}
