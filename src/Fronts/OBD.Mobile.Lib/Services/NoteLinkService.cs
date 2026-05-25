namespace OBD.Mobile.Lib.Services;

public class NoteLinkService(DatabaseContext db) : INoteLinkService
{
    public Task<List<NoteLink>> GetByNoteAsync(int noteId)
        => db.QueryAsync<NoteLink>("SELECT * FROM NoteLink WHERE NoteId = ?", noteId);

    public Task<int> InsertAsync(NoteLink link) => db.InsertAsync(link);

    public Task<int> DeleteAsync(NoteLink link) => db.DeleteAsync(link);
}
