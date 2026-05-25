namespace OBD.Mobile.Lib.UnitTests.Services;

[Collection("DatabaseTests")]
public class PersonServiceTests
{
    [Fact]
    public async Task SearchAsync_ShouldSearchNamePositionAndMemo()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        _ = await db.InsertAsync(new Person { Name = "Alice", Position = "Dev", Memo = "Main contact" });
        _ = await db.InsertAsync(new Person { Name = "Bruno", Position = "Manager", Memo = "Other" });

        var service = new PersonService(db);

        var byName = await service.SearchAsync("Alice");
        var byPosition = await service.SearchAsync("Manager");
        var byMemo = await service.SearchAsync("contact");

        byName.Count.ShouldBe(1);
        byPosition.Count.ShouldBe(1);
        byMemo.Count.ShouldBe(1);
    }

    [Fact]
    public async Task CrudMethods_ShouldInsertUpdateGetAndDelete()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        var service = new PersonService(db);
        var person = new Person { Name = "Chris", Position = "Junior", Memo = "m" };

        var insertCount = await service.InsertAsync(person);
        person.Position = "Senior";
        var updateCount = await service.UpdateAsync(person);
        var saved = await service.GetAsync(person.Id);
        var deleteCount = await service.DeleteAsync(person);
        var deleted = await service.GetAsync(person.Id);

        insertCount.ShouldBe(1);
        updateCount.ShouldBe(1);
        saved.ShouldNotBeNull();
        saved.Position.ShouldBe("Senior");
        deleteCount.ShouldBe(1);
        deleted.ShouldBeNull();
    }
}