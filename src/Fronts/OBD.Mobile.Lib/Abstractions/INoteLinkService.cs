namespace OBD.Mobile.Lib.Abstractions;

public interface INoteLinkService
{
    Task<IReadOnlyList<NoteLink>> GetByNoteAsync(int noteId);
    Task<int> InsertAsync(NoteLink link);
    Task<int> DeleteAsync(NoteLink link);
}
