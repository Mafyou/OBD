namespace OBD.Mobile.Lib.IntegrationTests.TestInfrastructure;

/// <summary>
/// Each test owns its database file, but closing the pooled SQLite connections (<see cref="TempWorkspace"/>) is
/// process-wide: the tests touching SQLite run one at a time.
/// </summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class SqliteCollection
{
    public const string Name = "Sqlite";
}
