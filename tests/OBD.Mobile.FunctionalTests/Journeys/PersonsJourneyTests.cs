namespace OBD.Mobile.FunctionalTests.Journeys;

/// <summary>People tab: the user adds a colleague, finds them with the filter, updates and removes them.</summary>
[Collection(AppCollection.Name)]
public class PersonsJourneyTests
{
    [Fact]
    public async Task NewColleague_ShouldBeListedWithTheirSector()
    {
        // Arrange
        using var app = new AppSession();
        var support = new Sector { Name = "Support" };
        _ = await app.Get<ISectorService>().InsertAsync(support);
        var persons = app.Get<PersonsViewModel>();
        await persons.OpenDetailCommand.ExecuteAsync(null);
        var details = app.Get<PersonDetailsViewModel>();
        details.PersonId = 0;
        await Eventually.HappensAsync(() => details.Sectors.Count is 1, "the sectors to load");

        // Act
        details.Person.Name = "Nadia Haddad";
        details.Person.Position = "Agent N2";
        details.Person.Memo = "Connaît tous les clients historiques";
        details.SelectedSector = details.Sectors[0];
        await details.SaveCommand.ExecuteAsync(null);
        await persons.LoadCommand.ExecuteAsync(null);

        // Assert
        AppPlatform.Routes.ShouldBe([$"{nameof(PersonDetailsPage)}?id=0", ".."]);
        details.HasNoSectors.ShouldBeFalse();
        var listed = persons.Persons.ShouldHaveSingleItem();
        listed.Name.ShouldBe("Nadia Haddad");
        listed.SectorName.ShouldBe("Support");
        listed.SectorAndPosition.ShouldBe("Support - Agent N2");
    }

    [Fact]
    public async Task NewColleague_ShouldSayThereIsNoSectorToChooseYet()
    {
        // Arrange
        using var app = new AppSession();
        var details = app.Get<PersonDetailsViewModel>();

        // Act
        details.PersonId = 0;
        await Eventually.HappensAsync(() => details.HasNoSectors, "the empty list of sectors");

        // Assert
        details.IsNew.ShouldBeTrue();
        details.PageTitle.ShouldBe(AppResources.GetString("PersonDetails_NewTitle"));
    }

    [Fact]
    public async Task ColleagueWithoutName_ShouldNotBeSaved()
    {
        // Arrange
        using var app = new AppSession();
        var details = app.Get<PersonDetailsViewModel>();
        details.PersonId = 0;
        await Eventually.HappensAsync(() => details.HasNoSectors, "the form to load");

        // Act
        details.Person.Position = "Stagiaire";
        await details.SaveCommand.ExecuteAsync(null);

        // Assert
        (await app.Get<IPersonService>().GetAllAsync()).ShouldBeEmpty();
        AppPlatform.Routes.ShouldBeEmpty();
    }

    [Fact]
    public async Task Filter_ShouldKeepOnlyTheMatchingColleagues_AndOpenOne()
    {
        // Arrange
        using var app = new AppSession();
        await app.Get<DataSeeder>().SeedAsync();
        var persons = app.Get<PersonsViewModel>();
        await persons.LoadCommand.ExecuteAsync(null);
        persons.Persons.Count.ShouldBe(6);

        // Act: each letter typed reloads the list
        persons.SearchQuery = "F";
        persons.SearchQuery = "Fi";
        persons.SearchQuery = "Figma";
        await Eventually.HappensAsync(() => persons.Persons.Count is 1 && !persons.IsLoading, "the filtered list");
        await persons.OpenDetailCommand.ExecuteAsync(persons.Persons[0]);

        // Assert
        var marc = persons.Persons[0];
        marc.Name.ShouldBe("Marc Leroy");
        marc.SectorName.ShouldBe("Design");
        AppSession.LastRoute.ShouldBe($"{nameof(PersonDetailsPage)}?id={marc.Id}");
    }

    [Fact]
    public async Task ExistingColleague_ShouldBeUpdated_ThenRemoved()
    {
        // Arrange
        using var app = new AppSession();
        await app.Get<DataSeeder>().SeedAsync();
        var thomas = (await app.Get<IPersonService>().SearchAsync("Thomas")).Single();
        var details = app.Get<PersonDetailsViewModel>();
        details.PersonId = thomas.Id;
        await Eventually.HappensAsync(() => details.SelectedSector is not null, "the colleague and their sector to load");
        details.SelectedSector.ShouldNotBeNull().Name.ShouldBe("Développement");

        // Act
        details.Person.Position = "Staff Engineer";
        details.SelectedSector = details.Sectors.Single(s => s.Name == "Management");
        await details.SaveCommand.ExecuteAsync(null);
        var updated = await app.Get<IPersonService>().GetAsync(thomas.Id);
        var again = app.Get<PersonDetailsViewModel>();
        again.PersonId = thomas.Id;
        await Eventually.HappensAsync(() => !again.IsNew, "the colleague to load again");
        await again.DeleteCommand.ExecuteAsync(null);

        // Assert
        updated.ShouldNotBeNull().Position.ShouldBe("Staff Engineer");
        (await app.Get<ISectorService>().GetAsync(updated.SectorId)).ShouldNotBeNull().Name.ShouldBe("Management");
        (await app.Get<IPersonService>().GetAsync(thomas.Id)).ShouldBeNull();
        AppPlatform.Routes.ShouldBe(["..", ".."]);
    }
}
