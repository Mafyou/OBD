namespace OBD.Mobile.FunctionalTests.Journeys;

/// <summary>Settings → sectors: the user adds a sector, renames it and deletes it.</summary>
[Collection(AppCollection.Name)]
public class SectorsJourneyTests
{
    [Fact]
    public async Task NewSector_ShouldBeSaved_AndListedWithTheNotes()
    {
        // Arrange
        using var app = new AppSession();
        var parameters = app.Get<ParametersViewModel>();
        await parameters.OpenSectorDetailCommand.ExecuteAsync(null);
        var details = app.Get<SectorDetailsViewModel>();
        details.SectorId = 0;

        // Act
        details.Sector.Name = "Finance";
        await details.SaveCommand.ExecuteAsync(null);
        var notes = app.Get<NotesViewModel>();
        await notes.LoadSectorsCommand.ExecuteAsync(null);

        // Assert
        AppPlatform.Routes.ShouldBe([$"{nameof(SectorDetailsPage)}?id=0", ".."]);
        details.IsNew.ShouldBeTrue();
        notes.Sectors.ShouldHaveSingleItem().Name.ShouldBe("Finance");
    }

    [Fact]
    public async Task ExistingSector_ShouldBeRenamed_WithoutTheReappliedQueryReplacingTheForm()
    {
        // Arrange
        using var app = new AppSession();
        var sector = new Sector { Name = "Finance" };
        _ = await app.Get<ISectorService>().InsertAsync(sector);
        var details = app.Get<SectorDetailsViewModel>();
        details.SectorId = sector.Id;
        await Eventually.HappensAsync(() => details.PageTitle == "Finance", "the sector to load");

        // Act: a popup closed over the page makes Shell apply the query again
        details.Sector.Name = "Finance & Achats";
        details.SectorId = sector.Id;
        await details.SaveCommand.ExecuteAsync(null);

        // Assert
        details.IsNew.ShouldBeFalse();
        (await app.Get<ISectorService>().GetAsync(sector.Id)).ShouldNotBeNull().Name.ShouldBe("Finance & Achats");
        (await app.Get<ISectorService>().GetAllAsync()).Count.ShouldBe(1);
        AppSession.LastRoute.ShouldBe("..");
    }

    [Fact]
    public async Task SectorWithoutName_ShouldNeitherBeSavedNorCloseThePage()
    {
        // Arrange
        using var app = new AppSession();
        var details = app.Get<SectorDetailsViewModel>();
        details.SectorId = 0;

        // Act
        details.Sector.Name = "   ";
        await details.SaveCommand.ExecuteAsync(null);

        // Assert
        (await app.Get<ISectorService>().GetAllAsync()).ShouldBeEmpty();
        AppPlatform.Routes.ShouldBeEmpty();
    }

    [Fact]
    public async Task DeletedSector_ShouldLeaveTheList()
    {
        // Arrange
        using var app = new AppSession();
        var kept = new Sector { Name = "Produit" };
        var removed = new Sector { Name = "Ancien service" };
        _ = await app.Get<ISectorService>().InsertAsync(kept);
        _ = await app.Get<ISectorService>().InsertAsync(removed);
        var parameters = app.Get<ParametersViewModel>();
        await parameters.LoadCommand.ExecuteAsync(null);
        await parameters.OpenSectorDetailCommand.ExecuteAsync(parameters.Sectors.Single(s => s.Name == "Ancien service"));
        var details = app.Get<SectorDetailsViewModel>();
        details.SectorId = removed.Id;
        await Eventually.HappensAsync(() => !details.IsNew, "the sector to load");

        // Act
        await details.DeleteCommand.ExecuteAsync(null);
        await parameters.LoadCommand.ExecuteAsync(null);

        // Assert
        AppPlatform.Routes.ShouldBe([$"{nameof(SectorDetailsPage)}?id={removed.Id}", ".."]);
        parameters.Sectors.ShouldHaveSingleItem().Name.ShouldBe("Produit");
    }
}
