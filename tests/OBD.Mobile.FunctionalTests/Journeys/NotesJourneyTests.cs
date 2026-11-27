namespace OBD.Mobile.FunctionalTests.Journeys;

/// <summary>
/// Notes of a sector: the user writes a note, links one of its #words to a sketch, follows and removes the link, hides
/// a note among the sensitive ones and deletes notes.
/// </summary>
[Collection(AppCollection.Name)]
public class NotesJourneyTests
{
    private static readonly byte[] PngBytes = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 3, 1, 4];

    [Fact]
    public async Task NewTextNote_ShouldBeSavedInItsSector_AndShownInViewMode()
    {
        // Arrange
        using var app = new AppSession();
        var sector = await AddSectorAsync(app, "Produit");
        var notesOfSector = await OpenSectorAsync(app, sector);
        await notesOfSector.OpenNewTextNoteCommand.ExecuteAsync(null);
        var note = OpenNote(app, noteId: 0, sector.Id);
        var renders = 0;
        note.ContentRendered += () => renders++;

        // Act
        note.Note.Title = "Rituels";
        note.Note.Content = "Démo chaque jeudi, voir #roadmap";
        note.Note.Keywords = "démo,jeudi";
        await note.SaveCommand.ExecuteAsync(null);
        var reopened = await OpenSectorAsync(app, sector);

        // Assert
        AppSession.LastRoute.ShouldBe($"{nameof(NoteDetailPage)}?noteid=0&sectorid={sector.Id}");
        note.IsNew.ShouldBeFalse();
        note.IsViewMode.ShouldBeTrue();
        note.IsEditMode.ShouldBeFalse();
        note.PageTitle.ShouldBe(AppResources.GetString("NoteDetail_Title"));
        renders.ShouldBe(1);
        var listed = reopened.Notes.ShouldHaveSingleItem();
        listed.Title.ShouldBe("Rituels");
        listed.Content.ShouldBe("Démo chaque jeudi, voir #roadmap");
    }

    [Fact]
    public async Task Hashtag_ShouldBeLinkedToTheChosenSketch_FollowedAndUnlinked()
    {
        // Arrange
        using var app = new AppSession();
        var sector = await AddSectorAsync(app, "Développement");
        var sketch = await DrawSketchAsync(app, sector, "Architecture");
        var note = OpenNote(app, noteId: 0, sector.Id);
        note.Note.Content = "Lire #archi avant la première PR";
        await note.SaveCommand.ExecuteAsync(null);
        AppPlatform.ChooseAnswer = (_, choices) => choices.Single(c => c.Contains("Architecture"));

        // Act
        await note.TapHashtagAsync("archi");
        var routesBeforeFollowing = AppPlatform.Routes.Count;
        await note.TapHashtagAsync("ARCHI");
        var followedRoute = AppSession.LastRoute;
        await note.RemoveLinkCommand.ExecuteAsync(note.Links.Single());

        // Assert
        AppPlatform.ChoicesShown.ShouldHaveSingleItem().Title.ShouldBe(AppResources.GetString("NoteDetail_LinkPromptFormat", "archi"));
        AppPlatform.Routes.Count.ShouldBe(routesBeforeFollowing + 1);
        followedRoute.ShouldBe($"{nameof(SketchDetailsPage)}?noteid={sketch.Id}&sectorid=0");
        note.Links.ShouldBeEmpty();
        note.HasLinks.ShouldBeFalse();
        (await app.Get<INoteLinkService>().GetByNoteAsync(note.Note.Id)).ShouldBeEmpty();
    }

    [Fact]
    public async Task LinkedSketch_ShouldBeShownByItsTitle_WhenTheNoteIsOpenedAgain()
    {
        // Arrange
        using var app = new AppSession();
        var sector = await AddSectorAsync(app, "Design");
        _ = await DrawSketchAsync(app, sector, "Maquette");
        var note = OpenNote(app, noteId: 0, sector.Id);
        note.Note.Content = "Voir #maquette";
        await note.SaveCommand.ExecuteAsync(null);
        AppPlatform.ChooseAnswer = (_, choices) => choices.Single();
        await note.TapHashtagAsync("maquette");

        // Act
        var reopened = OpenNote(app, note.Note.Id, sector.Id);
        await Eventually.HappensAsync(() => reopened.IsViewMode && !reopened.IsEditMode, "the note to load");
        await reopened.OpenLinkedSketchCommand.ExecuteAsync(reopened.Links.Single());

        // Assert
        reopened.HasLinks.ShouldBeTrue();
        var link = reopened.Links.ShouldHaveSingleItem();
        link.Word.ShouldBe("maquette");
        link.LinkedTitle.ShouldBe("✏ Maquette");
        AppSession.LastRoute.ShouldBe($"{nameof(SketchDetailsPage)}?noteid={link.SketchId}&sectorid=0");
    }

    [Fact]
    public async Task Hashtag_ShouldExplainThereIsNoSketchToLink_WhenTheNotebookHasNone()
    {
        // Arrange
        using var app = new AppSession();
        var sector = await AddSectorAsync(app, "RH");
        var note = OpenNote(app, noteId: 0, sector.Id);
        note.Note.Content = "Voir #organigramme";
        await note.SaveCommand.ExecuteAsync(null);

        // Act
        await note.TapHashtagAsync("organigramme");

        // Assert
        AppPlatform.Alerts.ShouldHaveSingleItem().Title.ShouldBe(AppResources.GetString("NoteDetail_NoVisualTitle"));
        AppPlatform.ChoicesShown.ShouldBeEmpty();
        note.Links.ShouldBeEmpty();
    }

    [Fact]
    public async Task Hashtag_ShouldStayUnlinked_WhenTheUserCancelsTheChoice()
    {
        // Arrange
        using var app = new AppSession();
        var sector = await AddSectorAsync(app, "RH");
        _ = await DrawSketchAsync(app, sector, "Organigramme");
        var note = OpenNote(app, noteId: 0, sector.Id);
        note.Note.Content = "Voir #organigramme";
        await note.SaveCommand.ExecuteAsync(null);
        AppPlatform.ChooseAnswer = (_, _) => AppResources.GetString("Common_Cancel");

        // Act
        await note.TapHashtagAsync("organigramme");

        // Assert
        AppPlatform.ChoicesShown.Count.ShouldBe(1);
        note.Links.ShouldBeEmpty();
        (await app.Get<INoteLinkService>().GetByNoteAsync(note.Note.Id)).ShouldBeEmpty();
    }

    [Fact]
    public async Task EditedNote_ShouldKeepItsChanges()
    {
        // Arrange
        using var app = new AppSession();
        var sector = await AddSectorAsync(app, "Produit");
        var existing = new Note { SectorId = sector.Id, Content = "Specs sur Confluence" };
        _ = await app.Get<INoteService>().InsertAsync(existing);
        var note = OpenNote(app, existing.Id, sector.Id);
        await Eventually.HappensAsync(() => note.IsViewMode && !note.IsEditMode, "the note to load");

        // Act
        note.EditCommand.Execute(null);
        var editing = note.IsEditMode && !note.IsViewMode;
        note.Note.Content = "Specs sur Notion depuis mars";
        await note.SaveCommand.ExecuteAsync(null);

        // Assert
        editing.ShouldBeTrue();
        note.IsViewMode.ShouldBeTrue();
        (await app.Get<INoteService>().GetAsync(existing.Id)).ShouldNotBeNull().Content.ShouldBe("Specs sur Notion depuis mars");
        (await app.Get<INoteService>().GetBySectorAsync(sector.Id)).Count.ShouldBe(1);
    }

    [Fact]
    public async Task DeletedNote_ShouldLeaveTheSectorWithItsLinks()
    {
        // Arrange
        using var app = new AppSession();
        var sector = await AddSectorAsync(app, "Design");
        _ = await DrawSketchAsync(app, sector, "Charte");
        var note = OpenNote(app, noteId: 0, sector.Id);
        note.Note.Content = "Voir #charte";
        await note.SaveCommand.ExecuteAsync(null);
        AppPlatform.ChooseAnswer = (_, choices) => choices.Single();
        await note.TapHashtagAsync("charte");
        var noteId = note.Note.Id;

        // Act
        await note.DeleteCommand.ExecuteAsync(null);
        var notesOfSector = await OpenSectorAsync(app, sector);

        // Assert
        AppSession.LastRoute.ShouldBe("..");
        (await app.Get<INoteService>().GetAsync(noteId)).ShouldBeNull();
        (await app.Get<INoteLinkService>().GetByNoteAsync(noteId)).ShouldBeEmpty();
        notesOfSector.Notes.ShouldHaveSingleItem().Type.ShouldBe(TypeNote.Sketch);
    }

    [Fact]
    public async Task SensitiveNote_ShouldMoveToTheSensitiveList_AndComeBack()
    {
        // Arrange
        using var app = new AppSession();
        var sector = await AddSectorAsync(app, "Management");
        var salaries = new Note { SectorId = sector.Id, Title = "Salaires", Content = "Grille 2026", Keywords = "salaire" };
        var visible = new Note { SectorId = sector.Id, Content = "Rétro vendredi" };
        _ = await app.Get<INoteService>().InsertAsync(salaries);
        _ = await app.Get<INoteService>().InsertAsync(visible);
        var notesOfSector = await OpenSectorAsync(app, sector);

        // Act
        await notesOfSector.MarkSensitiveCommand.ExecuteAsync(notesOfSector.Notes.Single(n => n.Id == salaries.Id));
        var sensitive = app.Get<SensitiveNotesViewModel>();
        await sensitive.LoadCommand.ExecuteAsync(null);
        var hiddenFromSearch = await app.Get<INoteService>().SearchAsync("salaire");
        await sensitive.OpenNoteDetailCommand.ExecuteAsync(sensitive.Notes.Single());
        var openedRoute = AppSession.LastRoute;
        await sensitive.UnmarkSensitiveCommand.ExecuteAsync(sensitive.Notes.Single());
        var backInSector = await OpenSectorAsync(app, sector);

        // Assert
        notesOfSector.Notes.ShouldHaveSingleItem().Id.ShouldBe(visible.Id);
        hiddenFromSearch.ShouldBeEmpty();
        openedRoute.ShouldBe($"{nameof(NoteDetailPage)}?noteid={salaries.Id}&sectorid={sector.Id}");
        sensitive.Notes.ShouldBeEmpty();
        backInSector.Notes.Select(n => n.Id).ShouldBe([salaries.Id, visible.Id], ignoreOrder: true);
    }

    [Fact]
    public async Task SensitiveSketch_ShouldKeepItsImage_WhenMarkedFromTheListAndOpenedInTheSketchPage()
    {
        // Arrange
        using var app = new AppSession();
        var sector = await AddSectorAsync(app, "Management");
        var sketch = await DrawSketchAsync(app, sector, "Plan de succession");
        var notesOfSector = await OpenSectorAsync(app, sector);

        // Act
        await notesOfSector.MarkSensitiveCommand.ExecuteAsync(notesOfSector.Notes.Single());
        var sensitive = app.Get<SensitiveNotesViewModel>();
        await sensitive.LoadCommand.ExecuteAsync(null);
        await sensitive.OpenNoteDetailCommand.ExecuteAsync(sensitive.Notes.Single());

        // Assert
        AppSession.LastRoute.ShouldBe($"{nameof(SketchDetailsPage)}?noteid={sketch.Id}&sectorid={sector.Id}");
        var stored = (await app.Get<INoteService>().GetAsync(sketch.Id)).ShouldNotBeNull();
        stored.IsSensitive.ShouldBeTrue();
        Convert.FromBase64String(stored.Content).ShouldBe(PngBytes);
    }

    [Fact]
    public async Task NotesOfASector_ShouldOpenEachKindOfNoteInItsOwnPage()
    {
        // Arrange
        using var app = new AppSession();
        var sector = await AddSectorAsync(app, "Design");
        var text = new Note { SectorId = sector.Id, Content = "Texte" };
        var photo = new Note { SectorId = sector.Id, Type = TypeNote.Photo, Title = "Badge" };
        _ = await app.Get<INoteService>().InsertAsync(text);
        _ = await app.Get<INoteService>().InsertAsync(photo);
        var notesOfSector = await OpenSectorAsync(app, sector);

        // Act
        await notesOfSector.OpenNoteDetailCommand.ExecuteAsync(notesOfSector.Notes.Single(n => n.Id == text.Id));
        await notesOfSector.OpenNoteDetailCommand.ExecuteAsync(notesOfSector.Notes.Single(n => n.Id == photo.Id));
        await notesOfSector.OpenNewSketchCommand.ExecuteAsync(null);

        // Assert
        AppPlatform.Routes.ShouldBe(
        [
            $"{nameof(NoteDetailPage)}?noteid={text.Id}&sectorid={sector.Id}",
            $"{nameof(SketchDetailsPage)}?noteid={photo.Id}&sectorid={sector.Id}",
            $"{nameof(SketchDetailsPage)}?noteid=0&sectorid={sector.Id}",
        ]);
    }

    [Fact]
    public async Task SectorPage_ShouldShowTheLastSectorOpened_WhenTwoAreOpenedOneAfterTheOther()
    {
        // Arrange
        using var app = new AppSession();
        var first = await AddSectorAsync(app, "Produit");
        var second = await AddSectorAsync(app, "Design");
        _ = await app.Get<INoteService>().InsertAsync(new Note { SectorId = first.Id, Content = "Note produit" });
        _ = await app.Get<INoteService>().InsertAsync(new Note { SectorId = second.Id, Content = "Note design" });
        var page = app.Get<NotesSectorViewModel>();

        // Act: the page is shared by every sector
        page.SectorId = first.Id;
        page.SectorId = second.Id;
        await Eventually.HappensAsync(() => page.SectorName == "Design" && !page.IsLoading, "the second sector to load");

        // Assert
        page.Notes.ShouldHaveSingleItem().Content.ShouldBe("Note design");
    }

    private static async Task<Sector> AddSectorAsync(AppSession app, string name)
    {
        var sector = new Sector { Name = name };
        _ = await app.Get<ISectorService>().InsertAsync(sector);
        return sector;
    }

    private static async Task<NotesSectorViewModel> OpenSectorAsync(AppSession app, Sector sector)
    {
        var page = app.Get<NotesSectorViewModel>();
        page.SectorId = sector.Id;
        await Eventually.HappensAsync(() => page.SectorName == sector.Name && !page.IsLoading, $"the notes of {sector.Name} to load");
        return page;
    }

    private static NoteDetailViewModel OpenNote(AppSession app, int noteId, int sectorId)
    {
        var page = app.Get<NoteDetailViewModel>();
        page.ApplyQueryAttributes(new Dictionary<string, object> { ["noteid"] = $"{noteId}", ["sectorid"] = $"{sectorId}" });
        return page;
    }

    private static async Task<Note> DrawSketchAsync(AppSession app, Sector sector, string title)
    {
        var page = app.Get<SketchDetailsViewModel>();
        page.ApplyQueryAttributes(new Dictionary<string, object> { ["noteid"] = "0", ["sectorid"] = $"{sector.Id}" });
        page.Note.Title = title;
        using MemoryStream drawing = new(PngBytes);
        await page.SaveAsync(drawing);
        return page.Note;
    }
}
