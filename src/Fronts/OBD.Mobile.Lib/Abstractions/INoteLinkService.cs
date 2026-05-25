namespace OBD.Mobile.Lib.Abstractions;

public interface INoteLinkService
{
    Task<List<NoteLink>> GetByNoteAsync(int noteId);
    Task<int> InsertAsync(NoteLink link);
    Task<int> DeleteAsync(NoteLink link);
}
