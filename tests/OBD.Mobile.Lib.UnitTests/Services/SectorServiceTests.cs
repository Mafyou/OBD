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
}
