namespace OBD.Mobile.Lib.Services;

public class SectorService(DatabaseContext db) : ISectorService
{
    public Task<IReadOnlyList<Sector>> GetAllAsync() => db.GetAllAsync<Sector>();

    public Task<Sector?> GetAsync(int id) => db.GetAsync<Sector>(id);

    public Task<IReadOnlyList<Sector>> SearchAsync(string query)
        => db.QueryAsync<Sector>($"SELECT * FROM Sector WHERE Name LIKE ? {SqlLike.EscapeClause}", SqlLike.Contains(query));

    public Task<int> InsertAsync(Sector sector) => db.InsertAsync(sector);

    public Task<int> UpdateAsync(Sector sector) => db.UpdateAsync(sector);

    public async Task<SectorContents> GetContentsAsync(int sectorId)
    {
        var notes = await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Note WHERE SectorId = ?", sectorId).ConfigureAwait(false);
        var persons = await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Person WHERE SectorId = ?", sectorId).ConfigureAwait(false);
        return new(notes, persons);
    }

    // The notes and people of a deleted sector stayed behind, filed under a sector that no longer existed
    public async Task<int> DeleteAsync(Sector sector)
    {
        var deleted = 0;
        await db.RunInTransactionAsync(conn =>
        {
            conn.Execute(
                """
                DELETE FROM NoteLink
                WHERE NoteId IN (SELECT Id FROM Note WHERE SectorId = ?)
                   OR SketchId IN (SELECT Id FROM Note WHERE SectorId = ?)
                """,
                sector.Id, sector.Id);
            conn.Execute("DELETE FROM Note WHERE SectorId = ?", sector.Id);
            conn.Execute("UPDATE WorkHabits SET ManagerId = 0 WHERE ManagerId IN (SELECT Id FROM Person WHERE SectorId = ?)", sector.Id);
            conn.Execute("DELETE FROM Person WHERE SectorId = ?", sector.Id);
            deleted = conn.Delete(sector);
        }).ConfigureAwait(false);
        return deleted;
    }
}
