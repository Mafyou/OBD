namespace OBD.Mobile.Lib.UnitTests.TestInfrastructure;

/// <summary>The tests that set <see cref="AppResources.Culture"/>, a static: one class at a time, or one would read the
/// language another has just set.</summary>
[CollectionDefinition("AppResourcesCulture", DisableParallelization = true)]
public class AppResourcesCultureCollection;
