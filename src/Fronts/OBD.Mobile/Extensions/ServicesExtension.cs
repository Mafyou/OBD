namespace OBD.Mobile.Extensions;

public static class ServicesExtension
{
    extension(IServiceCollection services)
    {
        public void AddServices()
        {
            services.AddSingleton<IPersonService, PersonService>();
            services.AddSingleton<INoteService, NoteService>();
            services.AddSingleton<ISectorService, SectorService>();
            services.AddSingleton<IReperesTravailService, ReperesTravailService>();
        }
    }
}