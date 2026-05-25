namespace OBD.Mobile.Lib.Data;

public class DatabaseContext
{
    private SQLiteAsyncConnection? _connection;
    private readonly string _dbPath = Path.Combine(FileSystem.AppDataDirectory, "obd.db3");

    private async ValueTask<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_connection is not null)
            return _connection;

        _connection = new SQLiteAsyncConnection(_dbPath,
            SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);

        await _connection.CreateTableAsync<Person>();
        await _connection.CreateTableAsync<Sector>();
        await _connection.CreateTableAsync<Note>();
        await _connection.CreateTableAsync<ReperesTravail>();
        return _connection;
    }

    public async Task<List<T>> GetAllAsync<T>() where T : new()
    {
        var conn = await GetConnectionAsync();
        return await conn.Table<T>().ToListAsync();
    }

    public async Task<T?> GetAsync<T>(int id) where T : new()
    {
        var conn = await GetConnectionAsync();
        return await conn.FindAsync<T>(id);
    }

    public async Task<int> InsertAsync<T>(T item)
    {
        var conn = await GetConnectionAsync();
        return await conn.InsertAsync(item);
    }

    public async Task<int> UpdateAsync<T>(T item)
    {
        var conn = await GetConnectionAsync();
        return await conn.UpdateAsync(item);
    }

    public async Task<int> DeleteAsync<T>(T item)
    {
        var conn = await GetConnectionAsync();
        return await conn.DeleteAsync(item);
    }

    public async Task<List<T>> QueryAsync<T>(string query, params object[] args) where T : new()
    {
        var conn = await GetConnectionAsync();
        return await conn.QueryAsync<T>(query, args);
    }
}