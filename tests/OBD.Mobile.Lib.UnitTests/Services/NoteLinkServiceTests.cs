namespace OBD.Mobile.Lib.UnitTests.Services;

[Collection("DatabaseTests")]
public class NoteLinkServiceTests
{
    [Fact]
    public async Task GetByNoteAsync_ShouldReturnLinksForMatchingNoteOnly()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        var note1 = new Note { Content = "n1", Keywords = "k" };
        var note2 = new Note { Content = "n2", Keywords = "k" };
        _ = await db.InsertAsync(note1);
        _ = await db.InsertAsync(note2);

        _ = await db.InsertAsync(new NoteLink { NoteId = note1.Id, Word = "alpha", SketchId = 1 });
        _ = await db.InsertAsync(new NoteLink { NoteId = note2.Id, Word = "beta", SketchId = 2 });

        var service = new NoteLinkService(db);

        var links = await service.GetByNoteAsync(note1.Id);

        links.Count.ShouldBe(1);
        links[0].Word.ShouldBe("alpha");
    }

    [Fact]
    public async Task InsertAndDeleteAsync_ShouldPersistThenRemoveLink()
    {
        var db = await TestDatabaseHelper.CreateCleanDatabaseContextAsync();
        var note = new Note { Content = "n", Keywords = "k" };
        _ = await db.InsertAsync(note);

        var service = new NoteLinkService(db);
        var link = new NoteLink { NoteId = note.Id, Word = "bridge", SketchId = 10 };

        var insertCount = await service.InsertAsync(link);
        var afterInsert = await service.GetByNoteAsync(note.Id);
        var deleteCount = await service.DeleteAsync(link);
        var afterDelete = await service.GetByNoteAsync(note.Id);

        insertCount.ShouldBe(1);
        afterInsert.Count.ShouldBe(1);
        deleteCount.ShouldBe(1);
        afterDelete.ShouldBeEmpty();
    }
}