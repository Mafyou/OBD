namespace OBD.Mobile.Lib.Abstractions;

public interface ISectorService
{
    Task<IReadOnlyList<Sector>> GetAllAsync();
    Task<Sector?> GetAsync(int id);
    Task<IReadOnlyList<Sector>> SearchAsync(string query);
    Task<int> InsertAsync(Sector sector);
    Task<int> UpdateAsync(Sector sector);
    Task<int> DeleteAsync(Sector sector);
}