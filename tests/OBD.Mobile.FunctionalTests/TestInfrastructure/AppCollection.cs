namespace OBD.Mobile.FunctionalTests.TestInfrastructure;

/// <summary>
/// The fake platform (<see cref="AppPlatform"/>) is static, as the MAUI one it stands for, and the SQLite connection
/// pool is process-wide: the journeys run one at a time.
/// </summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class AppCollection
{
    public const string Name = "App";
}
