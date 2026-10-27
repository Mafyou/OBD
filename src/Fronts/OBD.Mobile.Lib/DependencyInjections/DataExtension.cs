namespace OBD.Mobile.Lib.DependencyInjections;

public static class DataExtension
{
    extension(IServiceCollection services)
    {
        public void AddData()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "obd.db3");
            services.AddSingleton(_ => new DatabaseContext(dbPath));
            services.AddTransient<DataSeeder>();
        }
    }
}