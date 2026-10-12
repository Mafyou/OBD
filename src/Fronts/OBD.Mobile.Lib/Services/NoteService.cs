namespace OBD.Mobile.Lib.Services;

public class NoteService(DatabaseContext db) : INoteService
{
    public Task<IReadOnlyList<Note>> GetAllAsync() => db.GetAllAsync<Note>();

    public Task<IReadOnlyList<Note>> GetBySecteurAsync(int sectorId)
        => db.QueryAsync<Note>("SELECT * FROM Note WHERE SectorId = ? AND IsSensitive = 0", sectorId);

    public Task<IReadOnlyList<Note>> GetSensiblesAsync()
        => db.QueryAsync<Note>("SELECT * FROM Note WHERE IsSensitive = 1");

    public Task<IReadOnlyList<Note>> SearchAsync(string query)
        => db.QueryAsync<Note>(
            "SELECT * FROM Note WHERE (Content LIKE ? OR Keywords LIKE ? OR Title LIKE ?) AND IsSensitive = 0",
            $"%{query}%", $"%{query}%", $"%{query}%");

    public Task<Note?> GetAsync(int id) => db.GetAsync<Note>(id);

    public Task<int> InsertAsync(Note note) => db.InsertAsync(note);

    public Task<int> UpdateAsync(Note note) => db.UpdateAsync(note);

    public Task<int> DeleteAsync(Note note) => db.DeleteAsync(note);
}
