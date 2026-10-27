namespace OBD.Mobile.Lib.Services;

public class SectorService(DatabaseContext db) : ISectorService
{
    public Task<IReadOnlyList<Sector>> GetAllAsync() => db.GetAllAsync<Sector>();

    public Task<Sector?> GetAsync(int id) => db.GetAsync<Sector>(id);

    public Task<IReadOnlyList<Sector>> SearchAsync(string query)
        => db.QueryAsync<Sector>($"SELECT * FROM Sector WHERE Name LIKE ? {SqlLike.EscapeClause}", SqlLike.Contains(query));

    public Task<int> InsertAsync(Sector sector) => db.InsertAsync(sector);

    public Task<int> UpdateAsync(Sector sector) => db.UpdateAsync(sector);

    public Task<int> DeleteAsync(Sector sector) => db.DeleteAsync(sector);
}