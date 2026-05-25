namespace OBD.Mobile.Lib.Services;

public class SectorService(DatabaseContext db) : ISectorService
{
    public Task<List<Sector>> GetAllAsync() => db.GetAllAsync<Sector>();

    public Task<Sector?> GetAsync(int id) => db.GetAsync<Sector>(id);

    public Task<List<Sector>> SearchAsync(string query)
        => db.QueryAsync<Sector>("SELECT * FROM Sector WHERE Name LIKE ?", $"%{query}%");

    public Task<int> InsertAsync(Sector secteur) => db.InsertAsync(secteur);

    public Task<int> UpdateAsync(Sector secteur) => db.UpdateAsync(secteur);

    public Task<int> DeleteAsync(Sector secteur) => db.DeleteAsync(secteur);
}