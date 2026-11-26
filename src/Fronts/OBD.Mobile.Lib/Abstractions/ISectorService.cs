namespace OBD.Mobile.Lib.Abstractions;

public interface ISectorService
{
    Task<IReadOnlyList<Sector>> GetAllAsync();
    Task<Sector?> GetAsync(int id);
    Task<IReadOnlyList<Sector>> SearchAsync(string query);
    Task<int> InsertAsync(Sector sector);
    Task<int> UpdateAsync(Sector sector);

    /// <summary>The notes and people that <see cref="DeleteAsync"/> deletes with the sector, for its confirmation.</summary>
    Task<SectorContents> GetContentsAsync(int sectorId);

    /// <summary>
    /// Deletes the sector with its notes (sensitive ones included), the hashtag links from or to them, and its people;
    /// the manager of the work habits is cleared when one of them was it. All of it, or none of it.
    /// </summary>
    Task<int> DeleteAsync(Sector sector);
}
