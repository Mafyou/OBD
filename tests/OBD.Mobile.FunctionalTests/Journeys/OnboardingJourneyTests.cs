namespace OBD.Mobile.FunctionalTests.Journeys;

/// <summary>First launch: the user starts from the example team or from an empty notebook, then browses it.</summary>
[Collection(AppCollection.Name)]
public class OnboardingJourneyTests
{
    [Fact]
    public async Task PrefilledNotebook_ShouldOpenOnTheExampleTeam_AndLeadToTheNotesOfASector()
    {
        // Arrange
        using var app = new AppSession();
        var onboarding = app.Get<OnboardingViewModel>();
        var completed = false;
        onboarding.OnboardingCompleted += () => completed = true;

        // Act
        await onboarding.ChoosePrefilledCommand.ExecuteAsync(null);
        var notes = app.Get<NotesViewModel>();
        await notes.LoadSectorsCommand.ExecuteAsync(null);
        var development = notes.Sectors.Single(s => s.Name == "Développement");
        await notes.OpenSectorCommand.ExecuteAsync(development);
        var sector = app.Get<NotesSectorViewModel>();
        sector.SectorId = development.Id;
        await Eventually.HappensAsync(() => sector.SectorName == "Développement" && !sector.IsLoading, "the notes of the sector to load");

        // Assert
        completed.ShouldBeTrue();
        AppPlatform.Preferences[OnboardingViewModel.OnboardingCompletedKey].ShouldBe(true);
        notes.Sectors.Select(s => s.Name).ShouldBe(["Développement", "Produit", "Design", "Management", "RH"], ignoreOrder: true);
        notes.IsLoading.ShouldBeFalse();
        AppSession.LastRoute.ShouldBe($"{nameof(NotesSectorPage)}?sectorid={development.Id}");
        sector.Notes.ShouldHaveSingleItem().Content.ShouldStartWith("Stand-up daily");
    }

    [Fact]
    public async Task EmptyNotebook_ShouldOpenWithoutAnySector()
    {
        // Arrange
        using var app = new AppSession();
        var onboarding = app.Get<OnboardingViewModel>();
        var completed = false;
        onboarding.OnboardingCompleted += () => completed = true;

        // Act
        await onboarding.ChooseEmptyCommand.ExecuteAsync(null);
        var notes = app.Get<NotesViewModel>();
        await notes.LoadSectorsCommand.ExecuteAsync(null);

        // Assert
        completed.ShouldBeTrue();
        AppPlatform.Preferences[OnboardingViewModel.OnboardingCompletedKey].ShouldBe(true);
        notes.Sectors.ShouldBeEmpty();
    }

    [Fact]
    public async Task PrefilledNotebook_ShouldNotBeSeededTwice_WhenTheUserComesBackToTheOnboarding()
    {
        // Arrange
        using var app = new AppSession();
        await app.Get<OnboardingViewModel>().ChoosePrefilledCommand.ExecuteAsync(null);

        // Act
        await app.Get<OnboardingViewModel>().ChoosePrefilledCommand.ExecuteAsync(null);
        var persons = app.Get<PersonsViewModel>();
        await persons.LoadCommand.ExecuteAsync(null);

        // Assert
        persons.Persons.Count.ShouldBe(6);
    }
}
