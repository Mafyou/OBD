namespace OBD.Mobile.Lib.Abstractions;

public interface INoteService
{
    Task<List<Note>> GetAllAsync();
    Task<List<Note>> GetBySecteurAsync(int sectorId);
    Task<List<Note>> GetSensiblesAsync();
    Task<List<Note>> SearchAsync(string query);
    Task<Note?> GetAsync(int id);
    Task<int> InsertAsync(Note note);
    Task<int> UpdateAsync(Note note);
    Task<int> DeleteAsync(Note note);
}
