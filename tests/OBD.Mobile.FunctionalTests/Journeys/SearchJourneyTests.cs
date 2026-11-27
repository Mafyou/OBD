namespace OBD.Mobile.FunctionalTests.Journeys;

/// <summary>Search tab: the user types, pauses, and opens what was found among people, notes and sectors.</summary>
[Collection(AppCollection.Name)]
public class SearchJourneyTests
{
    private static readonly TimeSpan TypingPause = TimeSpan.FromMilliseconds(250);

    [Fact]
    public async Task Search_ShouldFindPeopleNotesAndSectors_OnceTheUserPauses()
    {
        // Arrange
        using var app = new AppSession();
        await app.Get<DataSeeder>().SeedAsync();
        var search = app.Get<SearchViewModel>();

        // Act
        search.SearchQuery = "design";
        var searchingBeforePause = search.IsSearching && search.HasSearched && !search.HasResults;
        app.Clock.Advance(TypingPause);
        await Eventually.HappensAsync(() => search.HasResults && !search.IsSearching, "the results of the search");

        // Assert
        searchingBeforePause.ShouldBeTrue();
        search.HasNoResults.ShouldBeFalse();
        search.Results.ShouldContain(r => r.ResultType == ResultType.Person && r.Title == "Marc Leroy" && r.Subtitle == "Designer UX");
        search.Results.ShouldContain(r => r.ResultType == ResultType.Sector && r.Title == "Design");
        search.Results.ShouldContain(r => r.ResultType == ResultType.Note && r.Subtitle.Contains("design system"));
    }

    [Fact]
    public async Task SearchResults_ShouldOpenThePageOfEachKindOfRecord()
    {
        // Arrange
        using var app = new AppSession();
        await app.Get<DataSeeder>().SeedAsync();
        var sketch = new Note { SectorId = 3, Type = TypeNote.Sketch, Title = "Design tokens" };
        _ = await app.Get<INoteService>().InsertAsync(sketch);
        var search = app.Get<SearchViewModel>();
        search.SearchQuery = "design";
        app.Clock.Advance(TypingPause);
        await Eventually.HappensAsync(() => search.HasResults, "the results of the search");
        var person = search.Results.Single(r => r.ResultType == ResultType.Person);
        var sector = search.Results.Single(r => r.ResultType == ResultType.Sector);
        var textNote = search.Results.Single(r => r.ResultType == ResultType.Note && r.NoteType == TypeNote.Text);
        var sketchResult = search.Results.Single(r => r.ResultType == ResultType.Note && r.NoteType == TypeNote.Sketch);

        // Act
        foreach (var result in new[] { person, sector, textNote, sketchResult })
            await search.OpenResultCommand.ExecuteAsync(result);

        // Assert
        AppPlatform.Routes.ShouldBe(
        [
            $"{nameof(PersonDetailsPage)}?id={person.Id}",
            $"{nameof(NotesSectorPage)}?sectorid={sector.Id}",
            $"{nameof(NoteDetailPage)}?noteid={textNote.Id}&sectorid={textNote.SectorId}",
            $"{nameof(SketchDetailsPage)}?noteid={sketch.Id}&sectorid=3",
        ]);
        sketchResult.Title.ShouldBe("✏ Design tokens");
    }

    [Fact]
    public async Task Search_ShouldRunOnlyForTheLastWordTyped()
    {
        // Arrange
        using var app = new AppSession();
        await app.Get<DataSeeder>().SeedAsync();
        var search = app.Get<SearchViewModel>();

        // Act: "S" alone would find most of the notebook
        search.SearchQuery = "S";
        search.SearchQuery = "So";
        search.SearchQuery = "Sophie";
        app.Clock.Advance(TypingPause);
        await Eventually.HappensAsync(() => search.HasResults && !search.IsSearching, "the results of the last search");
        app.Clock.Advance(TypingPause);

        // Assert
        search.Results.ShouldHaveSingleItem().Title.ShouldBe("Sophie Martin");
    }

    [Fact]
    public async Task Search_ShouldSayNothingWasFound()
    {
        // Arrange
        using var app = new AppSession();
        await app.Get<DataSeeder>().SeedAsync();
        var search = app.Get<SearchViewModel>();

        // Act
        search.SearchQuery = "kubernetes";
        app.Clock.Advance(TypingPause);
        await Eventually.HappensAsync(() => !search.IsSearching, "the search to end");

        // Assert
        search.HasSearched.ShouldBeTrue();
        search.HasNoResults.ShouldBeTrue();
        search.Results.ShouldBeEmpty();
    }

    [Fact]
    public async Task ClearedSearch_ShouldGoBackToTheEmptyScreen()
    {
        // Arrange
        using var app = new AppSession();
        await app.Get<DataSeeder>().SeedAsync();
        var search = app.Get<SearchViewModel>();
        search.SearchQuery = "Thomas";
        app.Clock.Advance(TypingPause);
        await Eventually.HappensAsync(() => search.HasResults && !search.IsSearching, "the results of the search");

        // Act
        search.SearchQuery = string.Empty;

        // Assert
        search.Results.ShouldBeEmpty();
        search.HasResults.ShouldBeFalse();
        search.HasSearched.ShouldBeFalse();
        search.HasNoResults.ShouldBeFalse();
        search.IsSearching.ShouldBeFalse();
    }

    [Fact]
    public async Task Search_ShouldNeverShowASensitiveNote()
    {
        // Arrange
        using var app = new AppSession();
        var secret = new Note { Content = "Augmentations prévues", Keywords = "augmentation" };
        _ = await app.Get<INoteService>().InsertAsync(secret);
        _ = await app.Get<INoteService>().SetSensitiveAsync(secret.Id, isSensitive: true);
        var search = app.Get<SearchViewModel>();

        // Act
        search.SearchQuery = "augmentation";
        app.Clock.Advance(TypingPause);
        await Eventually.HappensAsync(() => !search.IsSearching, "the search to end");

        // Assert
        search.HasNoResults.ShouldBeTrue();
    }
}
