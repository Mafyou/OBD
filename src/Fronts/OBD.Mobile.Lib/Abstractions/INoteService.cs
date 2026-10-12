namespace OBD.Mobile.Lib.Abstractions;

public interface INoteService
{
    Task<IReadOnlyList<Note>> GetAllAsync();
    Task<IReadOnlyList<Note>> GetBySecteurAsync(int sectorId);
    Task<IReadOnlyList<Note>> GetSensiblesAsync();
    Task<IReadOnlyList<Note>> SearchAsync(string query);
    Task<Note?> GetAsync(int id);
    Task<int> InsertAsync(Note note);
    Task<int> UpdateAsync(Note note);
    Task<int> DeleteAsync(Note note);
}
