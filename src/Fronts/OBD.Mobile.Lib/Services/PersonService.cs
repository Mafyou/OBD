namespace OBD.Mobile.Lib.Services;

public class PersonService(DatabaseContext db) : IPersonService
{
    public Task<IReadOnlyList<Person>> GetAllAsync() => db.GetAllAsync<Person>();

    public Task<Person?> GetAsync(int id) => db.GetAsync<Person>(id);

    public Task<IReadOnlyList<Person>> SearchAsync(string query)
    {
        var pattern = SqlLike.Contains(query);
        return db.QueryAsync<Person>(
            $"SELECT * FROM Person WHERE Name LIKE ? {SqlLike.EscapeClause} OR Position LIKE ? {SqlLike.EscapeClause} OR Memo LIKE ? {SqlLike.EscapeClause}",
            pattern, pattern, pattern);
    }

    public Task<int> InsertAsync(Person person) => db.InsertAsync(person);

    public Task<int> UpdateAsync(Person person) => db.UpdateAsync(person);

    // The work habits kept the id of a deleted manager, and the export pointed at someone no longer in it
    public async Task<int> DeleteAsync(Person person)
    {
        var deleted = 0;
        await db.RunInTransactionAsync(conn =>
        {
            conn.Execute("UPDATE WorkHabits SET ManagerId = 0 WHERE ManagerId = ?", person.Id);
            deleted = conn.Delete(person);
        }).ConfigureAwait(false);
        return deleted;
    }
}