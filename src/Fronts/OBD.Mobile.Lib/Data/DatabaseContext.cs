namespace OBD.Mobile.Lib.Data;

public class DatabaseContext(string databasePath)
{
    // Several screens query at once on a cold start (the search runs three queries together): the connection and
    // its tables are made once, by the first of them, and the others wait for it instead of opening their own
    private readonly SemaphoreSlim _connectionGate = new(1, 1);
    private SQLiteAsyncConnection? _connection;

    private async ValueTask<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_connection is { } ready)
            return ready;

        await _connectionGate.WaitAsync().ConfigureAwait(false);
        try
        {
            if (_connection is { } created)
                return created;

            var connection = new SQLiteAsyncConnection(databasePath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
            await connection.CreateTablesAsync(CreateFlags.None,
                typeof(Sector), typeof(Person), typeof(Note), typeof(NoteLink), typeof(WorkHabits)).ConfigureAwait(false);

            return _connection = connection;
        }
        finally
        {
            _connectionGate.Release();
        }
    }

    public async Task<IReadOnlyList<T>> GetAllAsync<T>() where T : new()
    {
        var conn = await GetConnectionAsync().ConfigureAwait(false);
        return await conn.Table<T>().ToListAsync().ConfigureAwait(false);
    }

    public async Task<T?> GetAsync<T>(int id) where T : new()
    {
        var conn = await GetConnectionAsync().ConfigureAwait(false);
        return await conn.FindAsync<T>(id).ConfigureAwait(false);
    }

    public async Task<int> InsertAsync<T>(T item)
    {
        var conn = await GetConnectionAsync().ConfigureAwait(false);
        return await conn.InsertAsync(item).ConfigureAwait(false);
    }

    public async Task<int> UpdateAsync<T>(T item)
    {
        var conn = await GetConnectionAsync().ConfigureAwait(false);
        return await conn.UpdateAsync(item).ConfigureAwait(false);
    }

    public async Task<int> DeleteAsync<T>(T item)
    {
        var conn = await GetConnectionAsync().ConfigureAwait(false);
        return await conn.DeleteAsync(item).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<T>> QueryAsync<T>(string query, params object[] args) where T : new()
    {
        var conn = await GetConnectionAsync().ConfigureAwait(false);
        return await conn.QueryAsync<T>(query, args).ConfigureAwait(false);
    }

    /// <summary>The first column of each row of <paramref name="query"/>, without building a whole record.</summary>
    public async Task<IReadOnlyList<T>> QueryScalarsAsync<T>(string query, params object[] args)
    {
        var conn = await GetConnectionAsync().ConfigureAwait(false);
        return await conn.QueryScalarsAsync<T>(query, args).ConfigureAwait(false);
    }

    public async Task<T> ExecuteScalarAsync<T>(string query, params object[] args)
    {
        var conn = await GetConnectionAsync().ConfigureAwait(false);
        return await conn.ExecuteScalarAsync<T>(query, args).ConfigureAwait(false);
    }

    public async Task<int> ExecuteAsync(string query, params object[] args)
    {
        var conn = await GetConnectionAsync().ConfigureAwait(false);
        return await conn.ExecuteAsync(query, args).ConfigureAwait(false);
    }

    /// <summary>
    /// Runs <paramref name="work"/> as one transaction: all of it is written, or none of it when it throws. Many writes
    /// also go faster together than in one transaction each.
    /// </summary>
    public async Task RunInTransactionAsync(Action<SQLiteConnection> work)
    {
        var conn = await GetConnectionAsync().ConfigureAwait(false);
        await conn.RunInTransactionAsync(work).ConfigureAwait(false);
    }
}