namespace OBD.Mobile.Lib.Abstractions;

public interface IPersonService
{
    Task<List<Person>> GetAllAsync();
    Task<Person?> GetAsync(int id);
    Task<List<Person>> SearchAsync(string query);
    Task<int> InsertAsync(Person personne);
    Task<int> UpdateAsync(Person personne);
    Task<int> DeleteAsync(Person personne);
}