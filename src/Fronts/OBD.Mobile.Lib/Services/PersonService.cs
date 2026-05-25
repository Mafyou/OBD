namespace OBD.Mobile.Lib.Services;

public class PersonService(DatabaseContext db) : IPersonService
{
    public Task<List<Person>> GetAllAsync() => db.GetAllAsync<Person>();

    public Task<Person?> GetAsync(int id) => db.GetAsync<Person>(id);

    public Task<List<Person>> SearchAsync(string query)
        => db.QueryAsync<Person>(
            "SELECT * FROM Person WHERE Name LIKE ? OR Position LIKE ? OR Memo LIKE ?",
            $"%{query}%", $"%{query}%", $"%{query}%");

    public Task<int> InsertAsync(Person personne) => db.InsertAsync(personne);

    public Task<int> UpdateAsync(Person personne) => db.UpdateAsync(personne);

    public Task<int> DeleteAsync(Person personne) => db.DeleteAsync(personne);
}
