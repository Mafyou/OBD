namespace OBD.Mobile.Lib.Services;

public class NoteService(DatabaseContext db) : INoteService
{
    // The image of a sketch (its Content, in base64) often weighs hundreds of kilobytes: lists only show its title, so
    // it stays in the database. 0 is TypeNote.Text, stored as a number.
    private const string ListColumns =
        "Id, SectorId, Type, Title, CASE WHEN Type = 0 THEN Content ELSE '' END AS Content, Keywords, IsSensitive, CreatedAt";

    public Task<IReadOnlyList<Note>> GetBySectorAsync(int sectorId)
        => db.QueryAsync<Note>($"SELECT {ListColumns} FROM Note WHERE SectorId = ? AND IsSensitive = 0", sectorId);

    public Task<IReadOnlyList<Note>> GetSensitiveAsync()
        => db.QueryAsync<Note>($"SELECT {ListColumns} FROM Note WHERE IsSensitive = 1");

    public Task<IReadOnlyList<Note>> GetVisualsAsync()
        => db.QueryAsync<Note>($"SELECT {ListColumns} FROM Note WHERE Type <> 0");

    // The content of a sketch is its image in base64: a search matched its letters at random
    public Task<IReadOnlyList<Note>> SearchAsync(string query)
    {
        var pattern = SqlLike.Contains(query);
        return db.QueryAsync<Note>(
            $"""
            SELECT {ListColumns} FROM Note
            WHERE ((Type = 0 AND Content LIKE ? {SqlLike.EscapeClause})
                OR Keywords LIKE ? {SqlLike.EscapeClause}
                OR Title LIKE ? {SqlLike.EscapeClause})
              AND IsSensitive = 0
            """,
            pattern, pattern, pattern);
    }

    public Task<Note?> GetAsync(int id) => db.GetAsync<Note>(id);

    public Task<int> InsertAsync(Note note) => db.InsertAsync(note);

    public Task<int> UpdateAsync(Note note) => db.UpdateAsync(note);

    public Task<int> SetSensitiveAsync(int noteId, bool isSensitive)
        => db.ExecuteAsync("UPDATE Note SET IsSensitive = ? WHERE Id = ?", isSensitive, noteId);

    // A link left behind pointed at a sketch that no longer exists, and opened an empty one
    public async Task<int> DeleteAsync(Note note)
    {
        var deleted = 0;
        await db.RunInTransactionAsync(conn =>
        {
            conn.Execute("DELETE FROM NoteLink WHERE NoteId = ? OR SketchId = ?", note.Id, note.Id);
            deleted = conn.Delete(note);
        });
        return deleted;
    }
}