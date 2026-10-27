namespace OBD.Mobile.Lib.Abstractions;

public interface IPersonService
{
    Task<IReadOnlyList<Person>> GetAllAsync();
    Task<Person?> GetAsync(int id);
    Task<IReadOnlyList<Person>> SearchAsync(string query);
    Task<int> InsertAsync(Person person);
    Task<int> UpdateAsync(Person person);
    Task<int> DeleteAsync(Person person);
}