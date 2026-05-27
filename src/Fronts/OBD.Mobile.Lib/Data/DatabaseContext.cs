namespace OBD.Mobile.Lib.Data;

public class DatabaseContext(string databasePath)
{
    private SQLiteAsyncConnection? _connection;
    private readonly string _dbPath = databasePath;

    private async ValueTask<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_connection is not null)
            return _connection;

        _connection = new SQLiteAsyncConnection(_dbPath,
            SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);


        await _connection.CreateTableAsync<Sector>();
        await _connection.CreateTableAsync<Person>();
        await _connection.CreateTableAsync<Note>();
        await _connection.CreateTableAsync<NoteLink>();
        await _connection.CreateTableAsync<WorkHabits>();
        await MigrateNoteLinkAsync();
        await MigrateWorkHabitsAsync();

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

    private async Task MigrateNoteLinkAsync()
    {
        // Ajoute la colonne PhotoId si elle n'existe pas encore (appareils anciens)
        try { await _connection!.ExecuteAsync("ALTER TABLE NoteLink ADD COLUMN PhotoId INTEGER NOT NULL DEFAULT 0"); }
        catch { /* déjà présente */ }

        // Fusionne les liens photo vers SketchId et nettoie
        await _connection!.ExecuteAsync(
            "UPDATE NoteLink SET SketchId = PhotoId WHERE SketchId = 0 AND PhotoId != 0");
    }

    private async Task MigrateWorkHabitsAsync()
    {
        var oldTableExists = await _connection!.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM sqlite_master WHERE type='table' AND name='ReperesTravail'");
        if (oldTableExists is 0)
            return;

        var oldRows = await _connection.QueryAsync<LegacyWorkHabits>(
            "SELECT Id, RegularMeetings, RemoteWorkDays, Manager FROM ReperesTravail");
        if (oldRows.Count is 0)
            return;

        var persons = await _connection.Table<Person>().ToListAsync();
        var existing = await _connection.Table<WorkHabits>().ToListAsync();

        foreach (var old in oldRows)
        {
            var managerId = persons
                .FirstOrDefault(p => string.Equals(p.Name, old.Manager, StringComparison.OrdinalIgnoreCase))?.Id ?? 0;

            var current = existing.FirstOrDefault(w => w.Id == old.Id);
            if (current is null)
            {
                await _connection.InsertAsync(new WorkHabits
                {
                    Id = old.Id,
                    RegularMeetings = old.RegularMeetings,
                    RemoteWorkDays = old.RemoteWorkDays,
                    ManagerId = managerId
                });
            }
            else
            {
                current.RegularMeetings = old.RegularMeetings;
                current.RemoteWorkDays = old.RemoteWorkDays;
                current.ManagerId = managerId;
                await _connection.UpdateAsync(current);
            }
        }

        await _connection.ExecuteAsync("DROP TABLE IF EXISTS ReperesTravail");
    }

    private sealed class LegacyWorkHabits
    {
        public int Id { get; set; }
        public string RegularMeetings { get; set; } = string.Empty;
        public string RemoteWorkDays { get; set; } = string.Empty;
        public string Manager { get; set; } = string.Empty;
    }
}
