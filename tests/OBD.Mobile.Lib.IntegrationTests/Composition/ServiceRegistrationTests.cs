namespace OBD.Mobile.Lib.IntegrationTests.Composition;

[Collection(SqliteCollection.Name)]
public class ServiceRegistrationTests
{
    [Fact]
    public void AddServices_ShouldResolveEveryServiceOfTheApp()
    {
        // Arrange
        using var workspace = new TempWorkspace();
        using var provider = workspace.BuildServices();

        // Act
        var resolved = new object[]
        {
            provider.GetRequiredService<IPersonService>(),
            provider.GetRequiredService<INoteService>(),
            provider.GetRequiredService<ISectorService>(),
            provider.GetRequiredService<INoteLinkService>(),
            provider.GetRequiredService<IWorkHabitsService>(),
            provider.GetRequiredService<IDataExportService>(),
            provider.GetRequiredService<IAppCrashStateService>(),
            provider.GetRequiredService<ICrashReportMailer>(),
            provider.GetRequiredService<IShakeDetectionService>(),
            provider.GetRequiredService<DataSeeder>(),
        };

        // Assert
        resolved.ShouldNotContain(service => service == null);
        provider.GetRequiredService<TimeProvider>().ShouldBeSameAs(TimeProvider.System);
    }

    [Fact]
    public void AddServices_ShouldRegisterEveryServiceAsASingleton()
    {
        // Arrange
        ServiceCollection services = new();

        // Act
        services.AddServices();

        // Assert
        services.ShouldNotBeEmpty();
        services.ShouldAllBe(descriptor => descriptor.Lifetime == ServiceLifetime.Singleton);
        services.Select(d => d.ServiceType).ShouldBeUnique();
    }

    [Fact]
    public void AddServices_ShouldGiveTheSameInstance_EachTimeAServiceIsAskedFor()
    {
        // Arrange
        using var workspace = new TempWorkspace();
        using var provider = workspace.BuildServices();

        // Act
        var first = provider.GetRequiredService<INoteService>();
        var second = provider.GetRequiredService<INoteService>();

        // Assert
        second.ShouldBeSameAs(first);
        provider.GetRequiredService<DataSeeder>().ShouldNotBeSameAs(provider.GetRequiredService<DataSeeder>());
    }

    [Fact]
    public async Task Services_ShouldShareOneDatabase()
    {
        // Arrange
        using var workspace = new TempWorkspace();
        using var provider = workspace.BuildServices();
        var sectors = provider.GetRequiredService<ISectorService>();
        var persons = provider.GetRequiredService<IPersonService>();
        var sector = new Sector { Name = "Support" };
        _ = await sectors.InsertAsync(sector);

        // Act
        _ = await persons.InsertAsync(new Person { Name = "Nadia Haddad", Position = "Agent", SectorId = sector.Id });
        var found = await persons.SearchAsync("Nadia");
        var sectorOfFound = await sectors.GetAsync(found.ShouldHaveSingleItem().SectorId);

        // Assert
        sectorOfFound.ShouldNotBeNull().Name.ShouldBe("Support");
        File.Exists(workspace.DatabasePath).ShouldBeTrue();
    }
}
