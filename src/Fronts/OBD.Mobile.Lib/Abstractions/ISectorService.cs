namespace OBD.Mobile.Lib.Services;

public interface ISectorService
{
    Task<List<Sector>> GetAllAsync();
    Task<Sector?> GetAsync(int id);
    Task<int> InsertAsync(Sector secteur);
    Task<int> UpdateAsync(Sector secteur);
    Task<int> DeleteAsync(Sector secteur);
}