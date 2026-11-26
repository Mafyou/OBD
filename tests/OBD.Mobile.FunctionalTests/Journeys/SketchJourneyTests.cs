namespace OBD.Mobile.FunctionalTests.Journeys;

/// <summary>Sketch page: the user draws, names, reopens, clears a sketch, and tries to take a photo as its background.</summary>
[Collection(AppCollection.Name)]
public class SketchJourneyTests
{
    private static readonly byte[] PngBytes = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 2, 7, 1];

    [Fact]
    public async Task NewSketch_ShouldBeSavedWithItsDrawing_AndReopenWithIt()
    {
        // Arrange
        using var app = new AppSession();
        var sector = new Sector { Name = "Design" };
        _ = await app.Get<ISectorService>().InsertAsync(sector);
        var page = OpenSketch(app, noteId: 0, sector.Id);
        var startedEmpty = page.IsNew && page.IsEditMode && !page.HasBackground;

        // Act
        page.Note.Title = "Parcours client";
        using (MemoryStream drawing = new(PngBytes))
            await page.SaveAsync(drawing);
        var reopened = OpenSketch(app, page.Note.Id, sector.Id);
        await Eventually.HappensAsync(() => reopened.IsViewMode && reopened.HasBackground, "the sketch and its drawing to load");

        // Assert
        startedEmpty.ShouldBeTrue();
        AppSession.LastRoute.ShouldBe("..");
        var stored = (await app.Get<INoteService>().GetAsync(page.Note.Id)).ShouldNotBeNull();
        stored.Type.ShouldBe(TypeNote.Sketch);
        stored.SectorId.ShouldBe(sector.Id);
        Convert.FromBase64String(stored.Content).ShouldBe(PngBytes);
        reopened.IsNew.ShouldBeFalse();
        reopened.PageTitle.ShouldBe("Parcours client");
        reopened.CurrentImage.ShouldNotBeNull();
        reopened.HasBackground.ShouldBeTrue();
        reopened.CanClear.ShouldBeTrue();
        (await app.Get<INoteService>().GetVisualsAsync()).ShouldHaveSingleItem().DisplayText.ShouldBe("✏ Parcours client");
    }

    [Fact]
    public async Task SketchWithoutTitle_ShouldTakeTheDefaultTitle()
    {
        // Arrange
        using var app = new AppSession();
        var page = OpenSketch(app, noteId: 0, sectorId: 0);

        // Act
        using (MemoryStream drawing = new(PngBytes))
            await page.SaveAsync(drawing);

        // Assert
        page.PageTitle.ShouldBe(AppResources.GetString("SketchDetail_DefaultTitle"));
    }

    [Fact]
    public async Task ClearedSketch_ShouldLoseItsDrawing_AndKeepItsName()
    {
        // Arrange
        using var app = new AppSession();
        var existing = new Note { Type = TypeNote.Sketch, Title = "Brouillon", Content = Convert.ToBase64String(PngBytes) };
        _ = await app.Get<INoteService>().InsertAsync(existing);
        var page = OpenSketch(app, existing.Id, sectorId: 0);
        await Eventually.HappensAsync(() => page.IsViewMode && !page.IsEditMode && page.HasBackground, "the sketch to load");

        // Act
        page.EditCommand.Execute(null);
        await page.SaveAsync(imageStream: null, clearImage: true);

        // Assert
        var stored = (await app.Get<INoteService>().GetAsync(existing.Id)).ShouldNotBeNull();
        stored.Content.ShouldBeEmpty();
        stored.Title.ShouldBe("Brouillon");
        (await app.Get<INoteService>().GetVisualsAsync()).Count.ShouldBe(1);
    }

    [Fact]
    public async Task Sketch_ShouldKeepItsDrawing_WhenSavedWithoutANewOne()
    {
        // Arrange
        using var app = new AppSession();
        var existing = new Note { Type = TypeNote.Sketch, Title = "Plan", Content = Convert.ToBase64String(PngBytes) };
        _ = await app.Get<INoteService>().InsertAsync(existing);
        var page = OpenSketch(app, existing.Id, sectorId: 0);
        await Eventually.HappensAsync(() => page.IsViewMode && !page.IsEditMode && page.HasBackground, "the sketch to load");

        // Act
        page.Note.Title = "Plan du 3e étage";
        await page.SaveAsync(imageStream: null);

        // Assert
        var stored = (await app.Get<INoteService>().GetAsync(existing.Id)).ShouldNotBeNull();
        stored.Title.ShouldBe("Plan du 3e étage");
        Convert.FromBase64String(stored.Content).ShouldBe(PngBytes);
    }

    [Fact]
    public async Task DeletedSketch_ShouldLeaveTheNotebook()
    {
        // Arrange
        using var app = new AppSession();
        var existing = new Note { Type = TypeNote.Sketch, Title = "Ancien plan" };
        _ = await app.Get<INoteService>().InsertAsync(existing);
        var page = OpenSketch(app, existing.Id, sectorId: 0);
        await Eventually.HappensAsync(() => page.IsViewMode && !page.IsEditMode, "the sketch to load");

        // Act
        await page.DeleteCommand.ExecuteAsync(null);

        // Assert
        AppSession.LastRoute.ShouldBe("..");
        (await app.Get<INoteService>().GetVisualsAsync()).ShouldBeEmpty();
    }

    [Fact]
    public void PenColor_ShouldFollowTheChoiceOfTheUser()
    {
        // Arrange
        using var app = new AppSession();
        var page = OpenSketch(app, noteId: 0, sectorId: 0);

        // Act
        page.SetLineColorCommand.Execute("Red");

        // Assert
        page.SelectedColorName.ShouldBe("Red");
        page.LineColor.ShouldNotBeNull();
    }

    [Fact]
    public async Task Camera_ShouldTellTheUser_WhenThePhotoCannotBeTaken()
    {
        // Arrange
        using var app = new AppSession();
        var page = OpenSketch(app, noteId: 0, sectorId: 0);
        AppPlatform.CapturePhoto = () => throw new UnauthorizedAccessException("Camera permission denied");

        // Act
        await page.TakePhotoAsBackgroundCommand.ExecuteAsync(null);

        // Assert
        AppPlatform.Alerts.ShouldHaveSingleItem().Message.ShouldBe(AppResources.GetString("SketchDetail_CameraErrorMessage"));
        page.HasBackground.ShouldBeFalse();
    }

    [Fact]
    public async Task Gallery_ShouldChangeNothing_WhenTheUserPicksNoPhoto()
    {
        // Arrange
        using var app = new AppSession();
        var page = OpenSketch(app, noteId: 0, sectorId: 0);

        // Act
        await page.PickPhotoAsBackgroundCommand.ExecuteAsync(null);
        await page.TakePhotoAsBackgroundCommand.ExecuteAsync(null);

        // Assert
        AppPlatform.Alerts.ShouldBeEmpty();
        page.CurrentImage.ShouldBeNull();
    }

    [Fact]
    public async Task Gallery_ShouldTellTheUser_WhenThePhotosCannotBeRead()
    {
        // Arrange
        using var app = new AppSession();
        var page = OpenSketch(app, noteId: 0, sectorId: 0);
        AppPlatform.PickPhotos = () => throw new IOException("Storage unavailable");

        // Act
        await page.PickPhotoAsBackgroundCommand.ExecuteAsync(null);

        // Assert
        AppPlatform.Alerts.ShouldHaveSingleItem().Message.ShouldBe(AppResources.GetString("SketchDetail_GalleryErrorMessage"));
    }

    private static SketchDetailsViewModel OpenSketch(AppSession app, int noteId, int sectorId)
    {
        var page = app.Get<SketchDetailsViewModel>();
        page.ApplyQueryAttributes(new Dictionary<string, object> { ["noteid"] = $"{noteId}", ["sectorid"] = $"{sectorId}" });
        return page;
    }
}
