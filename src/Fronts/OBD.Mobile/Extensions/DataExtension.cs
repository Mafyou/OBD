namespace OBD.Mobile.Extensions;

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