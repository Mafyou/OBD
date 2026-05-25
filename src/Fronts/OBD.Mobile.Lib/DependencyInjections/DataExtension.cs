namespace OBD.Mobile.Lib.DependencyInjections;

public static class DataExtension
{
    extension(IServiceCollection services)
    {
        public void AddData()
        {
            services.AddSingleton<DatabaseContext>();
        }
    }
}